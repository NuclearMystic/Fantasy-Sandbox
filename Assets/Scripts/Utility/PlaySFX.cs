using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    public AudioSource sfxToPlay;

    public void TriggerSFX()
    {
        sfxToPlay.Play();
    }
}
