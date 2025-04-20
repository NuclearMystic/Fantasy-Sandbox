using UnityEngine;

public class AmbienceStopEvent : TimedEvent
{
    public AudioSource ambience;
    public float fadeDuration = 2.0f; // Duration of the fade effect in seconds

    protected override void OnTimeTriggered()
    {
        StartCoroutine(AudioFadeUtility.FadeOut(ambience, fadeDuration));
    }
}
