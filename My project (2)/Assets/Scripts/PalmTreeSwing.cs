using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmTreeSwing : MonoBehaviour
{
    public float minSwingSpeed = 0.5f;
    public float maxSwingSpeed = 2.0f;
    public float minSwingAmplitude = 5.0f;
    public float maxSwingAmplitude = 20.0f;

    private float originalRotation;
    private float swingSpeed;
    private float swingAmplitude;

    void Start()
    {
        originalRotation = transform.rotation.eulerAngles.z;
        RandomizeValues();
    }

    void Update()
    {
        float swingOffset = Mathf.Sin(Time.time * swingSpeed) * swingAmplitude;
        float newRotation = originalRotation + swingOffset;

        transform.rotation = Quaternion.Euler(0, 0, newRotation);
    }

    void RandomizeValues()
    {
        swingSpeed = Random.Range(minSwingSpeed, maxSwingSpeed);
        swingAmplitude = Random.Range(minSwingAmplitude, maxSwingAmplitude);
    }
}
