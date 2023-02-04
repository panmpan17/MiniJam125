using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextDisplay : MonoBehaviour
{
    [SerializeField]
    private string prefix;
    [SerializeField]
    private string suffix;
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;

    public void ChangeByInt(int intValue)
    {
        textMeshProUGUI.text = prefix + intValue.ToString() + suffix;
    }

    public void DisplayFloatAsPercentage(float floatValue)
    {
        int percentage = Mathf.RoundToInt(floatValue * 100);
        textMeshProUGUI.text = prefix + percentage.ToString() + suffix;
    }
}
