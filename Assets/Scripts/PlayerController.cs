using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float initialForwardSpeed = 30f;
    [SerializeField] private float forwardAcceleration = 2.0f;
    [SerializeField] private float sideSpeed = 5f;
    [SerializeField] private float bouncingSpeed = 20f;
    [SerializeField] private float bounceNormalZCutoff = -0.7f;
    [SerializeField] private float timer = 15f;
    [SerializeField] private float stopAcceleration = 1.0f;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float projectileSpeed = 100f;
    [SerializeField] private float projectileOffset = 2f;
    [SerializeField] private Transform aimTarget;
    [SerializeField] private float hitTimeDamage = 10f;
    [SerializeField] private float distanceToScoreMultiplier = 0.1f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private bool autoFire = true;
    [SerializeField] private int projectileNumber = 1;
    [SerializeField] private float projectileSpread = 0.5f;
    [SerializeField] private List<int> projectileNumberThresholds;

    public static PlayerController Instance { get; private set; }
    public event EventHandler SpeedIncreased;
    public event EventHandler TimerStarted;
    public event EventHandler Shoot;
    public event EventHandler PlayerHit;
    public event EventHandler<TimerChangedEventArgs> TimeAdded;
    public event EventHandler<TimerChangedEventArgs> TimeRemoved;
    public class TimerChangedEventArgs : EventArgs
    {
        public float time;
    }
    public event EventHandler<ProjectileProgressChangedEventArgs> ProjectileProgressChanged;
    public class ProjectileProgressChangedEventArgs : EventArgs
    {
        public float projectileNumber;
        public float progressNormalized;
        public bool progressIncreased;
        public bool numberIncreased;
    }
    public event EventHandler<LevelChangedEventArgs> LevelChanged;
    public class LevelChangedEventArgs : EventArgs
    {
        public LevelSO newLevel;
    }

    private static float highScore = 0f;
    private float forwardSpeed;
    private CharacterController characterController;
    private float speed;
    private float speedOffset = 0.1f;
    private bool bouncing = false;
    private float score = 0f;
    private float initialZ;
    private int currentChunk;
    private LevelSO currentLevel;
    private float fireTimer = 0f;
    private bool fire = false;
    private bool timerStarted = false;
    private int projectileNumberProgress = 0;
    private int maxProjectileNumber;

    private void Awake()
    {
        Instance = this;

        maxProjectileNumber = projectileNumberThresholds.Count + 1;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        initialZ = transform.position.z;
        forwardSpeed = initialForwardSpeed;

        GameInput.Instance.Fire += GameInput_Fire;
        GameInput.Instance.FireStop += GameInput_FireStop;

        GameManager.Instance.GameStarted += GameManager_GameStarted;
    }

    private void GameManager_GameStarted(object sender, EventArgs e)
    {
        currentChunk = GameManager.Instance.GetCurrentChunk(transform.position);
        currentLevel = GameManager.Instance.GetCurrentLevel(transform.position);
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            ManageChunksAndLevels();
            Move();
            UpdateScore();
            UpdateTimer();
            ManageFire();
        }
        else if (GameManager.Instance.IsGameOver())
        {
            Move(false);
        }
    }

    private void ManageChunksAndLevels()
    {
        GameManager.Instance.GenerateLevel(transform.position);

        int newChunk = GameManager.Instance.GetCurrentChunk(transform.position);
        if (newChunk > currentChunk)
        {
            currentChunk = newChunk;
            OnChunkIncrease();

            LevelSO newLevel = GameManager.Instance.GetCurrentLevel(transform.position);
            if (newLevel != currentLevel)
            {
                currentLevel = newLevel;
                OnLevelChange();
            }
        }
    }

    private void Move(bool playerControl = true)
    {
        // Get input.
        Vector2 inputVector = Vector2.zero;
        if (playerControl) inputVector = GameInput.Instance.GetMoveVector();
        Vector3 moveDirection = new Vector3(inputVector.x, inputVector.y, 0f).normalized * sideSpeed;

        // Get target speed and acceleration.
        float targetSpeed = 0f;
        float targetAcceleration = stopAcceleration;
        if (playerControl)
        {
            targetSpeed = forwardSpeed;
            targetAcceleration = forwardAcceleration;
        }

        // Accelerate or decelerate to target speed.
        float currentForwardSpeed = characterController.velocity.z;
        if (bouncing)
        {
            speed = -bouncingSpeed;
            bouncing = false;
        }
        else if (currentForwardSpeed < targetSpeed - speedOffset || currentForwardSpeed > targetSpeed + speedOffset)
        {
            // Creates curved result rather than a linear one giving a more organic speed change.
            // Note T in Lerp is clamped, so we don't need to clamp our speed.
            speed = Mathf.Lerp(currentForwardSpeed, targetSpeed, targetAcceleration * Time.deltaTime);

            // Round speed to 3 decimal places.
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }
        moveDirection.z = speed;

        // Move character controller.
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void UpdateScore()
    {
        float newScore = Mathf.Abs(transform.position.z - initialZ) * distanceToScoreMultiplier;
        if (newScore > score) score = newScore;

        if (score > highScore) highScore = score;
    }

    private void UpdateTimer()
    {
        if (timerStarted) timer = Mathf.Max(timer - Time.deltaTime, 0f);

        if (timer <= 0f)
        {
            GameManager.Instance.SetGameOver();
        }
    }

    private void OnChunkIncrease()
    {
        if (currentChunk == 1)
        {
            StartTimer();
        }
    }

    private void OnLevelChange()
    {
        forwardSpeed += currentLevel.forwardSpeedIncrease;

        if (currentLevel.forwardSpeedIncrease > 0f) SpeedIncreased?.Invoke(this, EventArgs.Empty);

        LevelChanged?.Invoke(this, new LevelChangedEventArgs { newLevel = currentLevel });
    }

    private void StartTimer()
    {
        timerStarted = true;

        TimerStarted?.Invoke(this, EventArgs.Empty);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit != null)
        {
            if (hit.normal.z < bounceNormalZCutoff)
            {
                bouncing = true;
                RemoveTime(hitTimeDamage);
                ResetProjectileNumber();
                PlayerHit?.Invoke(this, EventArgs.Empty);

                if (hit.gameObject.transform.parent.TryGetComponent<Obstacle>(out Obstacle obstacle))
                {
                    obstacle.OnPlayerCollision(this);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Interactive>(out Interactive interactive))
        {
            interactive.OnPlayerCollision(this);
        }
    }

    public float GetTimer()
    {
        return timer;
    }

    public void AddTime(float time)
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            timer += time;
            TimeAdded?.Invoke(this, new TimerChangedEventArgs { time = time });
        }    
    }

    public void RemoveTime(float time)
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            timer -= time;
            TimeRemoved?.Invoke(this, new TimerChangedEventArgs { time = time });
        }
    }

    public float GetScore()
    {
        return score;
    }

    public float GetHighScore()
    {
        return highScore;
    }

    public void SetHighScore(float score)
    {
        highScore = score;
    }

    public float GetDamage()
    {
        return damage;
    }

    private void AddProjectileNumber()
    {
        projectileNumber++;
    }

    public void AddProjectileNumberProgress()
    {
        if (projectileNumber <= projectileNumberThresholds.Count)
        {
            bool numberIncreased = false;
            projectileNumberProgress++;
            if (projectileNumberProgress >= projectileNumberThresholds[projectileNumber - 1])
            {
                AddProjectileNumber();
                projectileNumberProgress = 0;
                numberIncreased = true;
            }

            float progressNormalized = 0f;
            if (projectileNumber <= projectileNumberThresholds.Count)
            {
                progressNormalized = (float)projectileNumberProgress / (float)projectileNumberThresholds[projectileNumber - 1];
            }
            ProjectileProgressChanged?.Invoke(this, new ProjectileProgressChangedEventArgs { projectileNumber = projectileNumber, progressNormalized = progressNormalized, progressIncreased = true, numberIncreased = numberIncreased});
        }
    }

    private void ResetProjectileNumber()
    {
        projectileNumber = 1;
        projectileNumberProgress = 0;

        ProjectileProgressChanged?.Invoke(this, new ProjectileProgressChangedEventArgs { projectileNumber = projectileNumber, progressNormalized = 0f, progressIncreased = false, numberIncreased = false });
    }

    private void GameInput_Fire(object sender, System.EventArgs e)
    {
        if (!autoFire)
        {
            fireTimer = 0f;
            fire = true;
        }
    }

    private void GameInput_FireStop(object sender, EventArgs e)
    {
        if (!autoFire)
        {
            fire = false;
        }
    }

    private void FireSingleProjectile(Vector2 multiProjectileOffset)
    {
        if (GameManager.Instance.IsGamePlaying() && aimTarget != null)
        {
            Vector3 projectileDirection = (aimTarget.position - transform.position).normalized;
            Vector3 projectilePosition = transform.position + projectileDirection * projectileOffset;
            GameObject currentProjectile = Instantiate(projectile, projectilePosition, Quaternion.LookRotation(projectileDirection));
            currentProjectile.transform.position += currentProjectile.transform.right * multiProjectileOffset.x + currentProjectile.transform.up * multiProjectileOffset.y;

            // Don't add player velocity vector as in realistic physics (for easier aiming), add only velocity magnitude.
            float finalProjectileSpeed = (currentProjectile.transform.forward * projectileSpeed + characterController.velocity).magnitude;
            currentProjectile.GetComponent<Rigidbody>().linearVelocity = currentProjectile.transform.forward * finalProjectileSpeed;
        }
    }

    private void FireProjectiles()
    {
        if (projectileNumber == 1)
        {
            FireSingleProjectile(Vector2.zero);
        }
        else if (projectileNumber == 2)
        {
            FireSingleProjectile(new Vector2(-1f, 0f) * projectileSpread);
            FireSingleProjectile(new Vector2(1f, 0f) * projectileSpread);
        }
        else if (projectileNumber == 3)
        {
            FireSingleProjectile(new Vector2(0f, 2 * 1.732f / 3f) * projectileSpread);
            FireSingleProjectile(new Vector2(-1f, -1.732f / 3f) * projectileSpread);
            FireSingleProjectile(new Vector2(1f, -1.732f / 3f) * projectileSpread);
        }
        else if (projectileNumber == 4)
        {
            FireSingleProjectile(new Vector2(1f, 1f) * projectileSpread);
            FireSingleProjectile(new Vector2(-1f, 1f) * projectileSpread);
            FireSingleProjectile(new Vector2(1f, -1f) * projectileSpread);
            FireSingleProjectile(new Vector2(-1f, -1f) * projectileSpread);
        }
        else if (projectileNumber >= 5)
        {
            FireSingleProjectile(new Vector2(1f, 1f) * projectileSpread);
            FireSingleProjectile(new Vector2(-1f, 1f) * projectileSpread);
            FireSingleProjectile(new Vector2(1f, -1f) * projectileSpread);
            FireSingleProjectile(new Vector2(-1f, -1f) * projectileSpread);
            FireSingleProjectile(new Vector2(0f, 0f) * projectileSpread);
        }

        Shoot?.Invoke(this, EventArgs.Empty);
    }

    private void ManageFire()
    {
        if (fire || autoFire)
        {
            if (fireTimer <= 0f)
            {
                FireProjectiles();
                fireTimer = fireRate;
            }
            else
            {
                fireTimer -= Time.deltaTime;
            }
        }
    }

    public int GetProjectileNumber()
    {
        return projectileNumber;
    }

    public int GetMaxProjectileNumber()
    { 
        return maxProjectileNumber; 
    }
}
