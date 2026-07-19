using UnityEngine;

public class ZombieAnimationEvents : MonoBehaviour
{
    private ZombieAI zombieAI;

    private void Awake()
    {
        zombieAI = GetComponentInParent<ZombieAI>();

        if (zombieAI == null)
        {
            Debug.LogError(
                "ZombieAnimationEvents could not find ZombieAI."
            );
        }
    }

    public void ApplyAttackDamage()
    {
        zombieAI?.ApplyAttackDamage();
    }

    public void FinishAttack()
    {
        Debug.Log("FinishAttack animation event called");
        zombieAI?.FinishAttack();
    }
}