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
    [SerializeField] int score;
    public static bool hasDash = false;
    public static bool hasDoubleJump = false;
    [SerializeField] int scoreForLife;
    int scoreThreshold = 2500;
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
}
