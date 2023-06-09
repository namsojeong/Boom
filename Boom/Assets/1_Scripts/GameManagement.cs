using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public static GameManagement Instance = null;

    [Header("UI")]
    private TextMeshProUGUI scoreText;
    private Transform canvas;

    [Header("Object")]
    private Food food;
    private GameObject goal;
    private Enemy enemy;
    private Enemy player;

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

        canvas = transform.Find("Canvas");
        scoreText = canvas.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        player = transform.Find("Player").GetComponent<Enemy>();
        enemy = transform.Find("Enemy").GetComponent<Enemy>();
        goal = transform.Find("Goal").gameObject;
        food = transform.Find("Food").GetComponent<Food>();

        enemySpawnPos = enemy.transform;
        playerSpawnPos = player.transform;

        goalSpawnList = transform.Find("GoalRandomPos").GetComponentsInChildren<Transform>();
        foodSpawnList = transform.Find("FoodRandomPos").GetComponentsInChildren<Transform>();
    }

    public void ResetGame()
    {
        SettingGame();

        playerScore = 0;
        enemyScore = 0;

        UpdateScoreUI();
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
    private const int MAX_SCORE = 3;
    public void AddPlayerScore(int addScore)
    {
        playerScore += addScore;
        UpdateScoreUI();
    }
    public void AddEnemyScore(int addScore)
    {
        enemyScore += addScore;
        UpdateScoreUI();
    }

    private void GameOver()
    {
        enemy.EndEpisode();
    }

    private void UpdateScoreUI()
    {
        // UI Update
        string scoreString = playerScore.ToString() + " VS " + enemyScore.ToString();
        scoreText.text = string.Format(scoreString);

        if (playerScore >= MAX_SCORE)
        {
            GameOver();
        }
        else if (enemyScore >= MAX_SCORE)
        {
            GameOver();
        }
        else
        {
            SettingGame();
        }
    }
    #endregion
}
