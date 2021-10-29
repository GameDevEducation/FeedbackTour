using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Sound Effect", fileName = "SoundEffect")]
public class SoundEffect : ScriptableObject
{
    [SerializeField] GameObject EmitterTemplate;
    [SerializeField] EAudioChannel Channel;

    [SerializeField] AudioClip[] Clips;
    [SerializeField] bool IsLooped;
    [SerializeField] [Range(0f, 1f)]  float DefaultVolume = 1f;

    [SerializeField] [Range(-1f, 1f)] float PitchAdjustment;
    [SerializeField] [Range(0f, 1f)] float PitchVariation;

    SoundEmitter PrepareToPlay(Vector3 location, out AudioClip clip)
    {
        SoundEmitter emitterLogic = null;
        if (EmitterTemplate != null)
        {
            var emitterGO = Instantiate(EmitterTemplate, location, Quaternion.identity);
            emitterGO.name = name;

            emitterLogic = emitterGO.GetComponent<SoundEmitter>();
        }
        else
        {
            emitterLogic = AudioManager.GetEmitterForChannel(Channel);
        }

        clip = Clips[Random.Range(0, Clips.Length)];

        return emitterLogic;
    }

    public SoundEmitter Play(Vector3 location)
    {
        AudioClip clip;
        var emitterLogic = PrepareToPlay(location, out clip);

        emitterLogic.Play(clip, IsLooped, DefaultVolume, PitchAdjustment, PitchVariation);

        return emitterLogic;
    }

    public SoundEmitter Play(System.Func<Vector3> getLocationFn, System.Func<float> getIntensityFn)
    {
        AudioClip clip;
        var emitterLogic = PrepareToPlay(getLocationFn(), out clip);

        emitterLogic.Play(clip, IsLooped, DefaultVolume, PitchAdjustment, PitchVariation,
                          getLocationFn, getIntensityFn);

        return emitterLogic;
    }
}
