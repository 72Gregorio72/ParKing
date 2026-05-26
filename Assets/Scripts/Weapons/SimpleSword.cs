using UnityEngine;

public class SimpleSword : Weapon
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float range = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject attackPoint;

    private void Start()
    {
        if (attackPoint == null)
        {
            attackPoint = transform.Find("AttackPoint")?.gameObject;
        }
    }

    public override void Attack()
    {
        if (!CanAttack()) return;
        
        // Circular overlap check for damage centered at attack point
        Vector3 attackPosition = attackPoint != null ? attackPoint.transform.position : transform.position;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, range, enemyLayer);
        
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Hit " + enemyCollider.name + " for " + damage + " damage.");
            }
        }

        ResetCooldown();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 gizmoPosition = attackPoint != null ? attackPoint.transform.position : transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoPosition, range);
    }
}
