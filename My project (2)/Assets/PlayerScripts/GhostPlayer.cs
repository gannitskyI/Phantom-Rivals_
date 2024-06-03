using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    private Transform[] recordedTransforms;
    private GhostTrail trail;
    private int currentFrame = 0;
    private bool isPlaying = false;
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

    private void Update()
    {
        if (isPlaying)
        {
            PlayRecording();
        }
    }

    private void PlayRecording()
    { 
        // Воспроизводим позицию призрака из записанных данных
        transform.position = recordedTransforms[currentFrame].position;
        transform.rotation = recordedTransforms[currentFrame].rotation;

        currentFrame++;
        trail.StopRecording();
    }

   
}