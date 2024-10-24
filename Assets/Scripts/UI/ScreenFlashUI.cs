using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlashUI : MonoBehaviour
{
    [SerializeField] private float fadeInSpeed = 20f;
    [SerializeField] private float fadeOutSpeed = 1f;
    [SerializeField] private float maxAlpha = 0.3f;
    [SerializeField] private Image flashImage;

    private bool fadeIn = false;
    private bool fadeOut = false;
    private float alpha = 0f;
    private Color color;

    private void Start()
    {
        color = flashImage.color;
    }

    private void Update()
    {
        if (fadeIn)
        {
            if (alpha >= maxAlpha)
            {
                alpha = maxAlpha;
                fadeIn = false;
                fadeOut = true;
            }
            else
            {
                alpha += fadeInSpeed * Time.deltaTime;
            }
        }
        else if (fadeOut)
        {
            if (alpha <= 0f)
            {
                alpha = 0f;
                fadeOut = false;
            }
            else
            {
                alpha -= fadeOutSpeed * Time.deltaTime;
            }
        }

        color.a = alpha;
        flashImage.color = color;
    }

    public void Flash()
    {
        fadeIn = true;
    }
}
