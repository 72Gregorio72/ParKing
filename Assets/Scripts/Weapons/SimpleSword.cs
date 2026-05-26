using UnityEngine;

public class SimpleSword : Weapon
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float range = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject attackPoint;
    
    private float attackVisualizationDuration = 0.5f;

    private void Start()
    {
        if (attackPoint == null)
        {
            attackPoint = transform.root.Find("AttackPoint")?.gameObject;
			if (attackPoint == null)
			{
				Debug.LogWarning("SimpleSword: No AttackPoint found. Using weapon's position as attack center.");
			}
        }
    }

    public override void Attack()
    {
        if (!CanAttack()) return;
        
        lastAttackTime = Time.time;
        
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
        // Mostra il gizmo solo se l'attacco è stato fatto di recente
        if (Time.time - lastAttackTime < attackVisualizationDuration)
        {
            Vector3 gizmoPosition = attackPoint != null ? attackPoint.transform.position : transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gizmoPosition, range);
        }
    }
}
