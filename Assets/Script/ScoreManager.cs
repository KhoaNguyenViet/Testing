using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int currentScore = 0;
    private int totalScore = 0;
    private int levelScore = 0;

    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;

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

    private void Start()
    {
        FindScoreText();
        SaveToPlayerPrefs();

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
        Debug.Log($"Scene loaded: {scene.name}");
        FindScoreText();

        // Log để debug
        Debug.Log($"Score State - Current: {currentScore}, Level: {levelScore}, Total: {totalScore}");
    }
    private void FindScoreText()
    {
        if (scoreText == null)
        {
            GameObject scoreObj = GameObject.FindGameObjectWithTag("ScoreText");
            if (scoreObj != null)
            {
                scoreText = scoreObj.GetComponent<TMP_Text>();
            }
        }
        UpdateScoreDisplay();
       
    }
    private void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.SetInt("CurrentScore", currentScore);
        PlayerPrefs.SetInt("LevelScore", levelScore);
        PlayerPrefs.Save();
    }
    public void UpdateScore(int amount)
    {
        currentScore += amount;
        levelScore += amount;
        totalScore += amount;

        Debug.Log($"Score: {currentScore} | Level Score: {levelScore} | Total Score: {totalScore}");
        UpdateScoreDisplay();
        SaveToPlayerPrefs();
    }
    
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    public void ResetLevelScore()
    {
        currentScore -= levelScore;
        levelScore = 0;
        UpdateScoreDisplay();
        SaveToPlayerPrefs();
    }

    public void ResetAllScores()
    {
        currentScore = 0;
        totalScore = 0;
        levelScore = 0;
        UpdateScoreDisplay();
        SaveToPlayerPrefs();
    }
    public void ResetLevelScoreOnly()
    {
        Debug.Log($"Resetting LevelScore only. Before - Level: {levelScore}, Current: {currentScore}, Total: {totalScore}");
        levelScore = 0;
        Debug.Log($"After reset - Level: {levelScore}, Current: {currentScore}, Total: {totalScore}");
        UpdateScoreDisplay();
    }
}
