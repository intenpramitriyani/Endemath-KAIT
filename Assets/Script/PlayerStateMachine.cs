using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        DEAD
    }

    public TurnState currentState; //for the ProgressBar

    private float cur_cooldown = 0f;
    private float max_cooldown = 5f;
    public Image ProgressBar;
    public GameObject Selector;

    // Start is called before the first frame update
    void Start()
    {
        cur_cooldown = Random.Range(0, 2.5f);
        Selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        currentState = TurnState.PROCESSING;
        // Pastikan progress bar diinisialisasi dengan skala yang benar
        ProgressBar.transform.localScale = new Vector3(0, ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentState);
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

                break;
            case TurnState.ACTION:
                break;
            case TurnState.DEAD:
                break;
        }
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
}
