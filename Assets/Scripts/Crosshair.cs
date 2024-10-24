using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Transform aimTarget;
    [SerializeField] private GameObject crosshairVisual;

    private void Update()
    {
        if (aimTarget != null && crosshairVisual != null)
        {
            crosshairVisual.transform.position = Camera.main.WorldToScreenPoint(aimTarget.position);
        }
    }
}
