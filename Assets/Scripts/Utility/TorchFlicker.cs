using UnityEngine;

[RequireComponent(typeof(Light))]
public class TorchFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    public float minIntensity = 0.7f;
    public float maxIntensity = 1.3f;
    public float flickerSpeed = 0.1f;

    private Light torchLight;
    private float timer;

    private void Start()
    {
        torchLight = GetComponent<Light>();
        timer = flickerSpeed;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            torchLight.intensity = Random.Range(minIntensity, maxIntensity);
            timer = Random.Range(flickerSpeed * 0.5f, flickerSpeed * 1.5f);
        }
    }
}
