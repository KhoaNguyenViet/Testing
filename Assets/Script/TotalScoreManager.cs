using TMPro;
using UnityEngine;

public class TotalScoreManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text detailedScoreText;
    void Start()
    {
        DisplayTotalScores();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void DisplayTotalScores()
    {
        int totalScore = 0;
        int currentScore = 0;
        int levelScore = 0;

        // Cách 1: Tìm ScoreManager instance
        if (ScoreManager.Instance != null)
        {
            totalScore = ScoreManager.Instance.TotalScore;
            currentScore = ScoreManager.Instance.CurrentScore;
            levelScore = ScoreManager.Instance.LevelScore;
            Debug.Log("ScoreManager found! Using instance data.");
        }
        // Cách 2: Fallback - Load từ PlayerPrefs
        else if (PlayerPrefs.HasKey("TotalScore"))
        {
            totalScore = PlayerPrefs.GetInt("TotalScore", 0);
            currentScore = PlayerPrefs.GetInt("CurrentScore", 0);
            levelScore = PlayerPrefs.GetInt("LevelScore", 0);
            Debug.Log("ScoreManager not found! Using PlayerPrefs data.");
        }
        if (totalScoreText != null)
        {
            totalScoreText.text = $"TOTAL SCORE: {totalScore}";
        }

        if (detailedScoreText != null)
        {
            detailedScoreText.text =
                $"Final Score: {currentScore}\n" +
                $"Level Score: {levelScore}\n" ;
               // $"Rank: {GetRank(totalScore)}";
        }

        Debug.Log($"Displaying scores - Total: {totalScore}, Current: {currentScore}");
    }
}
