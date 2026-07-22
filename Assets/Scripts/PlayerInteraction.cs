using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionRange = 3f;
    public TextMeshProUGUI promptText;

    [Header("Player References")]
    public Inventory inventory;
    public PlayerEquipment playerEquipment;

    private InteractableResource currentResource;
    private ReadableBook currentBook;

    private Animator animator;
    private bool isInteracting;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        if (playerEquipment == null)
        {
            playerEquipment = GetComponent<PlayerEquipment>();
        }

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Do not search for other interactions while reading.
        if (BookUI.Instance != null && BookUI.Instance.IsOpen)
        {
            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }

            return;
        }

        FindNearbyInteractable();
    }

    private void FindNearbyInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            interactionRange
        );

        InteractableResource closestResource = null;
        ReadableBook closestBook = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            InteractableResource resource =
                hit.GetComponentInParent<InteractableResource>();

            ReadableBook book =
                hit.GetComponentInParent<ReadableBook>();

            if (resource != null)
            {
                float resourceDistance = Vector3.Distance(
                    transform.position,
                    resource.transform.position
                );

                if (resourceDistance < closestDistance)
                {
                    closestDistance = resourceDistance;
                    closestResource = resource;
                    closestBook = null;
                }
            }

            if (book != null)
            {
                float bookDistance = Vector3.Distance(
                    transform.position,
                    book.transform.position
                );

                if (bookDistance < closestDistance)
                {
                    closestDistance = bookDistance;
                    closestBook = book;
                    closestResource = null;
                }
            }
        }

        currentResource = closestResource;
        currentBook = closestBook;

        UpdatePrompt();
    }

    private void UpdatePrompt()
    {
        if (promptText == null)
        {
            return;
        }

        if (isInteracting)
        {
            promptText.gameObject.SetActive(false);
            return;
        }

        if (currentBook != null)
        {
            promptText.text = currentBook.promptText;
            promptText.gameObject.SetActive(true);
        }
        else if (currentResource != null)
        {
            promptText.text = currentResource.promptText;
            promptText.gameObject.SetActive(true);
        }
        else
        {
            promptText.gameObject.SetActive(false);
        }
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
        {
            return;
        }

        // Pressing E while the book is open closes it.
        if (BookUI.Instance != null && BookUI.Instance.IsOpen)
        {
            BookUI.Instance.CloseBook();
            return;
        }

        if (isInteracting)
        {
            return;
        }

        // Open the nearby book.
        if (currentBook != null)
        {
            currentBook.Interact();

            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }

            return;
        }

        // Otherwise, interact with the nearby resource.
        if (currentResource != null)
        {
            StartCoroutine(InteractWithResource());
        }
    }

    private IEnumerator InteractWithResource()
    {
        isInteracting = true;

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }

        InteractableResource resourceBeingUsed = currentResource;

        if (animator != null &&
            resourceBeingUsed != null &&
            !string.IsNullOrEmpty(resourceBeingUsed.animationTrigger))
        {
            animator.SetTrigger(resourceBeingUsed.animationTrigger);
        }

        yield return new WaitForSeconds(0.8f);

        if (resourceBeingUsed != null)
        {
            resourceBeingUsed.Interact(inventory, playerEquipment);
        }

        yield return new WaitForSeconds(0.3f);

        isInteracting = false;
    }
}