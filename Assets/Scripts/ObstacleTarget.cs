using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTarget : MonoBehaviour
{
    [SerializeField] private Obstacle obstacle;
    [SerializeField] private Color flashColor;
    [SerializeField] private float flashTime = 0.1f;
    [SerializeField] private float flashColorBlend = 0.5f;

    private MeshRenderer meshRenderer;
    private Color originalColor;
    private Color flashColorBlended;
    private float timer = 0f;
    private bool flash = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        originalColor = meshRenderer.material.color;
        flashColorBlended = Color.Lerp(originalColor, flashColor, flashColorBlend);
    }

    private void Update()
    {
        if (flash)
        {
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
            }
            else if (meshRenderer != null)
            {
                meshRenderer.material.color = originalColor;
                flash = false;
            }
        }
    }

    public void Flash()
    {
        meshRenderer.material.color = flashColorBlended;
        timer = flashTime;
        flash = true;
    }

    public Obstacle GetObstacle() 
    { 
        return obstacle; 
    }
}
