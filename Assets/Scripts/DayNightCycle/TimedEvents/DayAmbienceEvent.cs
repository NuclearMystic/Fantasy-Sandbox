using UnityEngine;

public class DayAmbienceEvent : TimedEvent
{
    public AudioSource dayAmbience;
    public float fadeDuration = 2.0f; // Duration of the fade effect in seconds

    protected override void OnTimeTriggered()
    {
        StartCoroutine(AudioFadeUtility.FadeIn(dayAmbience, fadeDuration));
    }
}
