using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("References")]
    public CharacterController characterController;
    public AudioSource walkingAudioSource;

    [Header("Movement Speeds")]
    public float walkSpeed = 2f;
    public float sprintSpeed = 5.335f;

    [Header("Audio Speed")]
    public float walkPitch = 1f;
    public float sprintPitch = 1.6f;
    public float movementThreshold = 0.1f;

    private void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        Vector3 horizontalVelocity = characterController.velocity;
        horizontalVelocity.y = 0f;

        float currentSpeed = horizontalVelocity.magnitude;

        bool isMoving =
            characterController.isGrounded &&
            currentSpeed > movementThreshold;

        if (isMoving)
        {
            float speedAmount = Mathf.InverseLerp(
                walkSpeed,
                sprintSpeed,
                currentSpeed
            );

            walkingAudioSource.pitch = Mathf.Lerp(
                walkPitch,
                sprintPitch,
                speedAmount
            );

            if (!walkingAudioSource.isPlaying)
            {
                walkingAudioSource.Play();
            }
        }
        else
        {
            if (walkingAudioSource.isPlaying)
            {
                walkingAudioSource.Stop();
            }

            walkingAudioSource.pitch = walkPitch;
        }
    }
}