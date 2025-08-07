using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI abilitiesText;
    [SerializeField] TextMeshProUGUI currentTimeText;
    [SerializeField] TextMeshProUGUI bestTimeText;
    [SerializeField] int score;
    public static bool hasDash = false;
    public static bool hasDoubleJump = false;
    [SerializeField] int scoreForLife;
    int scoreThreshold = 2500;
    float timer = 0f;
    bool isTiming = true;
    float bestTime;
    string levelKey;
    string currentLevelName;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Awake()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        livesText.text = "Lives: " + playerLives.ToString();
        scoreText.text = "Score: " + score.ToString();
        abilitiesText.text = "Abilities: \n" + "Dash: " + hasDash.ToString() + "\n" + "Double Jump: " + hasDoubleJump.ToString();
        InitializeTimerForLevel();
    }

    void Update()
    {
        if (!isTiming)
        {
            return;
        }
        timer += Time.deltaTime;
        if (currentTimeText != null)
        {
            currentTimeText.text = "Time: " + timer.ToString("F2") + "s";
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeTimerForLevel();
    }

    public void PlayerDeath()
    {
        if (playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            ResetGameSession();
        }
    }

    public void addScore(int addPoints)
    {
        score += addPoints;
        scoreForLife += addPoints;
        scoreText.text = "Score: " + score.ToString();
        while (scoreForLife >= scoreThreshold)
        {
            playerLives += 1;
            livesText.text = "Lives: " + playerLives.ToString();
            scoreForLife -= scoreThreshold;
        }
    }

    void ResetGameSession()
    {
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    void TakeLife()
    {
        playerLives -= 1;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        livesText.text = "Lives: " + playerLives.ToString();
    }

    public void collectDash()
    {
        hasDash = true;
        abilitiesText.text = "Abilities: \n" + "Dash: " + hasDash.ToString() + "\n" + "Double Jump: " + hasDoubleJump.ToString();
    }

    public void collectDoubleJump()
    {
        hasDoubleJump = true;
        abilitiesText.text = "Abilities: \n" + "Dash: " + hasDash.ToString() + "\n" + "Double Jump: " + hasDoubleJump.ToString();
    }

    public void StopTimer()
    {
        isTiming = false;

        if (timer < bestTime)
        {
            bestTime = timer;
            PlayerPrefs.SetFloat(levelKey, bestTime);
            PlayerPrefs.Save();
            if (bestTimeText != null)
            {
                bestTimeText.text = "Best: " + bestTime.ToString("F2") + "s";
            }
        }
    }

    public float GetCurrentTime() => timer;
    public float GetBestTime() => bestTime;

    void InitializeTimerForLevel()
    {
        currentLevelName = SceneManager.GetActiveScene().name;
        levelKey = "BestTime_" + currentLevelName;

        timer = 0f;
        isTiming = true;

        bestTime = PlayerPrefs.GetFloat(levelKey, float.MaxValue);

        if (bestTimeText != null)
        {
            bestTimeText.text = bestTime == float.MaxValue ? "Best: --" : "Best: " + bestTime.ToString("F2") + "s";
        }
    }
}
