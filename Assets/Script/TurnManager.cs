using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public MathQuizManager mathQuizManager;
    public GameObject player;
    public GameObject enemy;

    private int enemyTurnCounter = 0;
    private List<HandleTurn> turnList = new List<HandleTurn>();

    void Start()
    {
        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        mathQuizManager.StartQuiz(OnQuizCompleted);
    }

    void OnQuizCompleted(bool isCorrect)
    {
        if (isCorrect)
        {
            enemyTurnCounter--;
            PlayerAttack();
        }
        else
        {
            enemyTurnCounter++;
            EnemyAdvanceTurn();
            PlayerAttack();
        }
    }

    void PlayerAttack()
    {
        HandleTurn playerTurn = new HandleTurn
        {
            Attacker = "Player",
            Type = "Attack",
            AttackersGameObject = player,
            AttackersTarget = enemy
        };

        turnList.Add(playerTurn);
        Debug.Log("Player attacks!");

        ProcessTurns();
    }

    void EnemyAdvanceTurn()
    {
        HandleTurn enemyAdvanceTurn = new HandleTurn
        {
            Attacker = "Enemy",
            Type = "AdvanceTurn",
            AttackersGameObject = enemy,
            AttackersTarget = player
        };

        turnList.Add(enemyAdvanceTurn);
        Debug.Log("Enemy advances turn!");
    }

    void ProcessTurns()
    {
        foreach (HandleTurn turn in turnList)
        {
            if (turn.Attacker == "Player")
            {
                // Logika penyerangan pemain
                Debug.Log("Processing player attack on " + turn.AttackersTarget.name);
            }
            else if (turn.Attacker == "Enemy")
            {
                // Logika penyerangan musuh
                Debug.Log("Processing enemy attack on " + turn.AttackersTarget.name);
            }
        }

        turnList.Clear();
        StartEnemyTurn();
    }

    void StartEnemyTurn()
    {
        enemyTurnCounter++;
        HandleTurn enemyTurn = new HandleTurn
        {
            Attacker = "Enemy",
            Type = "Attack",
            AttackersGameObject = enemy,
            AttackersTarget = player
        };

        turnList.Add(enemyTurn);
        Debug.Log("Enemy attacks!");

        StartPlayerTurn();
    }
}
