using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    public float DelayBeforeTheNextQuestion;
    public bool clickToChangeQuestion;

    private Sprite backgroundImage;
    private TextMeshProUGUI showCurrentQuestionNumber;
    private TextMeshProUGUI showTotalQuestionsCount;
    private GameObject finalScores;
    private TextMeshProUGUI questionText;
    private GameObject answers;
    private AcrossSceneAgentScript acrossSceneAgentScript;
    private GameObject[] answersObjects;
    private TextMeshProUGUI[] answersTexts;
    private Image[] answersImages;
    private Button[] answersButtons;
    private Button changeToNextQuestionButton;

    private Yandex yandex;

    private Dictionary<string, string[]> quiz;
    private int questionNumber = -1;
    private int rightAnswerID = -1;
    private int answersCount = -1;

    private int totalQuestionCount = -1;
    private int rightAnswersCount = -1;

    void Start()
    {
        acrossSceneAgentScript = GameObject.FindGameObjectWithTag("AcrossSceneAgent").GetComponent<AcrossSceneAgentScript>();
        backgroundImage = Resources.Load<Sprite>($"Backgrounds/General Background/фон для {acrossSceneAgentScript.TestName}");

        answers = GameObject.FindGameObjectWithTag("Answers");
        questionText = GameObject.FindGameObjectWithTag("QuestionText").GetComponent<TextMeshProUGUI>();
        finalScores = GameObject.FindGameObjectWithTag("Result");
        finalScores.SetActive(false);

        answersObjects = FindAllChilds(answers);
        answersTexts = answers.GetComponentsInChildren<TextMeshProUGUI>();
        answersImages = answers.GetComponentsInChildren<Image>();
        answersButtons = answers.GetComponentsInChildren<Button>();

        changeToNextQuestionButton = GameObject.FindGameObjectWithTag("ChangeToNextQuestion").GetComponent<Button>();
        changeToNextQuestionButton.gameObject.SetActive(false);

        yandex = GameObject.FindGameObjectWithTag("Yandex").GetComponent<Yandex>();
        try
        {
            yandex.ShowAdvSDK();
        }
        catch (Exception exception)
        {
            Debug.Log($"Got an exception:\n{exception}");
        }
        Debug.Log("Successfully recovered from the exception");

        showCurrentQuestionNumber = GameObject.FindGameObjectWithTag("CurrentQuestionNumber").GetComponentInChildren<TextMeshProUGUI>();
        showTotalQuestionsCount = GameObject.FindGameObjectWithTag("TotalQuestionNumber").GetComponentInChildren<TextMeshProUGUI>();

        SetBackground();
        quiz = GetQuiz();

        ChangeToNextQuestion();
    }

    private void SetBackground()
    {
        var backgroundImages = GameObject.Find("BackgroundGeneral").GetComponentsInChildren<Image>();
        foreach (var image in backgroundImages)
            image.sprite = backgroundImage;
    }

    public void VerifyAnswers(int clickedButtonID)
    {
        StartCoroutine(VerifyAnswersCoroutine(clickedButtonID));
    }

    IEnumerator VerifyAnswersCoroutine(int clickedButtonID)
    {
        SetButtonComponentsActiveOrInactive(false);

        if (clickedButtonID != rightAnswerID)
        {
            answersImages[clickedButtonID].color = new Color(255, 0, 0, 255);
            rightAnswersCount--;
        }
        answersImages[rightAnswerID].color = new Color(0, 255, 0, 255);

        if (clickToChangeQuestion)
        {
            var waitForButton = new WaitForUIButtons(changeToNextQuestionButton);
            yield return waitForButton.Reset();
        }
        else
        {
            yield return new WaitForSeconds(DelayBeforeTheNextQuestion);
        }

        answersImages[clickedButtonID].color = new Color(255, 255, 255, 255);
        answersImages[rightAnswerID].color = new Color(255, 255, 255, 255);

        ChangeToNextQuestion();
        SetButtonComponentsActiveOrInactive(true);
    }

    public void ChangeToNextQuestion()
    {
        var usedAnswers = new List<int>();
        var random = new System.Random();

        questionNumber++;

        if (questionNumber == quiz.Count)
        {
            ShowFinalScores();
            return;
        }
        if (questionNumber == quiz.Count - 1)
        {
            changeToNextQuestionButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Узнать результат";
        }

        showCurrentQuestionNumber.text = $"{questionNumber+1}";

        var questionFromFile = quiz.ElementAt(questionNumber).Key;
        var answersFromFile = quiz.ElementAt(questionNumber).Value;
        answersCount = answersFromFile.Length;

        questionText.text = questionFromFile;
        for (var i = 0; i < answersCount; i++)
        {
            var randomAnswer = random.Next(answersCount);
            while (usedAnswers.Contains(randomAnswer))
                randomAnswer = random.Next(answersCount);
            usedAnswers.Add(randomAnswer);

            answersObjects[i].SetActive(true);
            if (answersFromFile[randomAnswer].Contains(" верно"))
            {
                rightAnswerID = i;
                answersTexts[i].text = answersFromFile[randomAnswer].Replace(" верно", "");
            }
            else
            {
                answersTexts[i].text = answersFromFile[randomAnswer];
            }
        }

        for (var i = answersCount; i < answersObjects.Length; i++)
        {
            answersObjects[i].SetActive(false);
        }

    }

    private void ShowFinalScores()
    {
        answers.SetActive(false);
        questionText.text = "Ваш результат";
        finalScores.SetActive(true);

        var finalScoresTMPro = finalScores.GetComponentInChildren<TextMeshProUGUI>();

        var rightAnswersPercentage = rightAnswersCount * 100 / totalQuestionCount;

        if (rightAnswersPercentage >= 80)
            finalScoresTMPro.color = new Color(0, 255, 0, 255);
        else if (rightAnswersPercentage >= 40)
            finalScoresTMPro.color = new Color(255, 125, 0, 255);
        else
            finalScoresTMPro.color = new Color(255, 0, 0, 255);

        finalScoresTMPro.text = $"{rightAnswersCount}/{totalQuestionCount} ({rightAnswersPercentage}%)";
    }

    private Dictionary<string, string[]> GetQuiz()
    {
        var result = new Dictionary<string, string[]>();
        var path = $"Quiz/{acrossSceneAgentScript.TestName}/{acrossSceneAgentScript.TestDifficulty}";

        var splittedByTagsText = Resources.Load<TextAsset>(path).ToString().Split("{q}", StringSplitOptions.RemoveEmptyEntries);
        foreach (var task in splittedByTagsText)
        {
            var splittedTaskBySeparator = task.Split(';').Select(x => x.Trim()).ToArray();

            var question = $"{splittedTaskBySeparator[0]}";
            if (splittedTaskBySeparator.Length == 1)
                throw new IndexOutOfRangeException($"Error is here:\n{splittedTaskBySeparator[0]}");
            var answers = splittedTaskBySeparator[1].Split('\n');
            answersCount = answers.Length;

            result[question] = answers;
        }

        totalQuestionCount = result.Count;
        rightAnswersCount = totalQuestionCount;
        showTotalQuestionsCount.text = $"{totalQuestionCount}";

        return result;
    }

    private void SetButtonComponentsActiveOrInactive(bool setActive)
    {
        foreach (var button in answersButtons)
            button.enabled = setActive;
        changeToNextQuestionButton.gameObject.SetActive(!setActive);
    }

    private GameObject[] FindAllChilds(GameObject originalGameObject)
    {
        var result = new GameObject[originalGameObject.transform.childCount];
        for (int i = 0; i < originalGameObject.transform.childCount; i++)
        {
            result[i] = originalGameObject.transform.GetChild(i).gameObject;
        }
        return result;
    }

    private void PrintDictionaryForQuiz(Dictionary<string, string[]> dictionary)
    {
        var result = new StringBuilder();
        foreach (var e in dictionary)
        {
            result.Append($"{e.Key}\n");
            foreach (var answer in e.Value)
            {
                result.Append($"{answer}\n");
            }
            result.Append("\n");
        }
        Debug.Log(result.ToString());
    }
}
