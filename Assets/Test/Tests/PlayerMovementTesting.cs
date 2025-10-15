using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovementTesting : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [Header("Player Health Settings")]
        [SerializeField] private int health;
        [SerializeField] private float damageRecoveryTime = 1f;
        [SerializeField] private Slider healthSlider;
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

        [Header("Knockback Settings")]
        [SerializeField] private float knockbackForce = 10f;
        [SerializeField] private float knockbackUpwardForce = 5f;
        [SerializeField] private float knockbackDuration = 0.3f;
        [SerializeField]
        private AnimationCurve knockbackCurve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.5f, 0.5f),
            new Keyframe(1f, 0f)
        );

        private Rigidbody2D rb;
        private Vector2 moveInput;
        private bool jumpInput;

        private bool isGrounded;
        private bool canJump = true;
        private float lastJumpTime;

        private bool isFacingRight = true;

        private bool canTakeDamage = true;
        private bool isKnockbackActive = false;
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
            UpdateHealthSlider();
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="damageAmount"></param>
    #region System Health/Damage
    public void TakeDamage(int damageAmount)
    {
        TakeDamage(damageAmount, transform.position);
    }
    public void TakeDamage(int damageAmount, Vector2 damageSourcePosition)
        {
            canTakeDamage = false;
            currentHealth -= damageAmount;
            StartCoroutine(DamageRecoveryRoutine());
            ApplyKnockback(damageSourcePosition);
            UpdateHealthSlider();
        }
        public void TakeDamage(int damageAmount, Transform damageSource)
        {
            TakeDamage(damageAmount, damageSource.position);
        }
        //private IEnumerator DamageRecoveryRoutine()
        //{
        //    yield return new WaitForSeconds(damageRecoveryTime);
        //    canTakeDamage = true;
        //}
        private void UpdateHealthSlider()
        {
            if (healthSlider == null) return;

            healthSlider.maxValue = health;
            healthSlider.value = currentHealth;
        }

        public void ApplyKnockback(Vector2 damageSourcePosition)
        {
            isKnockbackActive = true;

            // Tính hướng knockback (đẩy ra xa khỏi damage source)
            Vector2 knockbackDirection = (transform.position - (Vector3)damageSourcePosition).normalized;
            knockbackDirection.y = Mathf.Clamp(knockbackDirection.y + 0.3f, 0.1f, 1f); // Thêm lực hướng lên

            StartCoroutine(KnockbackRoutine(knockbackDirection));
        }

        private IEnumerator KnockbackRoutine(Vector2 direction)
        {
            float elapsedTime = 0f;
            Vector2 startVelocity = rb.linearVelocity;

            while (elapsedTime < knockbackDuration)
            {
                float curveValue = knockbackCurve.Evaluate(elapsedTime / knockbackDuration);

                // Áp dụng knockback force với curve
                Vector2 knockbackVelocity = direction * knockbackForce * curveValue;
                knockbackVelocity.y += knockbackUpwardForce * curveValue;

                rb.linearVelocity = new Vector2(knockbackVelocity.x, knockbackVelocity.y);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Kết thúc knockback
            isKnockbackActive = false;

            // Reset velocity về 0 hoặc giữ lại vertical velocity nếu đang rơi
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }

        private IEnumerator DamageRecoveryRoutine()
        {
            // Hiệu ứng nhấp nháy khi bất tử
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                StartCoroutine(FlashEffect(spriteRenderer, damageRecoveryTime));
            }

            yield return new WaitForSeconds(damageRecoveryTime);
            canTakeDamage = true;
        }

        private IEnumerator FlashEffect(SpriteRenderer renderer, float duration)
        {
            float elapsedTime = 0f;
            Color originalColor = renderer.color;

            while (elapsedTime < duration)
            {
                // Nhấp nháy giữa màu đỏ và màu gốc
                renderer.color = Mathf.PingPong(elapsedTime * 10f, 1f) > 0.5f ? Color.red : originalColor;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            renderer.color = originalColor;
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
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
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                TakeDamage(10, collision.transform);
            }
        }
    }

