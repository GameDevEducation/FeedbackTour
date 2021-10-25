// Based heavily on CinemachineIndependentImpulseListener

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenshakeToHapticBridge : MonoBehaviour
{
    /// <summary>
    /// Impulse events on channels not included in the mask will be ignored.
    /// </summary>
    [Tooltip("Impulse events on channels not included in the mask will be ignored.")]
    [CinemachineImpulseChannelProperty]
    [SerializeField] int m_ChannelMask;

    /// <summary>
    /// Gain to apply to the Impulse signal.
    /// </summary>
    [Tooltip("Gain to apply to the Impulse signal.  1 is normal strength.  Setting this to 0 completely mutes the signal.")]
    [SerializeField] float m_Gain;

    /// <summary>
    /// Enable this to perform distance calculation in 2D (ignore Z).
    /// </summary>
    [Tooltip("Enable this to perform distance calculation in 2D (ignore Z)")]
    [SerializeField] bool m_Use2DDistance;

    [SerializeField] float MaxDelta = 0.5f;

    Vector3 PreviousTranslation = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 impulseTranslation;
        Quaternion impulseRotation;

        // read the impulse
        if (CinemachineImpulseManager.Instance.GetImpulseAt(transform.position,
                                                            m_Use2DDistance,
                                                            m_ChannelMask,
                                                            out impulseTranslation,
                                                            out impulseRotation))
        {
            impulseTranslation *= m_Gain;

            // convert the impulse translation delta directly to intensity based on magnitude
            float intensity = Mathf.Clamp01((impulseTranslation - PreviousTranslation).magnitude / MaxDelta);

            // use the intensity for both low and high speed motors
            HapticManager.SetAmbientSpeeds(intensity, intensity);

            PreviousTranslation = impulseTranslation;
        }
        else
            HapticManager.SetAmbientSpeeds(0f, 0f);
    }
}
