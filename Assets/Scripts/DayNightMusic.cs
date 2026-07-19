using UnityEngine;

public class DayNightMusic : MonoBehaviour
{
    public DayNightManager dayNightManager;

    public AudioSource dayMusic;
    public AudioSource nightMusic;

    private float fadeSpeed = 1f;
    private float dayMaxVolume = 0.06f;
    private float nightMaxVolume = 0.06f;

    void Start()
    {
        if (dayMusic != null)
        {
            dayMusic.loop = true;
            dayMusic.volume = 0f;
            dayMusic.Play();
        }

        if (nightMusic != null)
        {
            nightMusic.loop = true;
            nightMusic.volume = 0f;
            nightMusic.Play();
        }
    }

    void Update()
    {
        if (dayNightManager == null || dayMusic == null || nightMusic == null)
        {
            return;
        }

        float targetDayVolume = dayNightManager.IsNight ? 0f : dayMaxVolume;
        float targetNightVolume = dayNightManager.IsNight ? nightMaxVolume : 0f;

        dayMusic.volume = Mathf.MoveTowards(
            dayMusic.volume,
            targetDayVolume,
            fadeSpeed * Time.deltaTime
        );

        nightMusic.volume = Mathf.MoveTowards(
            nightMusic.volume,
            targetNightVolume,
            fadeSpeed * Time.deltaTime
        );
    }
}