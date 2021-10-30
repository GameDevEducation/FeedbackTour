using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DamageOverlayEffect;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] PostProcessVolume LinkedPPV;

    [SerializeField] float MaxHealth = 100f;
    [SerializeField] float DecayRate = 0.2f;

    [SerializeField] float AttackRate = 2f;
    [SerializeField] float ReleaseRate = 1f;

    [SerializeField] float MaxEffectIntensity = 0.5f;

    [SerializeField] AnimationCurve IntensityDueToHealth;

    float TargetIntensity = 0f;
    float CurrentIntensity = 0f;
    float CurrentHealth;

    DamageOverlay OverlaySettings;

    // Start is called before the first frame update
    void Start()
    {
        LinkedPPV.profile.TryGetSettings<DamageOverlay>(out OverlaySettings);
        CurrentHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // decay the target intensity
        if (TargetIntensity > 0f)
        {
            TargetIntensity = Mathf.Clamp01(TargetIntensity - DecayRate * Time.deltaTime);
            TargetIntensity = Mathf.Max(IntensityDueToHealth.Evaluate(CurrentHealth / MaxHealth), TargetIntensity);
        }

        // intensity needs updating
        if (CurrentIntensity != TargetIntensity)
        {
            float rate = TargetIntensity > CurrentIntensity ? AttackRate : ReleaseRate;
            CurrentIntensity = Mathf.MoveTowards(CurrentIntensity, TargetIntensity, rate * Time.deltaTime);
        }

        OverlaySettings.Intensity.value = Mathf.Lerp(0f, MaxEffectIntensity, CurrentIntensity);
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0f, MaxHealth);

        float damagePercent = Mathf.Clamp01(amount / MaxHealth);

        TargetIntensity = Mathf.Clamp01(TargetIntensity + damagePercent);
    }
}
