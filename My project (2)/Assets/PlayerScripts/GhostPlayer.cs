using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    private Transform[] recordedTransforms;
    private GhostTrail trail;
    private int currentFrame = 0;
    private bool isPlaying = false;
    private float lerpTime = 0f;
    private float lerpDuration = 0.1f; // Длительность интерполяции

    private void Start()
    {
        trail = FindObjectOfType<GhostTrail>();
    }

    public void SetRecordedTransforms(Transform[] transforms)
    {
        recordedTransforms = transforms;
    }

    public void StartPlayback()
    {
        isPlaying = true;
    }

    private void FixedUpdate()
    {
        if (isPlaying)
        {
            PlayRecording();
        }
    }

    private void PlayRecording()
    {
        // Воспроизводим позицию призрака из записанных данных
        if (lerpTime < lerpDuration)
        {
            lerpTime += Time.deltaTime;
            float t = lerpTime / lerpDuration;
            transform.position = Vector3.Lerp(recordedTransforms[currentFrame - 1].position, recordedTransforms[currentFrame].position, t);
            transform.rotation = Quaternion.Slerp(recordedTransforms[currentFrame - 1].rotation, recordedTransforms[currentFrame].rotation, t);
        }
        else
        {
            currentFrame++;
            lerpTime = 0f;
            if (currentFrame >= recordedTransforms.Length)
            {
                isPlaying = false;
                trail.StopRecording();
            }
        }
    }
}
