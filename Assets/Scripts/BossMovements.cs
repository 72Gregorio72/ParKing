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
    private float cooldownTimer;
    private bool isAttacking = false;

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

    // Coroutine per la scelta e l'esecuzione dell'attacco (stile Hollow Knight)
    private IEnumerator SelectAttackRoutine()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero; // Si ferma prima di attaccare
        currentState = BossState.Attacking;

        // Scegli casualmente tra attacco 1 e attacco 2
        int randomAttack = Random.Range(0, 2);

        if (randomAttack == 0)
        {
            yield return StartCoroutine(MeleeDashAttack());
        }
        else
        {
            yield return StartCoroutine(RangedProjectileAttack());
        }

        // Reset del Cooldown e ritorno allo stato di caccia
        cooldownTimer = attackCooldown;
        isAttacking = false;
        currentState = BossState.Chasing;
    }

    // --- ATTACCO 1: Fendente con Scatto (tipo "False Knight" o "Hornet") ---
    private IEnumerator MeleeDashAttack()
    {
        // 1. Anticipazione (Il boss si ferma, carica l'attacco)
        // anim.SetTrigger("MeleeAnticipation");
        yield return new WaitForSeconds(0.5f); // Tempo di carica del colpo

        // 2. Esecuzione (Scatto in avanti)
        // anim.SetTrigger("MeleeAttack");
        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, rb.linearVelocity.y);

        yield return new WaitForSeconds(0.2f); // Durata dello scatto
        rb.linearVelocity = Vector2.zero;

        // 3. Recupero (Il boss riprende fiato)
        yield return new WaitForSeconds(0.3f);
    }

    // --- ATTACCO 2: Lancio di un Proiettile (tipo "Soul Master") ---
    private IEnumerator RangedProjectileAttack()
    {
        // 1. Anticipazione
        // anim.SetTrigger("RangedAnticipation");
        yield return new WaitForSeconds(0.6f);

        // 2. Esecuzione (Istanzia il proiettile)
        // anim.SetTrigger("RangedAttack");
        if (projectilePrefab != null && attackPoint != null)
        {
            GameObject obj = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
            // Configura la direzione del proiettile (puoi passare una direzione allo script del proiettile stesso)
            Vector2 shootDir = (player.position - attackPoint.position).normalized;
            obj.GetComponent<Rigidbody2D>().linearVelocity = shootDir * 10f; // Esempio di velocità proiettile
        }

        yield return new WaitForSeconds(0.4f); // Fine animazione
    }

    // Visualizzazione dei Raycast/Range nell'editor di Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}