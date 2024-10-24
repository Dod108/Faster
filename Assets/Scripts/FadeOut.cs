using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public float finalAlpha = 0f;
    public float distanceFadeStart = 7f;
    public float distanceFadeStop = 1f;
    public float timeFadeOutRate = 8f;
    public Material transparentMaterial;

    private MeshRenderer meshRenderer;
    private Color color;
    private float originalAlpha;
    private float originalIntensity;
    private bool isTransparent = false;
    private bool timeFadeOutTriggered = false;
    private bool destroyAfterTimeFadeOut = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        color = transparentMaterial.color;
        originalAlpha = color.a;
    }

    private void Update()
    {
        if (timeFadeOutTriggered)
        {
            TimeFadeOut();
        }
        else
        {
            DistanceFadeOut();
        }
    }

    private void SetMaterialTransparent()
    {
        var materialsCopy = meshRenderer.materials;
        materialsCopy[0] = transparentMaterial;
        meshRenderer.materials = materialsCopy;
        isTransparent = true;
    }

    private void DistanceFadeOut()
    {
        float distanceToCamera = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

        if (distanceToCamera <= distanceFadeStart)
        {
            if (!isTransparent)
            {
                SetMaterialTransparent();
            }
            color.a = Mathf.Lerp(finalAlpha, originalAlpha, (distanceToCamera - distanceFadeStop) / (distanceFadeStart - distanceFadeStop));
            meshRenderer.materials[0].color = color;
        }
    }

    private void TimeFadeOut()
    {
        if (!isTransparent)
        {
            SetMaterialTransparent();
        }

        color.a -= timeFadeOutRate * Time.deltaTime;
        meshRenderer.materials[0].color = color;

        if (destroyAfterTimeFadeOut && color.a <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void StartTimeFadeOut(bool destroy = true)
    {
        timeFadeOutTriggered = true;
        destroyAfterTimeFadeOut = destroy;
    }
}
