using UnityEngine;
using TMPro;

public class CompassMarkerBar : MonoBehaviour
{
    public Transform player;
    public RectTransform markerContainer;
    public TMP_Text markerPrefab;

    private float panelWidth = 350f;
    private float pixelsPerDegree = 4f;

    private readonly string[] labels = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };

    private TMP_Text[] markers;

    void Start()
    {
        markers = new TMP_Text[labels.Length];

        for (int i = 0; i < labels.Length; i++)
        {
            TMP_Text marker = Instantiate(markerPrefab, markerContainer);
            marker.alignment = TextAlignmentOptions.Center;
            marker.gameObject.SetActive(true);
            marker.text = labels[i];
            markers[i] = marker;
        }
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        float heading = player.eulerAngles.y;

        for (int i = 0; i < markers.Length; i++)
        {
            float angle = i * 45f;
            float relativeAngle = Mathf.DeltaAngle(heading, angle);
            float x = relativeAngle * pixelsPerDegree;

            RectTransform rect = markers[i].rectTransform;
            rect.anchoredPosition = new Vector2(x, 0f);

            bool visible = Mathf.Abs(x) <= panelWidth * 0.5f;
            markers[i].gameObject.SetActive(visible);
        }
    }
}