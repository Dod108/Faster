using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Globalization;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Update()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + PlayerController.Instance.GetScore().ToString("0", CultureInfo.InvariantCulture);
        }
    }
}
