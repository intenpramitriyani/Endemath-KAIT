using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MathQuizManager : MonoBehaviour
{
    public Text questionText;
    public InputField answerInput;
    public Button submitButton;
    private int correctAnswer;
    private System.Action<bool> onAnswerSubmitted;

    void Start()
    {
        GenerateQuestion();
        submitButton.onClick.AddListener(CheckAnswer);
    }

    void GenerateQuestion()
    {
        int number1 = Random.Range(1, 10);
        int number2 = Random.Range(1, 10);
        correctAnswer = number1 + number2;
        questionText.text = number1 + " + " + number2 + " = ?";
    }

    void CheckAnswer()
    {
        int playerAnswer;
        if (int.TryParse(answerInput.text, out playerAnswer))
        {
            bool isCorrect = playerAnswer == correctAnswer;
            onAnswerSubmitted?.Invoke(isCorrect);
        }
    }

    public void SetOnAnswerSubmittedCallback(System.Action<bool> callback)
    {
        onAnswerSubmitted = callback;
    }
}
