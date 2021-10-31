using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBouncer : MonoBehaviour
{
    [SerializeField] float BounceAmplitude = 0.25f;
    [SerializeField] float BouncePeriod = 2f;

    [SerializeField] float RotationSpeed = 45f;

    Vector3 InitialPosition;
    float Progress;

    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = transform.position;
        Progress = Random.Range(0f, 1f);

        transform.Rotate(0f, Random.Range(0f, 360f), 0);
    }

    // Update is called once per frame
    void Update()
    {
        // update the progress
        Progress = (Progress + (Time.deltaTime / BouncePeriod)) % 1f;

        // calculate and apply the height
        float heightDelta = BounceAmplitude * Mathf.Sin(Progress * Mathf.PI * 2);
        transform.position = InitialPosition + Vector3.up * heightDelta;

        // apply the rotation
        transform.Rotate(0f, RotationSpeed * Time.deltaTime, 0f);
    }
}
