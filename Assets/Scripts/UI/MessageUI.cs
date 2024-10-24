using TMPro;
using UnityEngine;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] bool capitalize = true;


    private enum Phase
    {
        None,
        FadeIn,
        MessageOn,
        FadeOut,
    }
    private Phase phase = Phase.None;
    private float timer = 0f;
    private float alphaIncrease = 1f;
    private float alphaDecrease = 1f;
    private bool autoHide = true;
    private bool hideAfterFadeIn = false;

    private void Awake()
    {
        messageText.alpha = 0f;
    }

    private void Update()
    {
        FadeIn();
        DecreaseTimer();
        FadeOut();
    }

    private void DecreaseTimer()
    {
        if (phase == Phase.MessageOn)
        {
            if (autoHide)
            {
                if (timer > 0f)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    HideMessage();
                }
            }
            else if (hideAfterFadeIn)
            {
                hideAfterFadeIn = false;
                HideMessage();
            }
        }
    }

    private void FadeIn()
    {
        if (messageText != null && phase == Phase.FadeIn)
        {
            if (messageText.alpha < 1f)
            {
                messageText.alpha += alphaIncrease * Time.deltaTime;
            }
            else
            {
                messageText.alpha = 1f;
                phase = Phase.MessageOn;
            }
        }
    }

    private void FadeOut()
    {
        if (messageText != null && phase == Phase.FadeOut)
        {
            if (messageText.alpha > 0f)
            {
                messageText.alpha -= alphaDecrease * Time.deltaTime;
            }
            else
            {
                messageText.alpha = 0f;
                phase = Phase.None;
            }
        }
    }

    public void ShowMessage(string text, float time = 1f, float alphaIncrease = 1f, float alphaDecrease = 1f, bool autoHide = true)
    {
        if (capitalize) text = text.ToUpper();
        messageText.text = text;
        messageText.alpha = 0f;
        timer = time;
        this.alphaDecrease = alphaDecrease;
        this.alphaIncrease = alphaIncrease;
        this.autoHide = autoHide;
        phase = Phase.FadeIn;
    }

    public void ShowMessage(string text, bool autoHide)
    {
        ShowMessage(text, 1f, 1f, 1f, autoHide);
    }

    public void HideMessage()
    {
        if (phase == Phase.FadeIn)
        {
            hideAfterFadeIn = true;
        }
        else
        {
            phase = Phase.FadeOut;
        }
    }
}
