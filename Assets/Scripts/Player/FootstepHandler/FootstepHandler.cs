using System.Collections.Generic;
using UnityEngine;

public class FootstepHandler : MonoBehaviour
{
    [Header("Foot Transforms")]
    public Transform leftFoot;
    public Transform rightFoot;

    [Header("Audio Sources")]
    public AudioSource leftFootAudio;
    public AudioSource rightFootAudio;

    [Header("Footstep Settings")]
    public float rayDistance = 0.3f;
    public LayerMask groundLayer;

    [Tooltip("List of footstep audio sets by surface type")]
    public List<FootstepAudioData> footstepAudioSets;

    private Dictionary<SurfaceType, AudioClip[]> footstepClipsMap;

    public Animator animator;
    public string[] allowedStates = { "Walk", "Run" };

    private void Awake()
    {
        footstepClipsMap = new Dictionary<SurfaceType, AudioClip[]>();
        foreach (var set in footstepAudioSets)
        {
            footstepClipsMap[set.surfaceType] = set.footstepClips;
        }
    }

    public void PlayFootstep(int footIndex)
    {
        Transform foot = footIndex == 0 ? leftFoot : rightFoot;
        AudioSource source = footIndex == 0 ? leftFootAudio : rightFootAudio;

        SurfaceType surface = DetectSurface(foot);
        if (!footstepClipsMap.TryGetValue(surface, out AudioClip[] clips))
        {
            clips = footstepClipsMap[SurfaceType.Default];
        }

        if (clips != null && clips.Length > 0)
        {
            AudioClip chosenClip = clips[Random.Range(0, clips.Length)];
            source.PlayOneShot(chosenClip);
        }
    }

    private SurfaceType DetectSurface(Transform foot)
    {
        Ray ray = new Ray(foot.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundLayer))
        {
            // First try mesh/object-based detection
            var surface = hit.collider.GetComponent<SurfaceIdentifier>();
            if (surface != null)
                return surface.surfaceType;

            // Try terrain-based detection ouch my brain ouch ouch ouch ouch ouch ocuh
            Terrain terrain = hit.collider.GetComponent<Terrain>();
            if (terrain != null)
            {
                TerrainSurfaceIdentifier terrainSurface = terrain.GetComponent<TerrainSurfaceIdentifier>();
                if (terrainSurface != null)
                {
                    return terrainSurface.GetDominantSurface(hit.point);
                }
            }
        }

        return SurfaceType.Default;
    }

}
