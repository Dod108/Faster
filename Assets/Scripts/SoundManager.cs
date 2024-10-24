using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private float defaultSfxVolume = 1.0f;
    [SerializeField] private float positionZMultiplier = 0.1f;
    [SerializeField] private float shootingVolume = 0.25f;
    [SerializeField] private List<AudioClip> shoot;
    [SerializeField] private AudioClip obstacleHit;
    [SerializeField] private AudioClip obstacleShatter;
    [SerializeField] private AudioClip playerHit;
    [SerializeField] private AudioClip timeAdded;
    [SerializeField] private AudioClip projectileProgress;
    [SerializeField] private AudioClip projectileAdded;

    public static SoundManager Instance { get; private set; }

    private const string PLAYER_PREFS_SFX_VOLUME = "SfxVolume";
    private float sfxVolume;

    private void Awake()
    {
        Instance = this;

        ChangeVolume(PlayerPrefs.GetFloat(PLAYER_PREFS_SFX_VOLUME, defaultSfxVolume));
    }

    private void Start()
    {
        Obstacle.ObstacleHit += Obstacle_ObstacleHit;
        Obstacle.ObstacleShatter += Obstacle_ObstacleShatter;
        PlayerController.Instance.TimeAdded += PlayerController_TimeAdded;
        PlayerController.Instance.ProjectileProgressChanged += PlayerController_ProjectileProgressChanged;
        PlayerController.Instance.PlayerHit += PlayerController_PlayerHit;
        PlayerController.Instance.Shoot += PlayerController_Shoot;
    }

    private void PlayerController_Shoot(object sender, System.EventArgs e)
    {
        PlaySound(shoot, shootingVolume);
    }

    private void PlayerController_PlayerHit(object sender, System.EventArgs e)
    {
        PlaySound(playerHit);
    }

    private void PlayerController_ProjectileProgressChanged(object sender, PlayerController.ProjectileProgressChangedEventArgs e)
    {
        if (e.progressIncreased)
        {
            if (e.numberIncreased)
            {
                PlaySound(projectileAdded);
            }
            else
            {
                PlaySound(projectileProgress);
            }
        }
    }

    private void PlayerController_TimeAdded(object sender, PlayerController.TimerChangedEventArgs e)
    {
        PlaySound(timeAdded);
    }

    private void Obstacle_ObstacleHit(object sender, Obstacle.ObstacleHitEventArgs e)
    {
        PlaySound(obstacleHit, e.position);
    }

    private void Obstacle_ObstacleShatter(object sender, Obstacle.ObstacleShatterEventArgs e)
    {
        PlaySound(obstacleHit, e.position);
        PlaySound(obstacleShatter, e.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        position.z = Camera.main.transform.position.z + (position.z - Camera.main.transform.position.z) * positionZMultiplier;

        if (audioClip != null) AudioSource.PlayClipAtPoint(audioClip, position, volume * sfxVolume);
    }

    private void PlaySound(AudioClip audioClip, float volume = 1f)
    {
        PlaySound(audioClip, Camera.main.transform.position, volume);
    }

    private void PlaySound(List<AudioClip> audioClips, Vector3 position, float volume = 1f)
    {
        if (audioClips.Count > 0)
        {
            AudioClip audioClip = audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
            PlaySound(audioClip, position, volume);
        }
    }

    private void PlaySound(List<AudioClip> audioClips, float volume = 1f)
    {
        PlaySound(audioClips, Camera.main.transform.position, volume);
    }

    public void ChangeVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_SFX_VOLUME, sfxVolume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return sfxVolume;
    }
}
