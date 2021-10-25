// Based heavily on CinemachineIndependentImpulseListener

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance { get; private set; } = null;

    List<HapticEffectSO> ActiveEffects = new List<HapticEffectSO>();
    float Ambient_LowSpeed = 0f;
    float Ambient_HighSpeed = 0f;

    public static HapticEffectSO PlayEffect(HapticEffectSO effect, Vector3 location)
    {
        return Instance.PlayEffect_Internal(effect, location);
    }

    public static void StopEffect(HapticEffectSO effect)
    {
        Instance.StopEffect_Internal(effect);
    }

    public static void SetAmbientSpeeds(float lowSpeedMotor, float highSpeedMotor)
    {
        Instance.SetAmbientSpeeds_Internal(lowSpeedMotor, highSpeedMotor);
    }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Attempting to create second HapticManager on " + gameObject.name);

            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float lowSpeedMotor = Ambient_LowSpeed;
        float highSpeedMotor = Ambient_HighSpeed;

        for(int index = 0; index < ActiveEffects.Count; ++index)
        {
            var effect = ActiveEffects[index];

            // tick the effect and cleanup if finished
            float lowSpeedComponent = 0f;
            float highSpeedComponent = 0f;
            if (effect.Tick(Camera.main.transform.position, out lowSpeedComponent, out highSpeedComponent))
            {
                ActiveEffects.RemoveAt(index);
                --index;
            }

            // update the new speeds, constrain to 0 to 1
            lowSpeedMotor = Mathf.Clamp01(lowSpeedComponent + lowSpeedMotor);
            highSpeedMotor = Mathf.Clamp01(highSpeedComponent + highSpeedMotor);
        }

        // update the motors
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(lowSpeedMotor, highSpeedMotor);
    }

    void OnDestroy()
    {
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(0f, 0f);
    }

    HapticEffectSO PlayEffect_Internal(HapticEffectSO effect, Vector3 location)
    {
        // setup the effect
        var activeEffect = ScriptableObject.Instantiate(effect);
        activeEffect.Initialise(location);

        ActiveEffects.Add(activeEffect);

        return activeEffect;
    }

    void StopEffect_Internal(HapticEffectSO effect)
    {
        ActiveEffects.Remove(effect);
    }

    void SetAmbientSpeeds_Internal(float lowSpeedMotor, float highSpeedMotor)
    {
        Ambient_LowSpeed = lowSpeedMotor;
        Ambient_HighSpeed = highSpeedMotor;
    }
}
