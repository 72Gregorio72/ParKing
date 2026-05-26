using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected string weaponName;
    [SerializeField] protected float attackCooldown = 0.5f;
    
    protected float lastAttackTime;

    public virtual bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public abstract void Attack();

    protected void ResetCooldown()
    {
        lastAttackTime = Time.time;
    }
}
