using TMPro;
using UnityEngine;

public class DayCounterUI : MonoBehaviour
{
    [Header("References")]
    public DayNightManager dayNightManager;
    public TextMeshProUGUI dayCounterText;

    private int lastDisplayedDay = -1;

    private void Start()
    {
        if (dayNightManager == null)
        {
            dayNightManager = FindFirstObjectByType<DayNightManager>();
        }

        if (dayCounterText == null)
        {
            dayCounterText = GetComponent<TextMeshProUGUI>();
        }

        UpdateDayText();
    }

    private void Update()
    {
        if (dayNightManager == null || dayCounterText == null)
        {
            return;
        }

        if (dayNightManager.currentDay != lastDisplayedDay)
        {
            UpdateDayText();
        }
    }

    private void UpdateDayText()
    {
        if (dayNightManager == null || dayCounterText == null)
        {
            return;
        }

        lastDisplayedDay = dayNightManager.currentDay;
        dayCounterText.text = "Day " + lastDisplayedDay;
    }
}