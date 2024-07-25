using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YourGameNamespace;

public class PlayerStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    public BasePlayer player;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        SELECTING,
        WAITING,
        ACTION,
        DEAD,
        QUIZ
    }

    public TurnState currentState; // for the ProgressBar

    private float cur_cooldown = 0f;
    private float max_cooldown = 5f;
    public Image ProgressBar;
    public GameObject Selector;

    public GameObject EnemyToAttack;

    private bool actionStarted = false;

    private Vector3 startposition;
    private float animSpeed = 5f;
    public bool hasAnsweredCorrectly = false;

    // Start is called before the first frame update
    void Start()
    {
        startposition = transform.position;
        cur_cooldown = Random.Range(0, 2.5f);
        Selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        currentState = TurnState.PROCESSING;
        // Initialize progress bar scale
        ProgressBar.transform.localScale = new Vector3(0, ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case TurnState.PROCESSING:
                upgradeProgressBar();
                break;
            case TurnState.ADDTOLIST:
                BSM.PlayerToManage.Add(this.gameObject);
                currentState = TurnState.WAITING; // Transition to the next state
                break;
            case TurnState.WAITING:
                // Wait for action to be triggered
                break;
            case TurnState.QUIZ:
                BSM.mathQuizManager.StartQuiz(OnQuizCompleted);
                currentState = TurnState.WAITING;
                break;
            case TurnState.ACTION:
                StartCoroutine(TimeForAction());
                break;
            case TurnState.DEAD:
                // Handle player's death state
                break;
        }
    }

    void OnQuizCompleted(bool isCorrect)
    {
        if (isCorrect)
        {
            BSM.ShiftEnemyTurn(-1);
        }
        else
        {
            BSM.ShiftEnemyTurn(1);
        }
        currentState = TurnState.ACTION;
    }

    void upgradeProgressBar()
    {
        cur_cooldown += Time.deltaTime;
        float calc_cooldown = cur_cooldown / max_cooldown;
        ProgressBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0, 1), ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
        if (cur_cooldown >= max_cooldown)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // Animate the player towards the enemy to attack
        Vector3 enemyPosition = new Vector3(EnemyToAttack.transform.position.x - 1.5f, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z);
        while (MoveTowardsEnemy(enemyPosition))
        {
            yield return null;
        }

        // Wait a bit before attacking
        yield return new WaitForSeconds(0.5f);

        //

        // Perform attack logic here (e.g., dealing damage)
        currentState = TurnState.WAITING;
        currentState = TurnState.QUIZ;

        // Animate the player back to the start position
        while (MoveTowardsStart(startposition))
        {
            yield return null;
        }

        // Remove this player from the list in BSM
        BSM.PerformList.RemoveAt(0);
        // Reset BSM to waiting state
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
        // End coroutine
        actionStarted = false;
        // Reset player's cooldown and state
        cur_cooldown = 0f;
        currentState = TurnState.PROCESSING;
    }

    public void HandlePlayerAction()
    {

        if (hasAnsweredCorrectly)
        {
            Attack();
        }
        else
        {
            Debug.Log("IncorrectAnswer, Player can't attack the enemy");
        }
        NextTurn();
    }

private void Attack()
{
    Debug.Log("Player attacks Enemy!");
    // Here you would implement your attack logic, such as dealing damage.
}

private void NextTurn()
{
    Debug.Log("Player turn ends. Waiting for Enemy.");
    BattleStateMachine battleSM = FindObjectOfType<BattleStateMachine>();

    if (battleSM != null)
    {
        battleSM.StartEnemyTurn();
    }
}

    bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
}