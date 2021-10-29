using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    [SerializeField] AudioSource Source;
    [SerializeField] bool ManageLifetime = true;

    float InitialVolume;
    System.Func<Vector3> GetLocationFn;
    System.Func<float> GetIntensityFn;

    public bool IsPlaying => Source.isPlaying;

    void Update()
    {
        if (Source.isPlaying)
        {
            if (GetLocationFn != null)
                transform.position = GetLocationFn();
            if (GetIntensityFn != null)
                Source.volume = InitialVolume * GetIntensityFn();
        } // not playing and not looped?
        else if (!Source.loop && ManageLifetime)
        {
            Destroy(gameObject);
        }
    }

    public void Play(AudioClip clip, bool IsLooped, float DefaultVolume, 
                     float pitchAdjustment, float pitchVariation)
    {
        // configure the audio source
        Source.clip = clip;
        Source.loop = IsLooped;
        InitialVolume = Source.volume = DefaultVolume;
        Source.pitch = 1f + pitchAdjustment + (pitchVariation * Random.Range(-1f, 1f));

        GetLocationFn = null;
        GetIntensityFn = null;

        Source.Play();
    }

    public void Play(AudioClip clip, bool IsLooped, float DefaultVolume,
                     float pitchAdjustment, float pitchVariation,
                     System.Func<Vector3> getLocationFn, System.Func<float> getIntensityFn)
    {
        // configure the audio source
        Source.clip = clip;
        Source.loop = IsLooped;
        InitialVolume = DefaultVolume;
        Source.volume = InitialVolume * getIntensityFn();
        Source.pitch = 1f + pitchAdjustment + (pitchVariation * Random.Range(-1f, 1f));

        GetLocationFn  = getLocationFn;
        GetIntensityFn = getIntensityFn;

        Source.Play();
    }
}
