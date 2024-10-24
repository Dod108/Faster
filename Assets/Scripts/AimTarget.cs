using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTarget : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    private Obstacle lastObstacle;

    private void Update()
    {
        Vector2 aimVector = GameInput.Instance.GetAimVector();
        Ray ray = Camera.main.ScreenPointToRay(aimVector);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            transform.position = raycastHit.point;

            if (raycastHit.collider.gameObject.TryGetComponent<ObstacleTarget>(out ObstacleTarget target))
            {
                Obstacle obstacle = target.GetObstacle();
                obstacle.IsHighlighted = true;
                if (lastObstacle != obstacle)
                {
                    if (lastObstacle != null) lastObstacle.IsHighlighted = false;
                    lastObstacle = obstacle;
                }
            }
            else if (lastObstacle != null)
            {
                lastObstacle.IsHighlighted = false;
            }
        }
        else
        {
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(aimVector.x, aimVector.y, Camera.main.farClipPlane));

            if (lastObstacle != null)
            {
                lastObstacle.IsHighlighted = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<ObstacleTarget>(out ObstacleTarget target))
        {
            target.GetObstacle().IsHighlighted = false;
        }
    }
}
