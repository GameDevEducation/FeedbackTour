using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAudioChannel
{
    SFX
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; } = null;

    [SerializeField] GameObject SFXEmitter_Template;
    [SerializeField] Transform SFXEmitter_Parent;
    [SerializeField] int NumSFXEmitters = 10;
    List<SoundEmitter> SFXEmitters = new List<SoundEmitter>();

    public static SoundEmitter PlaySoundEffect(SoundEffect effect, Vector3 location)
    {
        return Instance.PlaySoundEffect_Internal(effect, location);
    }

    public static SoundEmitter PlaySoundEffect(SoundEffect effect, 
                                               System.Func<Vector3> getLocationFn,
                                               System.Func<float> getIntensityFn)
    {
        return Instance.PlaySoundEffect_Internal(effect, getLocationFn, getIntensityFn);
    }

    public static SoundEmitter GetEmitterForChannel(EAudioChannel channel)
    {
        if (channel == EAudioChannel.SFX)
            return GetFreeEmitter(Instance.SFXEmitters);

        return null;
    }

    static SoundEmitter GetFreeEmitter(List<SoundEmitter> emitters)
    {
        // find the first available emitter
        for (int index = 0; index < emitters.Count; ++index)
        {
            if (!emitters[index].IsPlaying)
                return emitters[index];
        }

        throw new System.Exception("Sound emitters saturated!");
    }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogErrorFormat("AudioManager already exists. Destroying newer GO {0}", gameObject.name);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        // setup the SFX emitter bank
        for (int index = 0; index < NumSFXEmitters; ++index)
        {
            var sfxEmitterGO = Instantiate(SFXEmitter_Template);
            sfxEmitterGO.transform.parent = SFXEmitter_Parent;
            sfxEmitterGO.name = "SFX Emitter " + index;
            sfxEmitterGO.SetActive(true);

            SFXEmitters.Add(sfxEmitterGO.GetComponent<SoundEmitter>());
        }
    }

    SoundEmitter PlaySoundEffect_Internal(SoundEffect effect, Vector3 location)
    {
        return effect.Play(location);
    }

    SoundEmitter PlaySoundEffect_Internal(SoundEffect effect,
                                          System.Func<Vector3> getLocationFn,
                                          System.Func<float> getIntensityFn)
    {
        return effect.Play(getLocationFn, getIntensityFn);
    }    
}
