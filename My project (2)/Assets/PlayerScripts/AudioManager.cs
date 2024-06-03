using UnityEngine;

public class CarSound : MonoBehaviour
{

    public AudioClip lowSpeedSound, mediumSpeedSound, highSpeedSound;
    private AudioSource audioSource;
    private PlayerControl playerControl;
    public float lowSpeedThreshold = 10f, mediumSpeedThreshold = 30f;
    private float targetPitch, targetVolume;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerControl = GetComponent<PlayerControl>();
        audioSource.loop = true;
        audioSource.clip = lowSpeedSound;
        audioSource.Play();
    }

    private void Update()
    {
        float speed = playerControl.currentSpeed;
        float absSpeed = Mathf.Abs(speed);

        if (absSpeed < lowSpeedThreshold && audioSource.clip != lowSpeedSound)
        {
            audioSource.clip = lowSpeedSound;
            audioSource.Play();
        }
        else if (absSpeed >= lowSpeedThreshold && absSpeed < mediumSpeedThreshold && audioSource.clip != mediumSpeedSound)
        {
            audioSource.clip = mediumSpeedSound;
            audioSource.Play();
        }
        else if (absSpeed >= mediumSpeedThreshold && audioSource.clip != highSpeedSound)
        {
            audioSource.clip = highSpeedSound;
            audioSource.Play();
        }

        targetPitch = Mathf.Lerp(0.5f, 1f, Mathf.InverseLerp(0f, mediumSpeedThreshold, absSpeed));
        targetVolume = Mathf.Lerp(0.1f, 0.5f, Mathf.InverseLerp(0f, mediumSpeedThreshold, absSpeed));
    }

    private void FixedUpdate()
    {
        audioSource.pitch = Mathf.MoveTowards(audioSource.pitch, targetPitch, Time.fixedDeltaTime * 5f);
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, Time.fixedDeltaTime * 5f);
    }
}