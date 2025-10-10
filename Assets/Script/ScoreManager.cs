using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private int currentScore = 0;
    private int totalScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void UpdateScore(int amount)
    {
        currentScore += amount;
        Debug.Log("Score:" + currentScore);
        //if (scoreText == null)
        //{
        //    scoreText = GameObject.Find(SCORE_AMOUNT_TEXT).GetComponent<TMP_Text>();
        //}

        //scoreText.text = currentScore.ToString("D3");
    }
}
