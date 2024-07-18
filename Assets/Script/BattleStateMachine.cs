using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public enum PlayerGUI
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }

    public PlayerGUI PlayerInput;

    // Define the list of players to manage here
    public List<GameObject> PlayerToManage = new List<GameObject>();
    public List<GameObject> PlayerInBattle = new List<GameObject>();
    public List<GameObject> EnemyInBattle;
    
    private HandleTurn PlayerChoice;

    public GameObject enemyButton; 
    public Transform Spacer;

    public GameObject AttackPanel;
    public GameObject EnemySelectPanel;
    private GameObject selectedEnemy; // Add a field to store the selected enemy
    
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

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemy == null)
        {
            Debug.LogError("Enemy not found. Make sure there is a GameObject with the tag 'Enemy' in the scene.");
        }
        else
        {
            foreach (GameObject enemy in enemies)
            {
                Debug.Log("Enemy found: " + enemy.name);
                EnemyInBattle.Add(enemy);
            }
        }
        
        PlayerInput = PlayerGUI.ACTIVATE;

        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);
        EnemyButtons();

    }

    // Update is called once per frame
    void Update()
    {
        // Manage battle state based on current battleStates
        switch (battleStates)
        {
            case PerformAction.WAIT:
                // Idle state, waiting for input
                if (PerformList.Count > 0)
                {
                    battleStates = PerformAction.TAKEACTION;
                }
                break;

            case PerformAction.TAKEACTION:
                // Logic for taking action
                GameObject performer = GameObject.Find(PerformList[0].Attacker);
                if (PerformList[0].Type == "Enemy")
                {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    ESM.PlayerToAttack = PerformList[0].AttackersTarget;
                    ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                }

                if (PerformList[0].Type == "Player")
                {
                    // Add player action logic here

                    Debug.Log("Hero is here to perform");
                }

                battleStates = PerformAction.PERFORMACTION;

                break;

            case PerformAction.PERFORMACTION:
                // Logic for performing action
                break;
        }

        switch (PlayerInput)
        {
            case (PlayerGUI.ACTIVATE):
                if (PlayerToManage.Count > 0)
                {
                    PlayerToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    PlayerChoice = new HandleTurn();

                    AttackPanel.SetActive(true);
                    PlayerInput = PlayerGUI.WAITING;
                }

                break;

            case (PlayerGUI.WAITING):
                break;

            case (PlayerGUI.INPUT1):
                break;

            case (PlayerGUI.INPUT2):
                break;

            case (PlayerGUI.DONE):
                PlayerInputDone ();
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

    void EnemyButtons()
    {
        foreach (GameObject enemy in EnemyInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
            button.EnemyPrefab = enemy; // Assign the enemy to the button

            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();

            Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
            if (buttonText!= null)
            {
                buttonText.text = cur_enemy.enemy.name;
                Debug.Log("Button text set to: " + cur_enemy.enemy.name);
            }

            else
            {
                Debug.LogError("Text component not found on button.");
            }
            

            button.EnemyPrefab = enemy;
            newButton.transform.SetParent(Spacer, false);

        }
    }

    public void Input1 () //attack button
    {
        PlayerChoice.Attacker = PlayerToManage[0].name;
        PlayerChoice.AttackersGameObject = PlayerToManage[0];
        PlayerChoice.Type = "Player";

        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    // Method to handle enemy selection

    public void SetSelectedEnemy(GameObject enemy)
        {
            selectedEnemy = enemy;
            Debug.Log("Selected enemy: " + enemy.name);
        }

        public void Input2(GameObject choosenEnemy) // enemy Selection
        {
            PlayerChoice.AttackersTarget = choosenEnemy;
            PlayerInput = PlayerGUI.DONE;
            // No need to return anything
        }


    void PlayerInputDone ()
    {
        PerformList.Add(PlayerChoice);
        EnemySelectPanel.SetActive(false);
        PlayerToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        PlayerToManage.RemoveAt(0);
        PlayerInput = PlayerGUI.ACTIVATE;
    }

}
