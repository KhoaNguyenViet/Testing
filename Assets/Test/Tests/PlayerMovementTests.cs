using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerMovementTests
{
    private GameObject playerObj;
    private PlayerMovementTesting player;
    private Rigidbody2D rb;

    [SetUp]
    public void Setup()
    {
        // Tạo đối tượng Player giả lập để test
        playerObj = new GameObject("Player");
        rb = playerObj.AddComponent<Rigidbody2D>();
        player = playerObj.AddComponent<PlayerMovementTesting>();

        // Thêm SpriteRenderer (để tránh null khi flash effect)
        playerObj.AddComponent<SpriteRenderer>();
        playerObj.transform.position = Vector3.zero;

        // Giả lập ground check
        GameObject groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.parent = playerObj.transform;
        groundCheck.transform.localPosition = Vector3.zero;

        // Gán private field groundCheck qua Reflection
        player.GetType().GetField("groundCheck",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, groundCheck.transform);

        // Thiết lập giá trị mặc định cho health và currentHealth
        player.GetType().GetField("health",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, 100);

        player.GetType().GetField("currentHealth",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, 100);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(playerObj);
    }

    // ============================================================
    //  TEST HEALTH SYSTEM
    // ============================================================

    [UnityTest]
    public IEnumerator TakeDamage_ShouldReduceHealth()
    {
        // Gọi TakeDamage
        player.TakeDamage(20, Vector2.zero);

        // Chờ coroutine (một frame là đủ)
        yield return null;

        // Lấy giá trị currentHealth sau khi bị damage
        int currentHealth = (int)player.GetType().GetField("currentHealth",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(player);

        Assert.AreEqual(80, currentHealth, "Máu không giảm đúng sau khi TakeDamage.");
    }

    // ============================================================
    //  TEST MOVEMENT
    // ============================================================

    [UnityTest]
    public IEnumerator HandleMovement_ShouldChangeVelocity()
    {
        // Gán moveInput.x = 1
        player.GetType().GetField("moveInput",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, new Vector2(1, 0));

        // Gọi FixedUpdate để mô phỏng chuyển động
        player.Invoke("FixedUpdate", 0f);
        yield return null;

        Assert.Greater(rb.linearVelocity.x, 0f, "Player không di chuyển khi có input.");
    }

    // ============================================================
    //  TEST FLIP SPRITE
    // ============================================================

    [Test]
    public void FlipSprite_ShouldFlipDirection()
    {
        // Giả lập đang di chuyển sang phải và đang quay trái
        player.GetType().GetField("moveInput",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, new Vector2(1, 0));

        player.GetType().GetField("isFacingRight",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, false);

        // Gọi FlipSprite
        player.FlipSprite();

        bool isFacingRight = (bool)player.GetType().GetField("isFacingRight",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(player);

        Assert.IsTrue(isFacingRight, "FlipSprite không đổi hướng đúng.");
    }

    // ============================================================
    //  TEST JUMP
    // ============================================================

    [UnityTest]
    public IEnumerator PerformJump_ShouldAddVerticalVelocity()
    {
        // Gán điều kiện để có thể nhảy
        player.GetType().GetField("isGrounded",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, true);

        player.GetType().GetField("jumpInput",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, true);

        float beforeY = rb.linearVelocity.y;

        // Gọi hàm HandleJump
        player.Invoke("HandleJump", 0f);
        yield return null;

        float afterY = rb.linearVelocity.y;

        Assert.Greater(afterY, beforeY, "Nhảy không làm tăng vận tốc trục Y.");
    }

    // ============================================================
    //  TEST KNOCKBACK
    // ============================================================

    [UnityTest]
    public IEnumerator ApplyKnockback_ShouldChangeVelocity()
    {
        Vector2 damageSource = new Vector2(-1, 0);

        player.ApplyKnockback(damageSource);

        yield return new WaitForSeconds(0.1f);

        Assert.AreNotEqual(Vector2.zero, rb.linearVelocity, "Knockback không thay đổi vận tốc.");
    }
}
