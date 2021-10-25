using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsImpulseListener : MonoBehaviour
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

    [SerializeField] ForceMode ImpulseMode = ForceMode.Impulse;

    protected Rigidbody ListenerRB;

    void Awake()
    {
        ListenerRB = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
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
            impulseRotation = Quaternion.SlerpUnclamped(Quaternion.identity, impulseRotation, -m_Gain);

            ListenerRB.AddForce(impulseTranslation, ImpulseMode);
        }
    }
}
