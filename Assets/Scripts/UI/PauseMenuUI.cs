using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseMenuVisuals;
    [SerializeField] private CanvasGroup mainCanvas;
    [SerializeField] private CanvasGroup optionsCanvas;
    [SerializeField] private float fadeInSpeed = 1f;
    [SerializeField] private float fadeOutSpeed = 1f;
    [SerializeField] private Button unpauseButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button backButton;

    private bool fadeIn = false;
    private bool fadeOut = false;
    private enum MenuLevel
    {
        Main,
        Options,
    }
    private MenuLevel level = MenuLevel.Main;

    private void Awake()
    {
        unpauseButton.onClick.AddListener(UnpauseClick);
        mainMenuButton.onClick.AddListener(MainMenuClick);
        optionsButton.onClick.AddListener(OptionsClick);
        backButton.onClick.AddListener(BackClick);
    }

    private void Start()
    {
        GameManager.Instance.GamePaused += GameManager_GamePaused;
        GameManager.Instance.GameUnpaused += GameManager_GameUnpaused;

        if (pauseMenuVisuals != null)
        {
            pauseMenuVisuals.alpha = 0f;
            pauseMenuVisuals.interactable = false;
            pauseMenuVisuals.blocksRaycasts = false;
        }
    }

    private void GameManager_GamePaused(object sender, System.EventArgs e)
    {
        if (pauseMenuVisuals != null)
        {
            fadeIn = true;
            Cursor.visible = true;
            ChangeLevel(MenuLevel.Main);
        }
    }

    private void GameManager_GameUnpaused(object sender, System.EventArgs e)
    {
        if (pauseMenuVisuals != null)
        {
            fadeOut = true;
            pauseMenuVisuals.interactable = false;
            pauseMenuVisuals.blocksRaycasts = false;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        ManageFade();
    }

    private void ManageFade()
    {
        if (pauseMenuVisuals != null)
        {
            if (fadeIn)
            {
                if (pauseMenuVisuals.alpha >= 1f)
                {
                    pauseMenuVisuals.alpha = 1f;
                    pauseMenuVisuals.interactable = true;
                    pauseMenuVisuals.blocksRaycasts = true;
                    fadeIn = false;
                }
                else
                {
                    pauseMenuVisuals.alpha += fadeInSpeed * Time.unscaledDeltaTime;
                }
            }
            else if (fadeOut)
            {
                if (pauseMenuVisuals.alpha <= 0f)
                {
                    pauseMenuVisuals.alpha = 0f;
                    pauseMenuVisuals.interactable = false;
                    pauseMenuVisuals.blocksRaycasts = false;
                    fadeOut = false;
                }
                else
                {
                    pauseMenuVisuals.alpha -= fadeOutSpeed * Time.unscaledDeltaTime;
                }
            }
        }
    }

    private void ChangeLevel(MenuLevel level)
    {
        this.level = level;

        mainCanvas.alpha = 0f;
        mainCanvas.interactable = false;
        mainCanvas.blocksRaycasts = false;

        optionsCanvas.alpha = 0f;
        optionsCanvas.interactable = false;
        optionsCanvas.blocksRaycasts = false;

        switch (level)
        {
            case MenuLevel.Main:
                mainCanvas.alpha = 1f;
                mainCanvas.interactable = true;
                mainCanvas.blocksRaycasts = true;
                break;

            case MenuLevel.Options:
                optionsCanvas.alpha = 1f;
                optionsCanvas.interactable = true;
                optionsCanvas.blocksRaycasts = true;
                break;
        }
    }

    private void UnpauseClick()
    {
        GameManager.Instance.TogglePause();
    }

    private void MainMenuClick()
    {
        GameManager.Instance.TogglePause();
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    private void OptionsClick()
    {
        ChangeLevel(MenuLevel.Options);
    }

    private void BackClick()
    {
        ChangeLevel(MenuLevel.Main);
    }

}
