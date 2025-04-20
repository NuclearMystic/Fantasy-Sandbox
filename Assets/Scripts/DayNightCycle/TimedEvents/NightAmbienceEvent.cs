using UnityEngine;

public class NightAmbienceEvent : TimedEvent
{
    public AudioSource nightAmbience;
    public float fadeDuration = 2.0f; // Duration of the fade effect in seconds

    protected override void OnTimeTriggered()
    {
        StartCoroutine(AudioFadeUtility.FadeIn(nightAmbience, fadeDuration));
    }
}
