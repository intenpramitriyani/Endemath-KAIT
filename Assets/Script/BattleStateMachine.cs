using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMachine : MonoBehaviour
{
    // Enum to manage battle states
    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION
    }

    public PerformAction battleStates; // Current battle state

    public List<HandleTurn> PerformList = new List<HandleTurn>(); // List of actions to be performed

    public GameObject player; // Single player GameObject
    public GameObject enemy; // Single enemy GameObject

    // Start is called before the first frame update
    void Start()
    {
        battleStates = PerformAction.WAIT; // Set initial battle state to WAIT

        // Find and assign player and enemy GameObjects based on tags
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found. Make sure there is a GameObject with the tag 'Player' in the scene.");
        }
        else
        {
            Debug.Log("Player found: " + player.name);
        }

        enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy == null)
        {
            Debug.LogError("Enemy not found. Make sure there is a GameObject with the tag 'Enemy' in the scene.");
        }
        else
        {
            Debug.Log("Enemy found: " + enemy.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Manage battle state based on current battleStates
        switch (battleStates)
        {
            case PerformAction.WAIT:
                // Idle state, waiting for input
                if (PerformList.Count > 0) // Corrected 'count' to 'Count'
                {
                    battleStates = PerformAction.TAKEACTION;
                }
                break;

            case PerformAction.TAKEACTION:
                // Logic for taking action
                break;

            case PerformAction.PERFORMACTION:
                // Logic for performing action
                break;
        }
    }

    // Method to collect actions to be performed
    public void CollectActions(HandleTurn getInput)
    {
        if (getInput != null)
        {
            PerformList.Add(getInput); // Add action to PerformList
            Debug.Log("Action collected from: " + getInput.Attacker);
        }
        else
        {
            Debug.LogWarning("getInput is null in CollectActions"); // Log warning if getInput is null
        }
    }
}
