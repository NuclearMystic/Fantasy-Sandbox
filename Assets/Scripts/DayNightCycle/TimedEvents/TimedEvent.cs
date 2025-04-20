using UnityEngine;

public abstract class TimedEvent : MonoBehaviour
{
    [Tooltip("Time at which the event should trigger, in \"HH:MM\" format")]
    public string triggerTime;
    private TimeManager timeManager;
    private int triggerTotalMinutes;

    protected virtual void Start()
    {
        timeManager = TimeManager.Instance;

        if (timeManager == null)
        {
            Debug.LogError("TimeManager not found in the scene.");
        }
        else
        {
            triggerTotalMinutes = timeManager.GetTotalMinutesOfDay(triggerTime);
        }
    }

    protected virtual void Update()
    {
        if (timeManager != null)
        {
            CheckTime();
        }
    }

    private void CheckTime()
    {
        string currentTime = timeManager.GetFormattedTimeOfDay();
        int currentTotalMinutes = timeManager.GetTotalMinutesOfDay(currentTime);

        if (currentTotalMinutes == triggerTotalMinutes)
        {
            OnTimeTriggered();
        }
    }

    // Method to be overridden by derived classes
    protected abstract void OnTimeTriggered();
}
