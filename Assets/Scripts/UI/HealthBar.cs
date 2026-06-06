using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image fillbar;
    public TextMeshProUGUI valueText;

    /// <summary>
    /// Updates the health bar fill amount and text value.
    /// </summary>
    /// <param name="currentValue">Current health value</param>
    /// <param name="maxValue">Maximum health value</param>
    public void updateBar(float currentValue, float maxValue)
    {
        if (fillbar != null)
        {
            if (maxValue > 0f)
            {
                fillbar.fillAmount = currentValue / maxValue;
            }
            else
            {
                fillbar.fillAmount = 0f;
            }
        }

        if (valueText != null)
        {
            valueText.text = currentValue.ToString() + " / " + maxValue.ToString();
        }
    }
}
