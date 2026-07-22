using TMPro;
using UnityEngine;

public class BookUI : MonoBehaviour
{
    public static BookUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject bookCanvas;
    public TMP_Text loreText;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        Instance = this;

        if (bookCanvas != null)
        {
            bookCanvas.SetActive(false);
        }
    }

    public void OpenBook(string newLoreText)
    {
        if (loreText != null)
        {
            loreText.text = newLoreText;
        }

        bookCanvas.SetActive(true);
        IsOpen = true;

        Time.timeScale = 0f;
    }

    public void CloseBook()
    {
        bookCanvas.SetActive(false);
        IsOpen = false;

        Time.timeScale = 1f;
    }
}