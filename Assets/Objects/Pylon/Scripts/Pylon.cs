using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pylon : MonoBehaviour
{
    [SerializeField] Transform MovingMass;
    [SerializeField] float RiseTime = 5f;
    [SerializeField] AnimationCurve RiseCurve;
    [SerializeField] float FallTime = 0.5f;
    [SerializeField] AnimationCurve FallCurve;
    [SerializeField] float MaxHeight = 5f;

    [SerializeField] bool UseHaptics = true;
    [SerializeField] HapticEffectSO ImpactEffect;

    [SerializeField] UnityEvent OnImpact = new UnityEvent();

    [SerializeField] SoundEffect ImpactSound;
    [SerializeField] SoundEffect RisingSound;
    [SerializeField] SoundEffect FallingSound;

    [SerializeField] float ImpactDamage = 20f;
    [SerializeField] SphereCollider DamageTrigger;

    SoundEmitter ActiveSoundEmitter = null;
    List<DamageReceiver> DamageableObjects = new List<DamageReceiver>();
    float MaxDamageRange;

    float CurrentProgress = 0f;
    bool IsRising = true;
    Vector3 StartPos;
    Vector3 EndPos;

    // Start is called before the first frame update
    void Start()
    {
        StartPos = MovingMass.localPosition;
        EndPos = StartPos + Vector3.up * MaxHeight;

        CurrentProgress = Random.Range(0f, 1f);

        MaxDamageRange = Mathf.Sqrt((DamageTrigger.bounds.extents.x * DamageTrigger.bounds.extents.x) +
                                    (DamageTrigger.bounds.extents.z * DamageTrigger.bounds.extents.z));

        ActiveSoundEmitter = AudioManager.PlaySoundEffect(RisingSound, GetSoundLocation, GetSoundIntensity);
    }

    Vector3 GetSoundLocation()
    {
        return MovingMass.position;
    }

    float GetSoundIntensity()
    {
        if (IsRising)
            return RiseCurve.Evaluate(CurrentProgress);
        else
            return 1f - FallCurve.Evaluate(CurrentProgress);
    }

    // Update is called once per frame
    void Update()
    {
        // update the progress
        CurrentProgress += Time.deltaTime / (IsRising ? RiseTime : FallTime);

        // work out the new position
        float lerpFactor = IsRising ? RiseCurve.Evaluate(CurrentProgress) : FallCurve.Evaluate(CurrentProgress);
        MovingMass.localPosition = Vector3.Lerp(StartPos, EndPos, lerpFactor);

        // finished rising or falling?
        if (CurrentProgress >= 1f)
        {
            // time to play impact effect?
            if (!IsRising)
            {
                if (UseHaptics)
                    HapticManager.PlayEffect(ImpactEffect, MovingMass.transform.position);
                OnImpact.Invoke();

                AudioManager.PlaySoundEffect(ImpactSound, MovingMass.position);

                // notify any damageable objects
                for(int index = 0; index < DamageableObjects.Count; ++index)
                {
                    // clear any null entries
                    if (DamageableObjects[index] == null)
                    {
                        DamageableObjects.RemoveAt(index);
                        --index;
                        continue;
                    }

                    Vector3 vecToTarget = DamageableObjects[index].transform.position - transform.position;
                    float dist2D = Mathf.Sqrt(vecToTarget.x * vecToTarget.x + vecToTarget.z * vecToTarget.z);

                    float damage = ImpactDamage * (1f - Mathf.Clamp01(dist2D / MaxDamageRange));

                    DamageableObjects[index].TakeDamage(damage);
                }
            }

            // reset for the next move
            IsRising = !IsRising;
            CurrentProgress = 0f;

            // cleanup the previous emitter
            Destroy(ActiveSoundEmitter.gameObject);

            // start the new sound
            if (IsRising)
                ActiveSoundEmitter = AudioManager.PlaySoundEffect(RisingSound, GetSoundLocation, GetSoundIntensity);
            else
                ActiveSoundEmitter = AudioManager.PlaySoundEffect(FallingSound, GetSoundLocation, GetSoundIntensity);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        DamageReceiver receiver = null;
        if (other.gameObject.TryGetComponent<DamageReceiver>(out receiver))
            DamageableObjects.Add(receiver);
    }

    void OnTriggerExit(Collider other)
    {
        DamageReceiver receiver = null;
        if (other.gameObject.TryGetComponent<DamageReceiver>(out receiver))
            DamageableObjects.Remove(receiver);
    }    
}
