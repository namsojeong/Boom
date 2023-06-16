using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagement : MonoBehaviour
{
    public static GameManagement Instance = null;

    [Header("UI")]
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;
    [SerializeField] GameObject startCanvas;

    private Transform scoreParent;
    private Transform[] playerScoreList;
    private Transform[] enemyScoreList;

    [Header("Object")]
    private Food food;
    private GameObject goal;
    private Enemy enemy;
    private PlayerController player;

    [Header("Pos")]
    private Transform[] goalSpawnList;
    private Transform[] foodSpawnList;
    private Transform playerSpawnPos;
    private Transform enemySpawnPos;

    public Transform GetPlayerTransform { get { return player.transform; } }
    public Transform GoalTransform { get { return goal.transform; } }
    public Transform FoodTransform { get { return food.transform; } }

    private void Awake()
    {
        Instance = this;
        startCanvas.SetActive(true);
        InitGame();
    }

    public void StartGame()
    {
        Time.timeScale = 1.0f;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        startCanvas.SetActive(false);

        ResetGame();
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    private void InitGame()
    {
        player = transform.Find("Player").GetComponent<PlayerController>();
        enemy = transform.Find("Enemy").GetComponent<Enemy>();

        goal = transform.Find("Goal").gameObject;
        food = transform.Find("Food").GetComponent<Food>();

        scoreParent = transform.Find("Score");
        playerScoreList = scoreParent.GetChild(0).GetComponentsInChildren<Transform>();
        enemyScoreList = scoreParent.GetChild(1).GetComponentsInChildren<Transform>();

        goalSpawnList = transform.Find("GoalRandomPos").GetComponentsInChildren<Transform>();
        foodSpawnList = transform.Find("FoodRandomPos").GetComponentsInChildren<Transform>();

        enemySpawnPos = enemy.transform;
        playerSpawnPos = player.transform;

        player.gameObject.SetActive(false);
        enemy.gameObject.SetActive(false);
    }
    private void ResetGame()
    {
        player.gameObject.SetActive(true);
        enemy.gameObject.SetActive(true);
        enemy.ResetEnemy();

        playerScore = 0;
        enemyScore = 0;

        SettingGame();
        InitScore();
    }

    private void SettingGame()
    {
        RandomPosObject(goal, goalSpawnList);
        RandomPosObject(food.gameObject, foodSpawnList);
        
        player.transform.position = playerSpawnPos.position;
        enemy.transform.position = enemySpawnPos.position;

        food.ResetObject();
        food.transform.SetParent(transform);
        food.gameObject.SetActive(true);
    }

    private void RandomPosObject(GameObject obj, Transform[] posList)
    {
        int randomPos = Random.Range(0, posList.Length);
        obj.transform.position = posList[randomPos].position;
    }

    #region Score
    private int playerScore = 0;
    private int enemyScore = 0;
    private const int MAX_SCORE = 5;
    private void InitScore()
    {
        for(int i=1;i<playerScoreList.Length;i++)
        {
            playerScoreList[i].gameObject.SetActive(false);
            enemyScoreList[i].gameObject.SetActive(false);
        }
    }
    public void AddPlayerScore(int addScore)
    {
        playerScore += addScore;
        playerScoreList[playerScore].gameObject.SetActive(true);
        UpdateScoreUI();
    }
    public void AddEnemyScore(int addScore)
    {
        enemyScore += addScore;
        enemyScoreList[enemyScore].gameObject.SetActive(true);
        UpdateScoreUI();
    }

    private void GameOver(bool isWin)
    {
        //enemy.EndEpisode();
        if (isWin) winPanel.gameObject.SetActive(true);
        else losePanel.gameObject.SetActive(true);

        Time.timeScale = 0;
    }

    private void UpdateScoreUI()
    {
        if (playerScore >= MAX_SCORE)
        {
            GameOver(true);
        }
        else if (enemyScore >= MAX_SCORE)
        {
            GameOver(false);
        }
        else
        {
            SettingGame();
        }
    }
    #endregion
}
