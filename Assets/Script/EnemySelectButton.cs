using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YourGameNamespace;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject EnemyPrefab; // Ini adalah prefab musuh yang diwakili oleh tombol ini
    
    // Method untuk menangani pemilihan musuh
    public void SelectEnemy()
    {
        // Temukan BattleStateMachine di dalam BattleManager
        BattleStateMachine BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();

        if (BSM != null)
        {
            // Panggil metode untuk memilih musuh pada BSM dengan prefab musuh
            BSM.Input2(EnemyPrefab);
            Debug.Log("Enemy selected: " + EnemyPrefab.name);
        }
        else
        {
            Debug.LogError("BattleManager tidak ditemukan. Pastikan ada GameObject bernama 'BattleManager' dengan komponen BattleStateMachine di dalam scene.");
        }

        MathQuestionPanel.SetActive(true);
    }

    public GameObject MathQuestionPanel;

    public Button attackButton;

    void Start()
    {
        attackButton.onClick.AddListener(OnAttackButtonClicked);
    }

    void OnAttackButtonClicked()
    {
        MathQuestionPanel.SetActive(true);
     
    }
    
}
