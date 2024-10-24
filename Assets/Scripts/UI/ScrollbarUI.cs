using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScrollbarUI : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private float conversionMultiplier = 100f;
    [SerializeField] private string format = "0";

    private void Start()
    {
        scrollbar.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(float value)
    {
        valueText.text = (value * conversionMultiplier).ToString(format, CultureInfo.InvariantCulture);
    }

    public Scrollbar GetScrollbar()
    {
        return scrollbar;
    }

    public void SetValue(float value)
    {
        scrollbar.value = value;
        OnValueChanged(value);
    }
}
