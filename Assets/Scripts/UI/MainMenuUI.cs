using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        Cursor.visible = true;

        startButton.onClick.AddListener(StartClick);
        quitButton.onClick.AddListener(QuitClick);
    }

    private void StartClick()
    {
        Loader.Load(Loader.Scene.GameScene);
        Cursor.visible = false;
    }

    private void QuitClick()
    {
        Application.Quit();
    }
}
