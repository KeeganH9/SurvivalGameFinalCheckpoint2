using UnityEngine;

public class ZombieAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource movementAudioSource;
    public AudioSource effectsAudioSource;

    [Header("Audio Clips")]
    public AudioClip runningClip;
    public AudioClip attackClip;
    public AudioClip deathClip;

    [Header("Volumes")]
    [Range(0f, 1f)]
    public float runningVolume = 0.67f;

    [Range(0f, 2f)]
    public float attackVolume = 1f;

    [Range(0f, 2f)]
    public float deathVolume = 1f;

    [Header("Running Detection")]
    public float minimumMovementSpeed = 0.1f;

    [Header("3D Sound Distance")]
    public float minimumDistance = 0.5f;
    public float maximumDistance = 15f;

    private Vector3 previousPosition;
    private bool isDead;
    private bool loggedRunningAttempt;

    private void Awake()
    {
        ConfigureAudioSource(movementAudioSource);
        ConfigureAudioSource(effectsAudioSource);

        if (movementAudioSource != null)
        {
            movementAudioSource.clip = runningClip;
            movementAudioSource.loop = true;
            movementAudioSource.volume = runningVolume;
        }

        if (effectsAudioSource != null)
        {
            effectsAudioSource.loop = false;
            effectsAudioSource.volume = 1f;
        }

        if (movementAudioSource == effectsAudioSource)
        {
            Debug.LogError(
                "The same Audio Source is assigned to both ZombieAudio fields.",
                this
            );
        }

        previousPosition = transform.position;
    }

    private void ConfigureAudioSource(AudioSource audioSource)
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.enabled = true;
        audioSource.mute = false;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = minimumDistance;
        audioSource.maxDistance = maximumDistance;
    }

    private void Update()
    {
        float movementSpeed =
            Vector3.Distance(
                transform.position,
                previousPosition
            ) / Mathf.Max(Time.deltaTime, 0.0001f);

        previousPosition = transform.position;

        UpdateRunningSound(movementSpeed);
    }

    private void UpdateRunningSound(float movementSpeed)
    {
        if (isDead)
        {
            StopRunningSound();
            return;
        }

        bool isMoving =
            movementSpeed > minimumMovementSpeed;

        if (isMoving)
        {
            if (movementAudioSource != null &&
                runningClip != null &&
                !movementAudioSource.isPlaying)
            {
                movementAudioSource.clip = runningClip;
                movementAudioSource.Play();

                if (!loggedRunningAttempt)
                {
                    loggedRunningAttempt = true;

                    Debug.LogWarning(
                        "Running sound started. IsPlaying: " +
                        movementAudioSource.isPlaying,
                        this
                    );
                }
            }
        }
        else
        {
            StopRunningSound();
        }
    }

    private void StopRunningSound()
    {
        if (movementAudioSource != null &&
            movementAudioSource.isPlaying)
        {
            movementAudioSource.Stop();
        }
    }

    public void PlayAttackSound()
    {
        if (isDead ||
            effectsAudioSource == null ||
            attackClip == null)
        {
            return;
        }

        effectsAudioSource.PlayOneShot(
            attackClip,
            attackVolume
        );
    }

    public void PlayDeathSound()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        StopRunningSound();

        if (effectsAudioSource != null &&
            deathClip != null)
        {
            effectsAudioSource.Stop();

            effectsAudioSource.PlayOneShot(
                deathClip,
                deathVolume
            );
        }
    }
}