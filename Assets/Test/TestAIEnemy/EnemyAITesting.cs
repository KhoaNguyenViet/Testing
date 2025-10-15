using UnityEngine;

public class EnemyAITesting : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject[] wayPoints;
    [SerializeField] private Animator animator;
    private int nextWayPoint = 1;
    private float distToPoint;
    private bool isFacingRight = true;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
        UpdateAnimation();
    }

    private void Move()
    {
        distToPoint = Vector2.Distance(transform.position, wayPoints[nextWayPoint].transform.position);
        transform.position = Vector2.MoveTowards(transform.position, wayPoints[nextWayPoint].transform.position, moveSpeed * Time.deltaTime);
        FlipSprite();
        if (distToPoint < 0.2f)
        {
            TakeTurn();
        }
    }

    private void TakeTurn()
    {
        Vector3 currRot = transform.eulerAngles;
        currRot.z += wayPoints[nextWayPoint].transform.eulerAngles.z;
        transform.eulerAngles = currRot;
        NextWayPoint();
    }

    private void NextWayPoint()
    {
        nextWayPoint++;
        if (nextWayPoint == wayPoints.Length)
        {
            nextWayPoint = 0;
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        // Simple: Set speed to 1 when moving, 0 when idle
        float blendValue = distToPoint > 0.2f ? 1f : 0f;
        animator.SetFloat("Speed", blendValue);
    }

    private void FlipSprite()
    {
        if (wayPoints[nextWayPoint].transform.position.x > transform.position.x && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (wayPoints[nextWayPoint].transform.position.x < transform.position.x && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
