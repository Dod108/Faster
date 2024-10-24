using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float destroyTimer = 2f;
    [SerializeField] bool destroyOnCollision = true;
    [SerializeField] bool destroyOnTrigger = false;

    private void Update()
    {
        destroyTimer -= Time.deltaTime;

        if (destroyTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<ObstacleTarget>(out ObstacleTarget obstacleTarget))
        {
            obstacleTarget.GetObstacle().OnProjectileCollision(this);
        }

        if (destroyOnCollision) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Interactive>(out Interactive interactive))
        {
            interactive.OnProjectileCollision(this);
        }

        if (destroyOnTrigger) Destroy(gameObject);
    }
}
