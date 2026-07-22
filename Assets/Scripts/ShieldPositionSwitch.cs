using UnityEngine;

public class ShieldPositionSwitcher : MonoBehaviour
{
    [Header("Shield Objects")]
    public GameObject shieldIdle;
    public GameObject shieldBlocking;

    private PlayerCombat playerCombat;

    private void Start()
    {
        playerCombat = GetComponentInParent<PlayerCombat>();

        ShowIdleShield();
    }

    private void Update()
    {
        bool isBlocking =
            playerCombat != null &&
            playerCombat.IsBlocking;

        shieldIdle.SetActive(!isBlocking);
        shieldBlocking.SetActive(isBlocking);
    }

    private void ShowIdleShield()
    {
        if (shieldIdle != null)
        {
            shieldIdle.SetActive(true);
        }

        if (shieldBlocking != null)
        {
            shieldBlocking.SetActive(false);
        }
    }
}