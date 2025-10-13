using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private int score = 10;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ScoreManager.Instance.UpdateScore(score);
            AudioManager.Instance.PlaySound("PickUp");
            Destroy(gameObject);
        }
    }
}
