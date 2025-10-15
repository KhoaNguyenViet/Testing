using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneLoader
{
    void LoadScene(string sceneName);
}

public class DefaultSceneLoader : ISceneLoader
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

public class GameManagerTesting : MonoBehaviour
{
    public static GameManagerTesting Instance { get; private set; }

    [Header("Level Management")]
    [SerializeField] private string[] levelScenes;
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string totalScoreScene = "TotalScene";

    private int currentLevelIndex = 0;
    private ISceneLoader sceneLoader = new DefaultSceneLoader();

    public void SetSceneLoader(ISceneLoader loader)
    {
        sceneLoader = loader;
    }

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
        for (int i = 0; i < levelScenes.Length; i++)
        {
            if (levelScenes[i] == scene.name)
            {
                currentLevelIndex = i;
                break;
            }
        }
    }

    public void CompleteLevel()
    {
        if (currentLevelIndex < levelScenes.Length - 1)
        {
            currentLevelIndex++;
            string nextScene = levelScenes[currentLevelIndex];
            ScoreManagerTesting.Instance?.ResetLevelScoreOnly();
            sceneLoader.LoadScene(nextScene);
        }
        else
        {
            GoToTotalScoreScene();
        }
    }

    public void GoToTotalScoreScene()
    {
        if (!string.IsNullOrEmpty(totalScoreScene))
            sceneLoader.LoadScene(totalScoreScene);
    }

    public void GoToLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelScenes.Length)
        {
            currentLevelIndex = levelIndex;
            ScoreManagerTesting.Instance?.ResetLevelScoreOnly();
            sceneLoader.LoadScene(levelScenes[levelIndex]);
        }
    }

    public void ReturnToMainMenu()
    {
        sceneLoader.LoadScene(mainMenuScene);
    }

    public void RestartLevel()
    {
        ScoreManagerTesting.Instance?.ResetLevelScore();
        sceneLoader.LoadScene(levelScenes[currentLevelIndex]);
    }

    public int GetCurrentLevelIndex() => currentLevelIndex;

    public string GetCurrentLevelName()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < levelScenes.Length)
            return levelScenes[currentLevelIndex];
        return "Unknown";
    }

    public bool IsLastLevel() => currentLevelIndex >= levelScenes.Length - 1;
}
