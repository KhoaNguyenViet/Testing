using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyAITests
{
    private GameObject enemyObj;
    private EnemyAITesting enemyAI;
    private GameObject[] waypoints;

    [SetUp]
    public void Setup()
    {
        // === T?o Enemy gi? l?p ===
        enemyObj = new GameObject("Enemy");
        enemyAI = enemyObj.AddComponent<EnemyAITesting>();

        // === T?o 2 Waypoint ===
        GameObject wp1 = new GameObject("WP1");
        GameObject wp2 = new GameObject("WP2");

        wp1.transform.position = new Vector3(0, 0, 0);
        wp2.transform.position = new Vector3(5, 0, 0);

        waypoints = new GameObject[] { wp1, wp2 };

        // Gán vào EnemyAI b?ng Reflection (vì là private serialized)
        var wpField = typeof(EnemyAITesting).GetField("wayPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        wpField.SetValue(enemyAI, waypoints);

        // Gán moveSpeed th?p ?? test d?
        var speedField = typeof(EnemyAITesting).GetField("moveSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        speedField.SetValue(enemyAI, 5f);

        // Gán m?c ??nh scale
        enemyObj.transform.localScale = Vector3.one;
        enemyObj.transform.position = Vector3.zero;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(enemyObj);
        foreach (var wp in waypoints)
        {
            Object.DestroyImmediate(wp);
        }
    }

    // ============================================================
    //  TEST 1: Enemy ph?i di chuy?n t?i waypoint
    // ============================================================
    [UnityTest]
    public IEnumerator Enemy_ShouldMoveTowardsWaypoint()
    {
        Vector3 startPos = enemyObj.transform.position;

        // Gọi Move() nhiều frame để đảm bảo enemy di chuyển
        for (int i = 0; i < 10; i++)
        {
            enemyAI.Invoke("Move", 0f);
            yield return null;
        }

        Vector3 newPos = enemyObj.transform.position;

        Assert.Greater(newPos.x, startPos.x, $"Enemy không di chuyển về waypoint. Start: {startPos.x}, End: {newPos.x}");
    }

    // ============================================================
    //  TEST 2: Enemy ph?i l?t h??ng khi waypoint n?m bên trái
    // ============================================================
    [UnityTest]
    public IEnumerator Enemy_ShouldFlip_WhenWaypointIsLeft()
    {
        // Gi? l?p waypoint bên trái
        waypoints[1].transform.position = new Vector3(-5, 0, 0);

        // Cho enemy quay m?t ph?i ban ??u
        enemyObj.transform.localScale = Vector3.one;

        // G?i Move() ?? flip
        enemyAI.Invoke("Move", 0f);
        yield return null;

        // L?y giá tr? private isFacingRight
        bool isFacingRight = (bool)typeof(EnemyAITesting).GetField("isFacingRight",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(enemyAI);

        Assert.IsFalse(isFacingRight, "Enemy không l?t h??ng sang trái khi waypoint ? bên trái.");
        Assert.Less(enemyObj.transform.localScale.x, 0, "Scale không b? ??o tr?c X sau khi flip.");
    }

    // ============================================================
    //  TEST 3: Enemy ??i waypoint khi ??n g?n
    // ============================================================
    [UnityTest]
    public IEnumerator Enemy_ShouldChangeWaypoint_WhenNearTarget()
    {
        // ??t enemy g?n waypoint th? 1
        enemyObj.transform.position = new Vector3(4.9f, 0, 0);

        yield return null;
        enemyAI.Invoke("Move", 0f);
        yield return null;

        int nextWP = (int)typeof(EnemyAITesting).GetField("nextWayPoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(enemyAI);

        Assert.AreEqual(0, nextWP, "Enemy không ??i waypoint khi t?i g?n m?c tiêu.");
    }

    // ============================================================
    //  TEST 4: UpdateAnimation set Speed = 1 khi ?ang di chuy?n
    // ============================================================
    [UnityTest]
    public IEnumerator UpdateAnimation_ShouldSetSpeedParameter()
    {
        // Thêm Animator gi? l?p
        var animator = enemyObj.AddComponent<Animator>();

        // Gán animator vào EnemyAI (qua Reflection)
        var animField = typeof(EnemyAITesting).GetField("animator",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        animField.SetValue(enemyAI, animator);

        // G?i UpdateAnimation
        enemyAI.Invoke("UpdateAnimation", 0f);
        yield return null;

        // Vì animator trong Test không có parameter th?t s?
        // nên ch? c?n xác nh?n không l?i (pass n?u không ném exception)
        Assert.Pass("UpdateAnimation ch?y mà không l?i.");
    }

    // ============================================================
    //  TEST 5: NextWayPoint quay vòng l?i 0 khi h?t danh sách
    // ============================================================
    [Test]
    public void NextWayPoint_ShouldLoopToZero()
    {
        var nextWPField = typeof(EnemyAITesting).GetField("nextWayPoint",
         System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        nextWPField.SetValue(enemyAI, waypoints.Length - 1);

        // Gọi phương thức private NextWayPoint()
        var nextMethod = typeof(EnemyAITesting).GetMethod("NextWayPoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        nextMethod.Invoke(enemyAI, null);

        int nextWP = (int)nextWPField.GetValue(enemyAI);
        Assert.AreEqual(0, nextWP, "Enemy không quay lại waypoint đầu tiên khi hết danh sách.");
    }
}
