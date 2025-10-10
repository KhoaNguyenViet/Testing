using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [Header("Player Health Settings")]
    [SerializeField] private int health;
    [SerializeField] private float damageRecoveryTime = 1f;
    private int currentHealth;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Cooldown")]
    [SerializeField] private float jumpCooldown = 0.3f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool jumpInput;

    private bool isGrounded;
    private bool canJump = true;
    private float lastJumpTime;

    private bool isFacingRight = true;

    private bool canTakeDamage = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
       
    }
    private void Start()
    {
        currentHealth = health;
    }

    void Update()
    {
        CheckGrounded();
        HandleJump();
        FlipSprite();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        HandleMovement();

        // Reset jump cooldown
        if (!canJump && Time.time - lastJumpTime >= jumpCooldown)
        {
            canJump = true;
        }
    }
    #region System Health/Damage
    public void TakeDamage(int damageAmount)
    {
        canTakeDamage = false;
        currentHealth -= damageAmount;
        StartCoroutine(DamageRecoveryRoutine());
    }
    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }
    #endregion
    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Update animation
        if (anim != null)
        {
            anim.SetBool("isJumping", !isGrounded);
        }
    }

    private void HandleMovement()
    {
        // Horizontal movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (jumpInput && isGrounded && canJump)
        {
            PerformJump();
        }
        jumpInput = false;
    }

    private void PerformJump()
    {
        // Sử dụng linearVelocity để nhảy trong Unity 6
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        canJump = false;
        lastJumpTime = Time.time;

        // Animation
        if (anim != null)
        {
            anim.SetBool("isJumping", true);
        }
    }

    // Input Events
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpInput = true;
        }
    }

    public void FlipSprite()
    {
        if ((moveInput.x > 0 && !isFacingRight) || (moveInput.x < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void UpdateAnimation()
    {
        if (anim == null) return;

        // Animation chạy
        float isRunning = Mathf.Abs(moveInput.x);
        anim.SetFloat("Run", isRunning);

        // Animation nhảy - sử dụng velocity y để blend tree
        anim.SetFloat("Jump", rb.linearVelocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
