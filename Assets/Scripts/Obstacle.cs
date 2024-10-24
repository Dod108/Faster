using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static PlayerController;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float maxDurability = 2f;
    [SerializeField] private ObstacleTarget target;
    [SerializeField] private GameObject visual;
    [SerializeField] private float speed = 0f;
    [SerializeField] private Vector3 direction = Vector3.right;
    [SerializeField] private bool randomDirection = false;
    [SerializeField] private float additionalOffsetX = 0f;
    [SerializeField] private float additionalOffsetY = 0f;

    public static event EventHandler<ObstacleHitEventArgs> ObstacleHit;
    public static event EventHandler<ObstacleShatterEventArgs> ObstacleShatter;
    public class ObstacleHitEventArgs : EventArgs
    {
        public Vector3 position;
    }
    public class ObstacleShatterEventArgs : EventArgs
    {
        public Vector3 position;
    }
    public static void ResetStaticData()
    {
        ObstacleHit = null;
        ObstacleShatter = null;
    }
    public bool IsHighlighted { get; set; } = false;

    private float durability;
    private float offsetX;
    private float offsetY;
    private float chunkSizeX;
    private float chunkSizeY;
    private bool bounce;

    private void Start()
    {
        Bounds bounds = visual.GetComponent<MeshFilter>().mesh.bounds;
        offsetX = bounds.extents.x * visual.transform.lossyScale.x + additionalOffsetX;
        offsetY = bounds.extents.y * visual.transform.lossyScale.y + additionalOffsetY;

        chunkSizeX = GameManager.Instance.GetChunkSize().x;
        chunkSizeY = GameManager.Instance.GetChunkSize().y;

        if (randomDirection) direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
        direction.Normalize();

        durability = maxDurability;
    }

    private void Update()
    {
        if (speed > 0f) Move();
    }

    private void Move()
    {
        transform.position += direction * Time.deltaTime * speed;

        bool bounceX = transform.position.x < -chunkSizeX / 2 + offsetX || transform.position.x > chunkSizeX / 2 - offsetX;
        bool bounceY = transform.position.y < -chunkSizeY / 2 + offsetY || transform.position.y > chunkSizeY / 2 - offsetY;

        if ((bounceX && direction.x != 0f) || (bounceY && direction.y != 0f))
        {
            if (!bounce)
            {
                direction = -direction;
                bounce = true;
            }
        }
        else
        {
            bounce = false;
        }
    }

    private void Shatter()
    {
        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            Transform child = transform.GetChild(i);
            if (child.TryGetComponent<MeshDestroy>(out MeshDestroy meshChildDestroy))
            {
                meshChildDestroy.DestroyMesh();
            }
        }

        ObstacleShatter?.Invoke(this, new ObstacleShatterEventArgs { position = transform.position });
    }

    public void OnProjectileCollision(Projectile projectile)
    {
        durability -= PlayerController.Instance.GetDamage();
        if (durability <= 0f)
        {
            Shatter();
        }
        else
        {
            ObstacleHit?.Invoke(this, new ObstacleHitEventArgs { position = transform.position });
            target.Flash();
        }
    }

    public void OnPlayerCollision(PlayerController playerController)
    {
        Shatter();
    }

    public GameObject GetVisual()
    {
        return visual;
    }

    public float GetDurability()
    {
        return durability;
    }
}
