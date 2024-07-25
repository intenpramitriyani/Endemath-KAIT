using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YourGameNamespace;

public class MathQuizManager : MonoBehaviour
{
    public TMP_Text questionText;
    public TMP_InputField answerInput;
    public TMP_Text feedbackText;
    public Button submitButton;
    public float timeLimit = 20f;

    private int num1;
    private int num2;
    private int correctAnswer;
    private float timer;
    private bool isAnswered;
    private Action<bool> onAnswer;
    public event Action<bool> OnQuizCompleted;

    public GameObject QuizPanel;
    public BattleStateMachine battleStateMachine;

    void Start()
    {
        if (battleStateMachine != null)
        {
            battleStateMachine.AttackPanel.SetActive(false);
        }

        feedbackText.text = "";
        QuizPanel.SetActive(false);
        submitButton.onClick.AddListener(CheckAnswer);
    }

    public void StartQuiz(Action<bool> callback)
    {
        onAnswer = callback;
        QuizPanel.SetActive(true);

        if (battleStateMachine != null)
        {
            battleStateMachine.AttackPanel.SetActive(false);
        }

        GenerateQuestion();
        timer = timeLimit;
        isAnswered = false;

        StartCoroutine(QuizCoroutine());
    }

    void Update()
    {
        if (!isAnswered)
        {
            timer -= Time.deltaTime;
            if (timer <= 0) // Menggunakan <= untuk menghindari masalah perbandingan float
            {
                HandleTimeout();
            }
        }
    }

    void GenerateQuestion()
    {
        num1 = UnityEngine.Random.Range(1, 10);
        num2 = UnityEngine.Random.Range(1, 10);
        correctAnswer = num1 + num2;
        questionText.text = $"{num1} + {num2} = ?";
    }

    void CheckAnswer()
    {
        int playerAnswer;
        if (int.TryParse(answerInput.text, out playerAnswer))
        {
            bool isCorrect = playerAnswer == correctAnswer;
            feedbackText.text = isCorrect ? "Correct!" : "Incorrect!";
            isAnswered = true;

            onAnswer?.Invoke(isCorrect);
            OnQuizCompleted?.Invoke(isCorrect);

            EndQuiz();
        }
        else
        {
            feedbackText.text = "Invalid input!";
        }
    }

    void EndQuiz()
    {
        QuizPanel.SetActive(false);
        if (battleStateMachine != null)
        {
            battleStateMachine.AttackPanel.SetActive(true);
        }
    }

    void HandleTimeout()
    {
        feedbackText.text = "Time's up!";
        isAnswered = true;

        onAnswer?.Invoke(false);
        OnQuizCompleted?.Invoke(false);

        EndQuiz();
    }

    private IEnumerator QuizCoroutine()
    {
        yield return new WaitForSeconds(10f);

        if (!isAnswered)
        {
            HandleTimeout();
        }
    }
}
