using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Management")]
    [SerializeField] private string[] levelScenes;
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string totalScoreScene = "TotalScene"; // Thêm scene tổng kết

    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Tìm index của scene hiện tại trong levelScenes
        for (int i = 0; i < levelScenes.Length; i++)
        {
            if (levelScenes[i] == scene.name)
            {
                currentLevelIndex = i;
                Debug.Log($"Current level index updated to: {currentLevelIndex} (Scene: {scene.name})");
                break;
            }
        }
    }
    // Hoàn thành level hiện tại
    public void CompleteLevel()
    {
        // KHÔNG gọi SaveLevelScore vì đã tách riêng
        // ScoreManager sẽ tự lưu khi update score

        // Chuyển đến level tiếp theo hoặc scene tổng kết
        if (currentLevelIndex < levelScenes.Length - 1)
        {
            currentLevelIndex++;
            string nextScene = levelScenes[currentLevelIndex];
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.ResetLevelScoreOnly(); // Chỉ reset levelScore
            }

            SceneManager.LoadScene(nextScene);
        }
        else
        {
            // Đã hoàn thành tất cả levels - chuyển thẳng đến scene tổng kết
            GoToTotalScoreScene();
        }
    }

    // Chuyển đến scene tổng kết
    public void GoToTotalScoreScene()
    {
        if (!string.IsNullOrEmpty(totalScoreScene))
        {
            SceneManager.LoadScene(totalScoreScene);
        }
        else
        {
            Debug.LogError("TotalScoreScene name is not set!");
        }
    }

    // Chuyển đến level cụ thể
    public void GoToLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelScenes.Length)
        {
            currentLevelIndex = levelIndex;
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.ResetLevelScoreOnly(); // Chỉ reset levelScore
            }

            SceneManager.LoadScene(levelScenes[levelIndex]);
        }
    }

    // Quay về main menu
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);

        // Reset tất cả điểm khi về menu (tuỳ chọn)
        // ScoreManager.Instance?.ResetAllScores();
    }

    // Game over
    public void GameOver()
    {
        // Có thể chuyển đến scene game over hoặc tổng kết
        GoToTotalScoreScene();
    }

    // Restart level hiện tại
    public void RestartLevel()
    {
        ScoreManager.Instance?.ResetLevelScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Lấy level hiện tại
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    // Lấy tên level hiện tại
    public string GetCurrentLevelName()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < levelScenes.Length)
        {
            return levelScenes[currentLevelIndex];
        }
        return "Unknown";
    }

    // Kiểm tra có phải level cuối không
    public bool IsLastLevel()
    {
        return currentLevelIndex >= levelScenes.Length - 1;
    }
}
