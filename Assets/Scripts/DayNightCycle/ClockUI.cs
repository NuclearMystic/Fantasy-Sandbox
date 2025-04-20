using UnityEngine;
using TMPro;
public class ClockUI : MonoBehaviour
{
    public TMP_Text clockText;
    [Tooltip("Option to switch between 24-hour and 12-hour formats")]
    public bool use24HourFormat = true;

    private TimeManager timeManager;

    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
    }

    void Update()
    {
        if (timeManager != null)
        {
            // Get current time of day
            float timeOfDay = timeManager.GetCurrentTimeOfDay();

            // Convert time of day (0 to 1) to hours and minutes
            int totalMinutes = Mathf.FloorToInt(timeOfDay * 24 * 60);
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;

            string timeString;

            if (use24HourFormat)
            {
                // Format time as HH:MM in 24-hour format
                timeString = string.Format("{0:00}:{1:00}", hours, minutes);
            }
            else
            {
                // Format time as HH:MM AM/PM in 12-hour format
                string period = hours >= 12 ? "PM" : "AM";
                hours = hours % 12;
                if (hours == 0) hours = 12;
                timeString = string.Format("{0:00}:{1:00} {2}", hours, minutes, period);
            }

            clockText.text = timeString;
        }
    }
}
