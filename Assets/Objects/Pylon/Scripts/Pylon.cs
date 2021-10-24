using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pylon : MonoBehaviour
{
    [SerializeField] Transform MovingMass;
    [SerializeField] float RiseTime = 5f;
    [SerializeField] AnimationCurve RiseCurve;
    [SerializeField] float FallTime = 0.5f;
    [SerializeField] AnimationCurve FallCurve;
    [SerializeField] float MaxHeight = 5f;

    [SerializeField] HapticEffectSO ImpactEffect;

    float CurrentProgress = 0f;
    bool IsRising = true;
    Vector3 StartPos;
    Vector3 EndPos;

    // Start is called before the first frame update
    void Start()
    {
        StartPos = MovingMass.localPosition;
        EndPos = StartPos + Vector3.up * MaxHeight;
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
                HapticManager.PlayEffect(ImpactEffect, MovingMass.transform.position);

            // reset for the next move
            IsRising = !IsRising;
            CurrentProgress = 0f;
        }
    }
}
