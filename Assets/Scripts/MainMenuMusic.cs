using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MainMenuMusic : MonoBehaviour
{
    [Header("Music")]
    public AudioClip menuMusic;

    [Range(0f, 1f)]
    public float volume = 0.3f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.volume = volume;
        audioSource.clip = menuMusic;
    }

    private void Start()
    {
        if (menuMusic != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning(
                "Main menu music clip has not been assigned.",
                this
            );
        }
    }
}