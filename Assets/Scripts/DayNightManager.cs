using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    [Header("Sun")]
    public Light sun;

    [Header("Day Length")]
    public float fullDayLength = 600f;

    [Header("Day Counter")]
    public int currentDay = 1;

    private float dayIntensity = 0.2f;
    private float nightIntensity = 0.1f;

    private Color dayAmbientColor =
        new Color(0.55f, 0.58f, 0.62f);

    private Color nightAmbientColor =
        new Color(0.08f, 0.09f, 0.12f);

    private Color daySkyTint =
        new Color(0.35f, 0.45f, 0.6f);

    private Color nightSkyTint =
        new Color(0.05f, 0.065f, 0.1f);

    private float timeOfDay = 0.25f;

    public bool IsNight { get; private set; }

    void Update()
    {
        if (sun == null)
        {
            return;
        }

        timeOfDay += Time.deltaTime / fullDayLength;

        if (timeOfDay >= 1f)
        {
            timeOfDay = 0f;

            currentDay++;

            Debug.Log("Day " + currentDay + " has started.");
        }

        UpdateLighting();
    }

    private void UpdateLighting()
    {
        float sunAngle =
            timeOfDay * 360f - 90f;

        sun.transform.rotation =
            Quaternion.Euler(sunAngle, 170f, 0f);

        float lightAmount =
            Mathf.Clamp01(
                Mathf.Sin(timeOfDay * Mathf.PI)
            );

        sun.intensity = Mathf.Lerp(
            nightIntensity,
            dayIntensity,
            lightAmount
        );

        RenderSettings.ambientLight = Color.Lerp(
            nightAmbientColor,
            dayAmbientColor,
            lightAmount
        );

        if (RenderSettings.skybox != null)
        {
            RenderSettings.skybox.SetColor(
                "_Tint",
                Color.Lerp(
                    nightSkyTint,
                    daySkyTint,
                    lightAmount
                )
            );

            RenderSettings.skybox.SetFloat(
                "_Exposure",
                Mathf.Lerp(
                    0.008f,
                    0.65f,
                    lightAmount
                )
            );
        }

        IsNight = lightAmount < 0.25f;
    }
}