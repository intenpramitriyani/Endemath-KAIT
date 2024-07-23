using System.Collections;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject EnemyPrefab;

    // Method to handle enemy selection
    public void SelectEnemy()
    {
        BattleStateMachine BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        if (BSM != null)
        {
            BSM.Input2(EnemyPrefab); //Call input2 on BSM instance
            BSM.SetSelectedEnemy(EnemyPrefab); // Save the selected enemy prefab
        }
        else
        {
            Debug.LogError("BattleManager not found. Make sure there is a GameObject named 'BattleManager' with the BattleStateMachine component in the scene.");
        }
    }
}