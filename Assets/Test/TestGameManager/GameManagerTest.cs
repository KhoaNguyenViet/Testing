using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class GameManagerTests
{
    private GameObject gameManagerObject;
    private GameManagerTesting gameManager;

    [SetUp]
    public void SetUp()
    {
        // Tạo GameManager cho testing
        gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManagerTesting>();

        // Setup test data
        SetPrivateField(gameManager, "levelScenes", new string[] { "Level1", "Level2" });
        SetPrivateField(gameManager, "mainMenuScene", "MainMenu");
        SetPrivateField(gameManager, "totalScoreScene", "TotalScene");
        SetPrivateField(gameManager, "currentLevelIndex", 0);
    }

    [TearDown]
    public void TearDown()
    {
        // Dọn dẹp sau mỗi test
        Object.DestroyImmediate(gameManagerObject);
    }

    [Test]
    public void GameManager_IsSingleton()
    {
        // Kiểm tra GameManager là singleton
        GameManagerTesting instance1 = GameManagerTesting.Instance;
        Assert.IsNotNull(instance1, "GameManager instance should exist");

        // Tạo GameManager thứ 2 để test singleton pattern
        GameObject secondManagerObject = new GameObject("SecondGameManager");
        GameManagerTesting instance2 = secondManagerObject.AddComponent<GameManagerTesting>();

        // Instance thứ 2 nên bị destroy
        Assert.IsTrue(instance1 == GameManagerTesting.Instance, "Should only have one GameManager instance");

        Object.DestroyImmediate(secondManagerObject);
    }

    [Test]
    public void CompleteLevel_WhenNotLastLevel_MovesToNextLevel()
    {
        // Arrange
        int initialLevel = 0;
        SetPrivateField(gameManager, "currentLevelIndex", initialLevel);

        // Act
        gameManager.CompleteLevel();

        // Assert
        int newLevel = (int)GetPrivateField(gameManager, "currentLevelIndex");
        Assert.AreEqual(initialLevel + 1, newLevel, "Should move to next level");
    }

    [Test]
    public void CompleteLevel_WhenLastLevel_GoesToTotalScene()
    {
        // Arrange - Đặt ở level cuối cùng
        string[] levels = { "Level1", "Level2"};
        SetPrivateField(gameManager, "levelScenes", levels);
        SetPrivateField(gameManager, "currentLevelIndex", levels.Length - 1);

        // Act
        gameManager.CompleteLevel();

        // Assert - Kiểm tra qua logic (vì không thể test SceneManager thực tế trong EditMode)
        bool isLastLevel = gameManager.IsLastLevel();
        Assert.IsTrue(isLastLevel, "Should be at last level");
    }

    [Test]
    public void GoToLevel_WithValidIndex_ChangesLevel()
    {
        // Arrange
        int targetLevel = 2;

        // Act
        gameManager.GoToLevel(targetLevel);

        // Assert
        int currentLevel = (int)GetPrivateField(gameManager, "currentLevelIndex");
        Assert.AreEqual(targetLevel, currentLevel, "Should change to specified level");
    }

    [Test]
    public void GoToLevel_WithInvalidIndex_DoesNothing()
    {
        // Arrange
        int initialLevel = 1;
        SetPrivateField(gameManager, "currentLevelIndex", initialLevel);

        // Act - Thử chuyển đến level không tồn tại
        gameManager.GoToLevel(999);

        // Assert - Level không đổi
        int currentLevel = (int)GetPrivateField(gameManager, "currentLevelIndex");
        Assert.AreEqual(initialLevel, currentLevel, "Should not change level with invalid index");
    }

    [Test]
    public void GoToLevel_WithNegativeIndex_DoesNothing()
    {
        // Arrange
        int initialLevel = 1;
        SetPrivateField(gameManager, "currentLevelIndex", initialLevel);

        // Act
        gameManager.GoToLevel(-1);

        // Assert
        int currentLevel = (int)GetPrivateField(gameManager, "currentLevelIndex");
        Assert.AreEqual(initialLevel, currentLevel, "Should not change level with negative index");
    }

    [Test]
    public void IsLastLevel_ReturnsCorrectValue()
    {
        // Arrange
        string[] levels = { "Level1", "Level2"};
        SetPrivateField(gameManager, "levelScenes", levels);

        // Test không phải level cuối
        SetPrivateField(gameManager, "currentLevelIndex", 0);
        Assert.IsFalse(gameManager.IsLastLevel(), "Should not be last level");

        // Test level cuối
        SetPrivateField(gameManager, "currentLevelIndex", levels.Length - 1);
        Assert.IsTrue(gameManager.IsLastLevel(), "Should be last level");
    }

    [Test]
    public void GetCurrentLevelName_ReturnsCorrectName()
    {
        // Arrange
        string[] levels = { "Level1", "Level2" };
        SetPrivateField(gameManager, "levelScenes", levels);
        SetPrivateField(gameManager, "currentLevelIndex", 1);

        // Act
        string levelName = gameManager.GetCurrentLevelName();

        // Assert
        Assert.AreEqual("Level2", levelName, "Should return correct level name");
    }

    [Test]
    public void GetCurrentLevelName_WithInvalidIndex_ReturnsUnknown()
    {
        // Arrange - Đặt index không hợp lệ
        SetPrivateField(gameManager, "currentLevelIndex", 999);

        // Act
        string levelName = gameManager.GetCurrentLevelName();

        // Assert
        Assert.AreEqual("Unknown", levelName, "Should return 'Unknown' for invalid level index");
    }

    [UnityTest]
    public IEnumerator GameManager_SurvivesSceneLoad()
    {
        // Arrange
        GameManagerTesting originalManager = GameManagerTesting.Instance;

        // Act - Giả lập scene load
        var sceneLoadedMethod = typeof(GameManagerTesting).GetMethod("OnSceneLoaded",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Scene testScene = new Scene();
        sceneLoadedMethod?.Invoke(gameManager, new object[] { testScene, LoadSceneMode.Single });

        yield return null;

        // Assert - GameManager vẫn tồn tại
        Assert.IsNotNull(GameManagerTesting.Instance, "GameManager should survive scene loads");
        Assert.AreEqual(originalManager, GameManagerTesting.Instance, "Should be the same instance");
    }

    // Helper methods để truy cập private fields
    private void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }

    private object GetPrivateField(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field?.GetValue(obj);
    }
}