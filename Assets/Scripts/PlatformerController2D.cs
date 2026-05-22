using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 60f;
    public float deceleration = 70f;
    public float velPower = 0.9f;

    [Header("Jump")]
    public float jumpForce = 14f;
    public int extraJumps = 1; // 1 = doppio salto
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;
    public bool variableJump = true;
    public float cutJumpGravityMultiplier = 2.2f;

    [Header("Gravity")]
    public float gravityScale = 3.5f;
    public float fallGravityMultiplier = 2.0f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.3f;
    public LayerMask wallLayer;
    public bool enableWallSlide = true;
    public float wallSlideSpeed = 2.5f;
    public bool enableWallJump = true;
    public Vector2 wallJumpForce = new Vector2(10f, 14f);
    public float wallJumpLockTime = 0.2f;

    [Header("Dash")]
    public bool enableDash = true;
    public float dashSpeed = 18f;
    public float dashTime = 0.15f;
    public float dashCooldown = 0.15f;

    // runtime
    private Rigidbody2D rb;
    private float inputX;
    private bool isGrounded;
    private bool isOnWall;
    private int jumpsLeft;

    // dash uses left
    private int dashesLeft = 1; // 1 dash in aria

    // timers
    private float coyoteCounter;
    private float jumpBufferCounter;

    private bool isDashing;
    private float dashCooldownUntil;

    // wall
    private bool wallJumpLock;
    private int wallDir; // -1 sinistra, +1 destra

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        ResetAirResources(); // reset iniziale
    }

    private void Update()
    {
        // Input
        inputX = isDashing || wallJumpLock ? 0f : Input.GetAxisRaw("Horizontal");

        // Checks
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        wallDir = CheckWallDir();
        isOnWall = wallDir != 0 && !isGrounded;

        // Ricarica risorse (salti extra e dash) sia a terra sia a muro
        if (isGrounded || isOnWall)
            ResetAirResources();

        // Timers: coyote e buffer
        coyoteCounter = isGrounded ? coyoteTime : Mathf.Max(0f, coyoteCounter - Time.deltaTime);
        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter = Mathf.Max(0f, jumpBufferCounter - Time.deltaTime);

        // Wall slide
        if (enableWallSlide && isOnWall && rb.linearVelocity.y < -wallSlideSpeed && !isDashing)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);

        // Jump handling
        if (jumpBufferCounter > 0f)
        {
            if (enableWallJump && isOnWall)
            {
                DoWallJump();
                jumpBufferCounter = 0f;
            }
            else if (coyoteCounter > 0f)
            {
                DoGroundJump();
                jumpBufferCounter = 0f;
            }
            else if (jumpsLeft > 0)
            {
                DoAirJump();
                jumpBufferCounter = 0f;
            }
        }

        // Variable jump cut
        if (variableJump && Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            rb.gravityScale = gravityScale * cutJumpGravityMultiplier;
        }

        // Dash: 1 per aria, si ricarica a terra o a muro
        if (enableDash && Time.time >= dashCooldownUntil && Input.GetButtonDown("Fire3") && !isDashing && dashesLeft > 0)
        {
            StartCoroutine(DashRoutine());
        }

        // Gravità dinamica
        if (!isDashing)
        {
            if (rb.linearVelocity.y < 0f)
                rb.gravityScale = gravityScale * fallGravityMultiplier;
            else if (!Input.GetButton("Jump") && variableJump && rb.linearVelocity.y > 0f)
                rb.gravityScale = gravityScale * cutJumpGravityMultiplier;
            else
                rb.gravityScale = gravityScale;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        float targetSpeed = inputX * moveSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rb.AddForce(new Vector2(movement, 0f));
    }

    private void DoGroundJump()
    {
        coyoteCounter = 0f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void DoAirJump()
    {
        jumpsLeft--;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void DoWallJump()
    {
        wallJumpLock = true;
        Invoke(nameof(ReleaseWallLock), wallJumpLockTime);

        int dir = -wallDir; // spingi lontano dal muro
        Vector2 v = new Vector2(dir * wallJumpForce.x, wallJumpForce.y);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(v, ForceMode2D.Impulse);
    }

    private void ReleaseWallLock() => wallJumpLock = false;

    private int CheckWallDir()
    {
        if (wallCheck == null) return 0;
        Vector2 origin = wallCheck.position;
        bool right = Physics2D.Raycast(origin, Vector2.right, wallCheckDistance, wallLayer);
        bool left = Physics2D.Raycast(origin, Vector2.left, wallCheckDistance, wallLayer);
        if (right == left) return 0;
        return right ? +1 : -1;
    }

    private System.Collections.IEnumerator DashRoutine()
    {
        isDashing = true;
        dashesLeft = Mathf.Max(0, dashesLeft - 1);
        dashCooldownUntil = Time.time + dashCooldown;

        float dir = Mathf.Abs(inputX) > 0.01f ? Mathf.Sign(inputX) : Mathf.Sign(transform.localScale.x);

        float prevRbGrav = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);

        float t = 0f;
        while (t < dashTime)
        {
            t += Time.deltaTime;
            rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);
            yield return null;
        }

        rb.gravityScale = prevRbGrav;
        isDashing = false;
    }

    private void ResetAirResources()
    {
        jumpsLeft = extraJumps;   // ricarica doppio salto
        dashesLeft = 1;           // ricarica 1 dash
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (wallCheck)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * wallCheckDistance);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.left * wallCheckDistance);
        }
    }
}
