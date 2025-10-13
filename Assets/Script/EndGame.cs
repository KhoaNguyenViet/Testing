using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private string totalSceneName = "TotalScene";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CompleteGame();
        }
    }

    private void CompleteGame()
    {
        Debug.Log("=== END GAME TRIGGERED ===");
        Debug.Log($"Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");

        if (GameManager.Instance != null)
        {
            Debug.Log($"GameManager current level: {GameManager.Instance.GetCurrentLevelIndex()}");
            Debug.Log($"Is last level: {GameManager.Instance.IsLastLevel()}");
            GameManager.Instance.CompleteLevel();
        }
        else
        {
            Debug.LogError("GameManager Instance is null!");
            // Fallback: load scene trực tiếp
            UnityEngine.SceneManagement.SceneManager.LoadScene(totalSceneName);
        }
    }
}
