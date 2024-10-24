using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Globalization;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI timerChangeText;
    [SerializeField] private float decimalFormatThreshold = 5f;
    [SerializeField] private ScreenFlashUI timeAddedFlash;
    [SerializeField] private ScreenFlashUI timeRemovedFlash;
    [SerializeField] private Color timeAddedColor;
    [SerializeField] private Color timeRemovedColor;
    [SerializeField] private float timeChangedAlphaDecrease = 1.0f;
    [SerializeField] private Image timerWarningImage;
    [SerializeField] private float timerWarningThreshold = 5f;
    [SerializeField] private float timerWarningAlphaMax = 0.7f;

    private float timeAdded = 0f;
    private float timeRemoved = 0f;

    private void Start()
    {
        PlayerController.Instance.TimeAdded += PlayerController_TimeAdded;
        PlayerController.Instance.TimeRemoved += PlayerController_TimeRemoved;

        timerChangeText.alpha = 0f;
    }

    private void PlayerController_TimeAdded(object sender, PlayerController.TimerChangedEventArgs e)
    {
        timeAddedFlash.Flash();

        timeAdded += e.time;
        timeRemoved = 0f;
        timerChangeText.text = "+" + timeAdded.ToString("0", CultureInfo.InvariantCulture);
        timerChangeText.color = timeAddedColor;
        timerChangeText.alpha = 1f;
    }

    private void PlayerController_TimeRemoved(object sender, PlayerController.TimerChangedEventArgs e)
    {
        timeRemovedFlash.Flash();

        timeAdded = 0f;
        timeRemoved += e.time;
        timerChangeText.text = "-" + timeRemoved.ToString("0", CultureInfo.InvariantCulture);
        timerChangeText.color = timeRemovedColor;
        timerChangeText.alpha = 1f;
    }

    private void Update()
    {
        float timerValue = PlayerController.Instance.GetTimer();

        if (timerText != null)
        {
            string format = "0";
            if (timerValue < decimalFormatThreshold) 
            {
                format = "0.0";
            }
            timerText.text = timerValue.ToString(format, CultureInfo.InvariantCulture);
        }

        if (timerChangeText != null)
        {
            if (timerChangeText.alpha > 0f)
            {
                timerChangeText.alpha -= timeChangedAlphaDecrease * Time.deltaTime;
            }
            else
            {
                timeAdded = 0f;
                timeRemoved = 0f;
                timerChangeText.alpha = 0f;
            }
        }

        if (timerWarningImage != null)
        {
            Color color = timerWarningImage.color;
            color.a = Mathf.Lerp(0f, timerWarningAlphaMax, (timerWarningThreshold - timerValue) / timerWarningThreshold);
            timerWarningImage.color = color;
        }
    }
}
