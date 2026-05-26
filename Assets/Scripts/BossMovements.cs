using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScriptTemplate : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject projectilePrefab;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Parametri di Base")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float meleeRange = 2f;
    private bool isFacingRight = false;

    [Header("Parametri Attacchi")]
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float projectileSpeed = 18f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float rangedAttackRange = 5f;
    [SerializeField] private GameObject fallingCubePrefab;
    
    private float cooldownTimer;
    private bool isAttacking = false;
    private int lastAttackType = -1;

    // Stati del Boss
    private enum BossState { Idle, Chasing, Attacking }
    private BossState currentState = BossState.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Se il player non è assegnato, lo cerca tramite Tag
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        cooldownTimer -= Time.deltaTime;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Gestione degli Stati
        switch (currentState)
        {
            case BossState.Idle:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                if (distanceToPlayer <= detectionRange)
                    currentState = BossState.Chasing;
                break;

            case BossState.Chasing:
                LookAtPlayer();
                
                // Se è abbastanza vicino e il cooldown è passato, attacca
                if (distanceToPlayer <= meleeRange && cooldownTimer <= 0)
                {
                    StartCoroutine(SelectAttackRoutine());
                }
                // Altrimenti continua a camminare verso il player
                else if (distanceToPlayer > meleeRange)
                {
                    MoveTowardsPlayer();
                }
                else
                {
                    // Troppo vicino ma in cooldown: aspetta in Idle
                    currentState = BossState.Idle;
                }
                break;
        }
    }

    private void MoveTowardsPlayer()
    {
        // Movimento bidimensionale classico (X) mantenendo la gravità (Y)
        float direction = player.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        
        // Trigger animazione camminata (es. anim.SetBool("IsWalking", true);)
    }

    private void LookAtPlayer()
    {
        // Ruota il boss per guardare sempre il player
        if ((player.position.x > transform.position.x && !isFacingRight) ||
            (player.position.x < transform.position.x && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    // Coroutine per la scelta e l'esecuzione dell'attacco
    private IEnumerator SelectAttackRoutine()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero; // Si ferma prima di attaccare
        currentState = BossState.Attacking;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        int attackType;

        // Logica intelligente: sceglie l'attacco in base alla distanza, evitando di ripetere lo stesso attacco
        if (distanceToPlayer < meleeRange + 1.5f)
        {
            // Se vicino: Dash o Jump Slam (ma non lo stesso del turno precedente)
            attackType = Random.value > 0.4f ? 0 : 3;
            if (attackType == lastAttackType)
                attackType = attackType == 0 ? 3 : 0; // Scambia se è lo stesso
        }
        else if (distanceToPlayer < rangedAttackRange)
        {
            // Se a media distanza: Proiettile o Cubi dall'alto (ma non lo stesso del turno precedente)
            attackType = Random.value > 0.5f ? 1 : 2;
            if (attackType == lastAttackType)
                attackType = attackType == 1 ? 2 : 1; // Scambia se è lo stesso
        }
        else
        {
            // Se molto lontano: solo Cubi dall'alto
            attackType = 2;
        }

        lastAttackType = attackType;

        switch (attackType)
        {
            case 0: yield return StartCoroutine(MeleeDashAttack()); break;
            case 1: yield return StartCoroutine(RangedProjectileAttack()); break;
            case 2: yield return StartCoroutine(FallingCubesAttack()); break;
            case 3: yield return StartCoroutine(JumpSlamAttack()); break;
        }

        // Reset del Cooldown e ritorno allo stato di caccia
        cooldownTimer = attackCooldown;
        isAttacking = false;
        currentState = BossState.Chasing;
    }

    // --- ATTACCO 1: Fendente con Scatto ---
    private IEnumerator MeleeDashAttack()
    {
        yield return new WaitForSeconds(0.4f); // Carica
        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, rb.linearVelocity.y);
        yield return new WaitForSeconds(0.25f);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.3f);
    }

    // --- ATTACCO 2: Lancio di un Proiettile (Più veloce) ---
    private IEnumerator RangedProjectileAttack()
    {
        yield return new WaitForSeconds(0.5f);
        if (projectilePrefab != null && attackPoint != null)
        {
            GameObject obj = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
            Vector2 shootDir = (player.position - attackPoint.position).normalized;
            
            Rigidbody2D projRb = obj.GetComponent<Rigidbody2D>();
            if (projRb != null) projRb.linearVelocity = shootDir * projectileSpeed;
            
            DoDamage dd = obj.GetComponent<DoDamage>();
            if (dd != null) dd.SetShooter(gameObject);
        }
        yield return new WaitForSeconds(0.3f);
    }

    // --- ATTACCO 3: Cubi dall'alto ---
    private IEnumerator FallingCubesAttack()
    {
        yield return new WaitForSeconds(0.5f);
        int cubeCount = 6;
        for (int i = 0; i < cubeCount; i++)
        {
            if (fallingCubePrefab != null)
            {
                float xOffset = Random.Range(-6f, 6f);
                Vector3 spawnPos = new Vector3(player.position.x + xOffset, transform.position.y + 12f, 0);
                Instantiate(fallingCubePrefab, spawnPos, Quaternion.identity);
            }
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(0.5f);
    }

    // --- ATTACCO 4: Salto e Schiacciata (Jump Slam) ---
    private IEnumerator JumpSlamAttack()
    {
        yield return new WaitForSeconds(0.3f);
        float xDist = player.position.x - transform.position.x;
        rb.linearVelocity = new Vector2(xDist * 1.2f, jumpForce);
        yield return new WaitUntil(() => rb.linearVelocity.y < 0);
        rb.linearVelocity = new Vector2(0, -jumpForce * 2f);
        yield return new WaitForSeconds(0.7f);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.4f);
    }

    // Visualizzazione dei Raycast/Range nell'editor di Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangedAttackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}