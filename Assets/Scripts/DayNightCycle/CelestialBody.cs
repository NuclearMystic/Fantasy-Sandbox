using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    [Header("Sun & Moon")]
    public Transform sunTransform;
    public Transform moonTransform;
    public Light sunLight;
    public Light moonLight;

    [Header("Time Settings")]
    [Tooltip("When sunset starts (0 to 1, where 0.75 = 6:00 PM)")]
    [Range(0f, 1f)] public float sunsetStart = 0.8f;

    [Tooltip("When sunrise ends (0 to 1, where 0.25 = 6:00 AM)")]
    [Range(0f, 1f)] public float sunriseEnd = 0.25f;

    [Tooltip("When star spawning begins (0–1), e.g. 0.74 = 5:45 PM")]
    [Range(0f, 1f)] public float starSpawnStart = 0.75f;

    [Tooltip("When the stars should start fading out in the morning (0–1, e.g. 0.26 = 6:15 AM)")]
    [Range(0f, 1f)] public float starFadeStart = 0.26f;

    [Tooltip("When all stars should be cleared after fade (0–1, e.g. 0.375 = 9:00 AM)")]
    [Range(0f, 1f)] public float starFadeEnd = 0.375f;

    [Header("Color & Intensity")]
    public Gradient sunColor;
    public Gradient moonColor;
    public Gradient fogColorGradient;
    public AnimationCurve sunIntensity;
    public AnimationCurve moonIntensity;

    [Header("Stars")]
    public ParticleSystem starsParticleSystem;
    public AnimationCurve starBurstRateCurve;
    public float maxBurstsPerSecond = 5f;
    public float maxParticlesPerBurst = 30f;

    private TimeManager timeManager;
    private float blendFactor;
    private float lastStarBurstTime;
    private ParticleSystem.EmissionModule starEmission;

    private enum StarSystemState { Night, SunriseFade, Day }
    private StarSystemState starState = StarSystemState.Day;

    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();

        if (starsParticleSystem != null)
        {
            starEmission = starsParticleSystem.emission;
            starEmission.rateOverTime = 0f;

            var colorOverLifetime = starsParticleSystem.colorOverLifetime;
            colorOverLifetime.enabled = false;
        }
    }

    void Update()
    {
        float time = timeManager.GetCurrentTimeOfDay();

        // SUN & MOON ROTATION
        sunTransform.rotation = Quaternion.Euler(new Vector3((time * 360f) - 90f, 170f, 0f));
        moonTransform.rotation = Quaternion.Euler(new Vector3((time * 360f) - 270f, 170f, 0f));

        // LIGHT COLORS & INTENSITY
        sunLight.color = sunColor.Evaluate(time);
        moonLight.color = moonColor.Evaluate(time);
        sunLight.intensity = sunIntensity.Evaluate(time);
        moonLight.intensity = moonIntensity.Evaluate(time);

        // FOG COLOR
        if (fogColorGradient != null)
        {
            RenderSettings.fogColor = fogColorGradient.Evaluate(time);
        }

        // BLEND FACTOR FOR NIGHT TRANSITIONS
        if (time < sunriseEnd)
        {
            blendFactor = Mathf.Clamp01((sunriseEnd - time) / sunriseEnd);
        }
        else if (time > sunsetStart)
        {
            blendFactor = Mathf.Clamp01((time - sunsetStart) / (1f - sunsetStart));
        }
        else
        {
            blendFactor = 0f;
        }

        // LIGHT ENABLE/DISABLE
        if (time < sunriseEnd || time > sunsetStart)
        {
            sunLight.enabled = false;
            moonLight.enabled = true;
        }
        else
        {
            sunLight.enabled = true;
            moonLight.enabled = false;
        }

        // STAR PARTICLE STATE MANAGEMENT
        if (starsParticleSystem != null)
        {
            var main = starsParticleSystem.main;
            var colorOverLifetime = starsParticleSystem.colorOverLifetime;

            // Night: emit stars, no fading
            if ((time >= starSpawnStart || time <= sunriseEnd) && starState != StarSystemState.Night)
            {
                main.startLifetime = 999f;
                starsParticleSystem.Play();
                colorOverLifetime.enabled = false;
                starState = StarSystemState.Night;
            }
            // Sunrise: begin fading
            else if (time > starFadeStart && time <= starFadeEnd && starState != StarSystemState.SunriseFade)
            {
                main.startLifetime = 8f;
                starsParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                colorOverLifetime.enabled = true;
                starState = StarSystemState.SunriseFade;
            }
            // After fade end: fully clear
            else if (time > starFadeEnd && time < starSpawnStart && starState != StarSystemState.Day)
            {
                starsParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                colorOverLifetime.enabled = false;
                starState = StarSystemState.Day;
            }
        }



        // AMBIENT LIGHT SETTINGS
        RenderSettings.reflectionIntensity = Mathf.Lerp(1.0f, 0.5f, blendFactor);
        RenderSettings.ambientIntensity = Mathf.Lerp(0.5f, 1.0f, 1 - blendFactor);

        Color nightAmbientColor = new Color(0.05f, 0.05f, 0.1f);
        Color dayAmbientColor = new Color(1f, 0.96f, 0.85f);

        RenderSettings.ambientLight = Color.Lerp(nightAmbientColor, dayAmbientColor, 1 - blendFactor);

        // STAR BURST LOGIC
        HandleStarBursts(time);
    }

    private void HandleStarBursts(float time)
    {
        if (starsParticleSystem == null || starState != StarSystemState.Night)
            return;

        float progress = (time >= starSpawnStart)
            ? Mathf.InverseLerp(starSpawnStart, 1f, time)
            : Mathf.InverseLerp(0f, sunriseEnd, time);

        float burstRate = (starBurstRateCurve != null)
            ? starBurstRateCurve.Evaluate(progress) * maxBurstsPerSecond
            : progress * maxBurstsPerSecond;

        float timeBetweenBursts = 1f / Mathf.Max(burstRate, 0.01f);

        if (Time.time - lastStarBurstTime >= timeBetweenBursts)
        {
            int particleCount = Mathf.RoundToInt(progress * maxParticlesPerBurst);
            starsParticleSystem.Emit(particleCount);
            lastStarBurstTime = Time.time;
        }
    }
}
