using UnityEngine;

public class ReadableBook : MonoBehaviour
{
    [Header("Book Content")]
    [TextArea(5, 20)]
    public string loreText;

    [Header("Interaction")]
    public string promptText = "Press E to read";

    public void Interact()
    {
        if (BookUI.Instance != null)
        {
            BookUI.Instance.OpenBook(loreText);
        }
        else
        {
            Debug.LogWarning("ReadableBook could not find BookUI.");
        }
    }
}