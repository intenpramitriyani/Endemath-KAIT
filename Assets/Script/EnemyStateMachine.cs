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

    private Vector3 startposition; // Starting position of the enemy
    private bool actionStarted = false;
    private GameObject PlayerToAttack;
    private float animSpeed = 5f;
    // Start is called before the first frame update
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

    // Update is called once per frame
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
        HandleTurn myAttack = new HandleTurn(); // Create a new HandleTurn instance for the enemy's attack
        myAttack.Attacker = enemy.name; // Assign the attacker's name (enemy's name)
        myAttack.AttackersGameObject = this.gameObject; // Assign the attacker's GameObject (this enemy)

        // Directly assign the player's GameObject as the target if it exists
        if (BSM.player != null)
        {
            myAttack.AttackersTarget = BSM.player; // Assign the player as the target
            BSM.CollectActions(myAttack); // Collect the action into BattleStateMachine's PerformList
        }
        else
        {
            Debug.LogError("Player reference in BSM is null. Cannot choose action."); // Log an error if player reference is null
        }
    }

    private IEnumerator TimeForAction ()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //animate the enemy near the hero to attack
        Vector3 PlayerPosition = new Vector3 (PlayerToAttack.transform.position.x+1.5f, PlayerToAttack.transform.position.y, PlayerToAttack.transform.position.z);
        while (MoveTowardsEnemy (PlayerPosition))
        {
            yield return null; 
        }

        //wait a bit
        //do damage

        //animate back to startposition

        //remove this performer from the list in BSM

        //reset BSM (wait)

        actionStarted = false;
        cur_cooldown = 0f;
        currentState = TurnState.PROCESSING;
    }

    private bool MoveTowardsEnemy (Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards (transform.position, target, animSpeed * Time.deltaTime));
    }
}
