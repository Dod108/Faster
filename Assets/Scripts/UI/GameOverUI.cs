using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup gameOverVisuals;
    [SerializeField] private float fadeInSpeed = 1f;
    [SerializeField] private float fadeOutSpeed = 1f;
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private bool fadeIn = false;
    private bool fadeOut = false;

    private void Awake()
    {
        restartButton.onClick.AddListener(RestartClick);
    }

    private void Start()
    {
        GameManager.Instance.GameOver += Instance_GameOver;

        if (gameOverVisuals != null)
        {
            gameOverVisuals.alpha = 0;
            gameOverVisuals.interactable = false;
            gameOverVisuals.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (gameOverVisuals != null)
        {
            if (fadeIn)
            {
                if (gameOverVisuals.alpha >= 1f)
                {
                    gameOverVisuals.alpha = 1f;
                    gameOverVisuals.interactable = true;
                    gameOverVisuals.blocksRaycasts = true;
                    fadeIn = false;
                }
                else
                {
                    gameOverVisuals.alpha += fadeInSpeed * Time.deltaTime;
                }
            }
            else if (fadeOut)
            {
                if (gameOverVisuals.alpha <= 0f)
                {
                    gameOverVisuals.alpha = 0f;
                    gameOverVisuals.interactable = false;
                    gameOverVisuals.blocksRaycasts = false;
                    fadeOut = false;
                }
                else
                {
                    gameOverVisuals.alpha -= fadeOutSpeed * Time.deltaTime;
                }
            }

            if (gameOverVisuals.alpha > 0f)
            {
                if (scoreText != null)
                {
                    scoreText.text = "Score: " + PlayerController.Instance.GetScore().ToString("0", CultureInfo.InvariantCulture);
                }

                if (highScoreText != null)
                {
                    highScoreText.text = "High score: " + PlayerController.Instance.GetHighScore().ToString("0", CultureInfo.InvariantCulture);
                }
            }
        }
    }

    private void Instance_GameOver(object sender, System.EventArgs e)
    {
        if (gameOverVisuals != null)
        {
            fadeIn = true;
            Cursor.visible = true;
        }
    }

    private void RestartClick()
    {
        Cursor.visible = false;
        GameManager.Instance.RestartGame();
    }
}
