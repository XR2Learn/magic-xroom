using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image fill;

    private double _maxValue;
    private double _currentValue;

    public double MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;
            fill.fillAmount = (float)(_currentValue / _maxValue);
        }
    }

    public double CurrentValue
    {
        get => _currentValue;
        set
        {
            _currentValue = value;
            fill.fillAmount = (float)(_currentValue / _maxValue);
            text.text = $"{_currentValue:F0}";
        }
    }
}