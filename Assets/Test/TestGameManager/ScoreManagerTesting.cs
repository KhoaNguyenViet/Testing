using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManagerTesting : MonoBehaviour
{
    public static ScoreManagerTesting Instance { get; private set; }

    private int currentScore = 0;
    private int totalScore = 0;
    private int levelScore = 0;

    [Header("UI References")]
    [SerializeField] private Text scoreText;

    public int CurrentScore => currentScore;
    public int TotalScore => totalScore;
    public int LevelScore => levelScore;

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
            return;
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
        FindScoreText();
    }

    private void FindScoreText()
    {
        if (scoreText == null)
        {
            GameObject scoreObj = GameObject.FindGameObjectWithTag("ScoreText");
            if (scoreObj != null)
            {
                scoreText = scoreObj.GetComponent<Text>();
            }
        }
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    public void UpdateScore(int amount)
    {
        currentScore += amount;
        levelScore += amount;
        totalScore += amount;
        UpdateScoreDisplay();
    }

    public void ResetLevelScore()
    {
        currentScore -= levelScore;
        levelScore = 0;
        UpdateScoreDisplay();
    }

    public void ResetAllScores()
    {
        currentScore = 0;
        totalScore = 0;
        levelScore = 0;
        UpdateScoreDisplay();
    }

    public void ResetLevelScoreOnly()
    {
        levelScore = 0;
        UpdateScoreDisplay();
    }
}
