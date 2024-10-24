using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private float delayBetweenTutorials = 3f;
    [SerializeField] private float initialDelay = 1f;

    public static Tutorial Instance { get; private set; }
    public event EventHandler HideTutorial;
    public event EventHandler<ShowTutorialEventArgs> ShowTutorial;
    public class ShowTutorialEventArgs : EventArgs
    {
        public TutorialPrompt prompt;
    }
    public enum TutorialPrompt
    {
        None,
        Move,
        Aim,
        TimerAdders,
        ProjectileAdders,
    }

    private static List<TutorialPrompt> shownTutorials = new List<TutorialPrompt>();
    private Queue<TutorialPrompt> tutorialQueue = new Queue<TutorialPrompt>();
    private TutorialPrompt currentTutorial = TutorialPrompt.None;
    private float delayTimer;

    private void Awake()
    {
        Instance = this;

        delayTimer = initialDelay;
    }

    private void Start()
    {
        GameManager.Instance.GameStarted += GameManager_GameStarted;

        GameInput.Instance.Move += GameInput_Move;
        GameInput.Instance.Aim += GameInput_Aim;

        PlayerController.Instance.TimeAdded += PlayerController_TimeAdded;
        PlayerController.Instance.LevelChanged += PlayerController_LevelChanged;
        PlayerController.Instance.ProjectileProgressChanged += PlayerController_ProjectileProgressChanged;
    }

    private void Update()
    {
        ManageQueue();
    }

    private void GameManager_GameStarted(object sender, EventArgs e)
    {
        Queue(TutorialPrompt.Move);
        Queue(TutorialPrompt.Aim);
        Queue(TutorialPrompt.TimerAdders);
    }

    private void PlayerController_LevelChanged(object sender, PlayerController.LevelChangedEventArgs e)
    {
        Queue(TutorialPrompt.ProjectileAdders);
    }

    private void GameInput_Move(object sender, System.EventArgs e)
    {
        Hide(TutorialPrompt.Move);
    }

    private void GameInput_Aim(object sender, EventArgs e)
    {
        Hide(TutorialPrompt.Aim);
    }

    private void PlayerController_TimeAdded(object sender, PlayerController.TimerChangedEventArgs e)
    {
        Hide(TutorialPrompt.TimerAdders);
    }

    private void PlayerController_ProjectileProgressChanged(object sender, PlayerController.ProjectileProgressChangedEventArgs e)
    {
        Hide(TutorialPrompt.ProjectileAdders);
    }

    private void ManageQueue()
    {
        if (currentTutorial == TutorialPrompt.None)
        {
            if (delayTimer > 0f)
            {
                delayTimer -= Time.deltaTime;
            }
            else
            {
                if (tutorialQueue.Count > 0)
                {
                    Show(tutorialQueue.Dequeue());
                }
            }
        }
    }

    private void Show(TutorialPrompt tutorialPrompt)
    {
        currentTutorial = tutorialPrompt;
        shownTutorials.Add(tutorialPrompt);
        ShowTutorial?.Invoke(this, new ShowTutorialEventArgs { prompt = tutorialPrompt });
    }

    private void Queue(TutorialPrompt tutorialPrompt)
    {
        if (!shownTutorials.Contains(tutorialPrompt))
        {
            tutorialQueue.Enqueue(tutorialPrompt);
        }
    }

    private void Hide(TutorialPrompt tutorialPrompt)
    {
        if (tutorialPrompt == currentTutorial)
        {
            currentTutorial = TutorialPrompt.None;
            delayTimer = delayBetweenTutorials;
            HideTutorial?.Invoke(this, EventArgs.Empty);
        }
    }
}
