using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YourGameNamespace
{
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

        public GameObject enemyButtonPrefab;
        public Transform enemyButtonParent;


        // Define the list of players to manage here
        public List<GameObject> PlayerToManage = new List<GameObject>();
        public List<GameObject> PlayerInBattle = new List<GameObject>();
        public List<GameObject> EnemyInBattle = new List<GameObject>();

        private HandleTurn PlayerChoice;

        public GameObject enemyButton;
        public Transform Spacer;

        public GameObject AttackPanel;
        public GameObject EnemySelectPanel;
        private GameObject selectedEnemy; // Add a field to store the selected enemy


        private int enemyTurnCounter = 0;

        public MathQuizManager mathQuizManager;

        public PlayerStateMachine playerStateMachine;
        public BattleStateMachine battleStateMachine;

        public List<GameObject> GetEnemyInBattle()
        {
            return EnemyInBattle;
        }

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
                playerStateMachine = player.GetComponent<PlayerStateMachine>();
                if (playerStateMachine == null)
                {
                    Debug.LogError("PlayerStateMachine component is missing on player.");
                }
            }


            /* playerStateMachine = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
             if (playerStateMachine == null)
             {
                 Debug.LogError("PlayerStateMachine is missing from Player.");
             }
            */

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0)
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

            if (PlayerToManage == null)
            {
                Debug.LogError("PlayerToManage list is null!");
            }
            else if (PlayerToManage.Count == 0)
            {
                Debug.LogError("PlayerToManage list is empty!");
            }
            else
            {
                Debug.Log("PlayerToManage initialized with " + PlayerToManage.Count + " players.");
            }

            if (PlayerToManage.Count > 0)
            {
                Debug.Log("PlayerToManage has players.");
            }
            else
            {
                Debug.LogError("PlayerToManage is empty!");
            }

            AttackPanel.SetActive(false);
            EnemySelectPanel.SetActive(false);
            EnemyButtons();
            PlayerInput = PlayerGUI.ACTIVATE;

            if (mathQuizManager != null)
            {
                mathQuizManager = GameObject.Find("MathQuizManager").GetComponent<MathQuizManager>();
            }
            else
            {
                Debug.LogError("MathQuizManager object not assigned!");
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
                    if (PerformList.Count > 0)
                    {
                        battleStates = PerformAction.TAKEACTION;
                    }
                    break;

                case PerformAction.TAKEACTION:
                    if (PerformList.Count > 0)
                    {
                        GameObject performer = GameObject.Find(PerformList[0].Attacker);
                        if (performer != null)
                        {
                            // Validasi tipe dan aksi
                            if (PerformList[0].Type == "Enemy")
                            {
                                EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                                if (ESM != null)
                                {
                                    ESM.PlayerToAttack = PerformList[0].AttackersTarget;
                                    ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                                }
                                else
                                {
                                    Debug.LogError("EnemyStateMachine component is missing on performer");
                                }
                            }
                            else if (PerformList[0].Type == "Player")
                            {
                                PlayerStateMachine PSM = performer.GetComponent<PlayerStateMachine>();
                                if (PSM != null)
                                {
                                    PSM.EnemyToAttack = PerformList[0].AttackersTarget;
                                    PSM.currentState = PlayerStateMachine.TurnState.ACTION;
                                    Debug.Log("Player is here to perform");

                                    // Start quiz
                                    mathQuizManager.StartQuiz(OnQuizCompleted);
                                    battleStates = PerformAction.WAIT;
                                }
                                else
                                {
                                    Debug.LogError("PlayerStateMachine component is missing on performer");
                                }
                            }
                            else
                            {
                                Debug.LogError("Performer type is incorrect");
                            }
                        }
                        else
                        {
                            Debug.LogError("Performer is null");
                        }

                        battleStates = PerformAction.PERFORMACTION;
                    }
                    else
                    {
                        Debug.LogError("PerformList is empty");
                    }
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
                    PlayerInputDone();
                    break;
            }
        }


        // Method to collect actions to be performed
        public void CollectActions(HandleTurn getInput)
        {
            if (getInput != null)
            {
                // Validasi bahwa `getInput` memiliki data yang benar
                if (string.IsNullOrEmpty(getInput.Attacker))
                {
                    Debug.LogError("Attacker is null or empty!");
                }
                else
                {
                    Debug.Log("Adding action for: " + getInput.Attacker);
                }

                if (getInput.AttackersTarget == null)
                {
                    Debug.LogError("AttackersTarget is null!");
                }
                else
                {
                    Debug.Log("Target for the action: " + getInput.AttackersTarget.name);
                }

                PerformList.Add(getInput); // Tambahkan aksi ke PerformList
                Debug.Log("Action collected from: " + getInput.Attacker);
            }
            else
            {
                Debug.LogWarning("getInput is null in CollectActions"); // Log warning jika getInput null
            }
        }


        void EnemyButtons()
        {
            // Bersihkan tombol musuh sebelumnya
            foreach (Transform child in enemyButtonParent)
            {
                Destroy(child.gameObject);
            }

            // Buat tombol musuh baru
            foreach (GameObject enemy in EnemyInBattle)
            {
                GameObject newButton = Instantiate(enemyButtonPrefab) as GameObject;
                newButton.transform.SetParent(enemyButtonParent, false);

                EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
                button.EnemyPrefab = enemy;

                Text buttonText = newButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = enemy.name;
                }
                else
                {
                    Debug.LogError("Text component not found on button.");
                }
            }
        }

        public void Input1() // Attack button
        {
            if (PlayerToManage.Count > 0)
            {
                // Set player choice
                PlayerChoice.Attacker = PlayerToManage[0].name;
                PlayerChoice.AttackersGameObject = PlayerToManage[0];
                PlayerChoice.Type = "Player";

                // Hide Attack Panel and show Enemy Select Panel
                AttackPanel.SetActive(false);
                EnemySelectPanel.SetActive(true); // Show enemy selection panel
            }
            else
            {
                Debug.LogError("PlayerToManage is empty or null!");
            }
        }

        // Method to handle enemy selection
        /* public void SetSelectedEnemy(GameObject enemy)
        {
            selectedEnemy = enemy; public void OnPlayerAttack()
            {
                // Mulai kuiz matematika dengan callback
                if (mathQuizManager != null)
                {
                    mathQuizManager.StartQuiz(HandleQuizResult);
                }
                else
                {
                    Debug.LogError("MathQuizManager is not assigned!");
                }
            }
            Debug.Log("Selected enemy: " + enemy.name);
        } */

        public void OnEnemySelected(GameObject enemy)
        {
            // Set the selected enemy
            selectedEnemy = enemy;

            // Hide Enemy Select Panel and show Math Quiz Panel
            EnemySelectPanel.SetActive(false);
            ShowMathQuiz();
        }

        public void OnPlayerAttack()
        {
            // Mulai kuiz matematika dengan callback
            if (mathQuizManager != null)
            {
                mathQuizManager.StartQuiz(OnQuizCompleted);
            }
            else
            {
                Debug.LogError("MathQuizManager is not assigned!");
            }
        }

        public GameObject MathQuizPanel; // Panel untuk kuiz matematika

        void ShowMathQuiz()
        {
            Debug.Log("ShowMathQuiz called");
            if (mathQuizManager != null)
            {
                mathQuizManager.StartQuiz(OnQuizCompleted);
                MathQuizPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("MathQuizManager component is missing!");
            }
        }

        public void Input2(GameObject choosenEnemy) // Enemy selection
        {
            PlayerChoice.AttackersTarget = choosenEnemy;
            PlayerInput = PlayerGUI.DONE;
        }

        void PlayerInputDone()
        {
            // Validasi sebelum menambahkan ke PerformList
            if (PlayerChoice != null)
            {
                if (string.IsNullOrEmpty(PlayerChoice.Attacker))
                {
                    Debug.LogError("PlayerChoice.Attacker is null or empty!");
                }
                if (PlayerChoice.AttackersTarget == null)
                {
                    Debug.LogError("PlayerChoice.AttackersTarget is null!");
                }
                else
                {
                    PerformList.Add(PlayerChoice);
                    Debug.Log("Action added to PerformList from PlayerChoice");
                }
            }
            else
            {
                Debug.LogError("PlayerChoice is null!");
            }

            EnemySelectPanel.SetActive(false);
            PlayerToManage[0].transform.Find("Selector").gameObject.SetActive(false);
            PlayerToManage.RemoveAt(0);
            PlayerInput = PlayerGUI.ACTIVATE;
        }

        public void ClearAttackPanel()
        {
            EnemySelectPanel.SetActive(false);
            AttackPanel.SetActive(false);
        }

        public void ShiftEnemyTurn(int shiftAmount)
        {
            enemyTurnCounter += shiftAmount;

            if (enemyTurnCounter < 0)
            {
                enemyTurnCounter = 0;
            }

            Debug.Log("Enemy turn counter: " + enemyTurnCounter);
        }

        private bool hasAnsweredCorrectly = false;

        void OnQuizCompleted(bool isCorrect)
        {
            hasAnsweredCorrectly = isCorrect;

            if (isCorrect)
            {
                Debug.Log("Answer is correct!");
                if (playerStateMachine != null)
                {
                    playerStateMachine.HandlePlayerAction();
                }
                else
                {
                    Debug.LogError("PlayerStateMachine is not assigned!");
                }

                ShiftEnemyTurn(-1);
            }
            else
            {
                Debug.Log("Answer is incorrect!");

                // Allow player to attack regardless
                if (playerStateMachine != null)
                {
                    playerStateMachine.HandlePlayerAction();
                }
                else
                {
                    Debug.LogError("PlayerStateMachine is not assigned!");
                }

                ShiftEnemyTurn(1);
            }

            battleStates = PerformAction.PERFORMACTION;
        }

        public void StartEnemyTurn()
        {
            foreach (var enemy in EnemyInBattle)
            {
                EnemyStateMachine enemySM = enemy.GetComponent<EnemyStateMachine>();
                if (enemySM != null)
                {
                    enemySM.HandleEnemyAction();
                }

                else
                {
                    Debug.LogError("EnemyStateMachine component is missing on enemy: " + enemy.name);
                }
            }
            battleStates = PerformAction.PERFORMACTION;
        }

    }
}
