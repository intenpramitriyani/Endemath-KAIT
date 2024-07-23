using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM; // Reference to the BattleStateMachine
    public BaseEnemy enemy; // Reference to the enemy's data

    public enum TurnState
    {
        PROCESSING,  // State when the enemy is processing its turn
        CHOOSEACTION,  // State when the enemy is choosing its action
        SELECTING,  // State when the enemy is selecting a target
        WAITING,  // State when the enemy is waiting
        ACTION,  // State when the enemy is performing an action
        DEAD  // State when the enemy is dead
    }

    public TurnState currentState; // Current state of the enemy

    private float cur_cooldown = 0f; // Current cooldown time for actions
    private float max_cooldown = 5f; // Maximum cooldown time before taking action

    //timeforaction things
    private Vector3 startposition;
    private bool actionStarted = false;
    public GameObject PlayerToAttack;
    private float animSpeed = 5f;

    void Start()
    {
        currentState = TurnState.PROCESSING; // Initialize state to PROCESSING
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>(); // Find and get reference to BattleStateMachine
        if (BSM == null)
        {
            Debug.LogError("BattleStateMachine not found on 'BattleManager'. Make sure 'BattleManager' has the 'BattleStateMachine' component.");
        }

        startposition = transform.position; // Record starting position of the enemy
    }

    void Update()
    {
        switch (currentState)
        {
            case TurnState.PROCESSING:
                UpgradeProgressBar(); // Process cooldown progress
                break;
            case TurnState.WAITING:
                // Idle state, waiting for further action
                break;
            case TurnState.CHOOSEACTION:
                ChooseAction(); // Choose action to perform
                currentState = TurnState.WAITING; // Transition to waiting state
                break;
            case TurnState.ACTION:
                // Perform action logic here
                currentState = TurnState.WAITING; // Transition to waiting state after action
                StartCoroutine(TimeForAction());
                break;
            case TurnState.DEAD:
                // Handle death logic here
                break;
        }
    }

    void UpgradeProgressBar()
    {
        cur_cooldown += Time.deltaTime; // Increase cooldown progress over time

        if (cur_cooldown >= max_cooldown)
        {
            currentState = TurnState.CHOOSEACTION; // When cooldown reaches maximum, choose action
        }
    }

    void ChooseAction()
    {
        if (BSM.PlayerInBattle.Count > 0) // Check if PlayerInBattle is not empty
        {
            Debug.Log("Choosing action for enemy: " + enemy.name);
            HandleTurn myAttack = new HandleTurn(); // Create a new HandleTurn instance for the enemy's attack
            myAttack.Attacker = enemy.name; // Assign the attacker's name (enemy's name)
            myAttack.Type = "Enemy";
            myAttack.AttackersGameObject = this.gameObject; // Assign the attacker's GameObject (this enemy)
            myAttack.AttackersTarget = BSM.PlayerInBattle[Random.Range(0, BSM.PlayerInBattle.Count)];
            Debug.Log("Target chosen: " + myAttack.AttackersTarget.name);
            BSM.CollectActions(myAttack);
        }
        else
        {
            Debug.LogError("No players available to target in PlayerInBattle.");
        }
    }

    private IEnumerator TimeForAction()
    {
        Debug.Log("TimeForAction started");
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //animate the enemy near the hero to attack
        Vector3 PlayerPosition = new Vector3(PlayerToAttack.transform.position.x + 1.5f, PlayerToAttack.transform.position.y, PlayerToAttack.transform.position.z);
        while (MoveTowardsEnemy(PlayerPosition))
        {
            yield return null;
        }

        //wait a bit
        yield return new WaitForSeconds(0.5f);

        //do damage

        //animate back to startposition
        Vector3 firstPosition = startposition;
        while (MoveTowardsStart(firstPosition))
        {
            yield return null;
        }

        //remove this performer from the list in BSM
        BSM.PerformList.RemoveAt(0);
        //reset BSM (wait)
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
        //end coroutine
        actionStarted = false;
        //reset this enemy state
        cur_cooldown = 0f;
        currentState = TurnState.PROCESSING;
    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
}
