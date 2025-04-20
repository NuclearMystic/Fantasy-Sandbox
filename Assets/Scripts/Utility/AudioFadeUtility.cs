using System.Collections;
using UnityEngine;

public static class AudioFadeUtility
{
    public static IEnumerator FadeIn(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = 0.8f; 
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < startVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / fadeDuration;

            yield return null;
        }

        audioSource.volume = startVolume;
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
