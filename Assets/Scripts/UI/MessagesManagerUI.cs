using UnityEngine;

public class MessagesManagerUI : MonoBehaviour
{
    [SerializeField] private MessageUI message;
    [SerializeField] private MessageUI tutorialMessage;

    private void Start()
    {
        PlayerController.Instance.SpeedIncreased += PlayerController_SpeedIncreased;
        PlayerController.Instance.TimerStarted += PlayerController_TimerStarted;
        PlayerController.Instance.ProjectileProgressChanged += PlayerController_ProjectileProgressChanged;

        Tutorial.Instance.ShowTutorial += Tutorial_ShowTutorial;
        Tutorial.Instance.HideTutorial += Tutorial_HideTutorial;
    }

    private void Tutorial_ShowTutorial(object sender, Tutorial.ShowTutorialEventArgs e)
    {
        string messageText = "";

        switch (e.prompt)
        {
            case Tutorial.TutorialPrompt.Move:
                messageText = "WASD to move";
                break;
            case Tutorial.TutorialPrompt.Aim:
                messageText = "move mouse to aim";
                break;
            case Tutorial.TutorialPrompt.TimerAdders:
                messageText = "collect batteries to increase timer";
                break;
            case Tutorial.TutorialPrompt.ProjectileAdders:
                messageText = "collect orbs to increase projectile number";
                break;
        }

        tutorialMessage.ShowMessage(messageText, false);
    }

    private void Tutorial_HideTutorial(object sender, System.EventArgs e)
    {
        tutorialMessage.HideMessage();
    }

    private void PlayerController_ProjectileProgressChanged(object sender, PlayerController.ProjectileProgressChangedEventArgs e)
    {
        if (e.numberIncreased)
        {
            message.ShowMessage("Projectile number increased", 1f, 2f, 1f);
        }
    }

    private void PlayerController_TimerStarted(object sender, System.EventArgs e)
    {
        message.ShowMessage("Timer started");
    }

    private void PlayerController_SpeedIncreased(object sender, System.EventArgs e)
    {
        message.ShowMessage("Speed increased");
    }
}
