using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class BalanceUI : MonoBehaviour
{
    [SerializeField] TMP_Text levelScoreLabel;
    [SerializeField] TMP_Text levelScoreText;
    [SerializeField] TMP_Text totalScoreLabel;
    [SerializeField] TMP_Text totalScoreText;
    [SerializeField] Text nextButtonText;
    [SerializeField] Text mainMenuButtonText;
    [SerializeField] float animationInterval;

    bool endlessGame;
    bool hasLost;

    private void Start()
    {
        SoundManager.Instance.PlayMenuMusic();

        Setup();

        if (endlessGame)
        {
            StartCoroutine(EndlessModeBalance());
        }
        else
        {
            StartCoroutine(RegularModeBalance());
        }
    }

    private void Setup()
    {
        endlessGame = GameManager.Instance.EndlessGame;
        hasLost = GameManager.Instance.HasLost;

        if (hasLost)
        {
            Camera.main.backgroundColor = Color.black;
        }

        if (endlessGame || hasLost)
        {
            nextButtonText.text = "Leaderboard";
            mainMenuButtonText.text = "Main menu";
        }
    }

    private IEnumerator RegularModeBalance()
    {
        var previousTotalScore = GameManager.Instance.PreviousTotalScore;
        totalScoreText.text = previousTotalScore.ToString();

        yield return new WaitForSeconds(2f);

        var levelEarnings = GameManager.Instance.LevelScore;
        StartCoroutine(AddScoreAnimation(levelEarnings, 0, 10, levelScoreText));

        yield return new WaitForSeconds(1f);

        StartCoroutine(AddScoreAnimation(levelEarnings, previousTotalScore, 10, totalScoreText));
    }

    private IEnumerator EndlessModeBalance()
    {
        levelScoreLabel.text = "Total jewels:";
        totalScoreLabel.text = "Your score:";

        yield return new WaitForSeconds(2f);

        var jewelsDestroyed = GameManager.Instance.JewelHits;
        StartCoroutine(AddScoreAnimation(jewelsDestroyed, 0, 10, levelScoreText));

        yield return new WaitForSeconds(1f);

        var levelScore = GameManager.Instance.LevelScore;
        StartCoroutine(AddScoreAnimation(levelScore, 0, 100, totalScoreText));
    }

    private IEnumerator AddScoreAnimation(int scoreToAdd, int baseScore, int units, TMP_Text text)
    {
        var points = scoreToAdd / units;

        for (int i = 0; i < points; i++)
        {
            baseScore += units;
            text.text = baseScore.ToString();
            yield return new WaitForSeconds(animationInterval);
        }

        var remainder = points % units;
        if (remainder > 0)
        {
            StartCoroutine(AddScoreAnimation(remainder, baseScore, units / 10, text));
        }
    }

    public void EndAnimations()
    {
        StopAllCoroutines();

        var totalScore = GameManager.Instance.TotalScore;
        totalScoreText.text = totalScore.ToString();

        if (endlessGame)
        {
            levelScoreText.text = GameManager.Instance.JewelHits.ToString();
        }
        else
        {
            levelScoreText.text = GameManager.Instance.LevelScore.ToString();
        }
    }

    public void RespondToNextButtonPress()
    {
        if (hasLost)
        {
            TryToShowLeaderboard();
        }
        else
        {
            GameManager.Instance.LoadNextLevel();
        }
    }

    private void TryToShowLeaderboard()
    {
        if (GameManager.Instance.ValidateConnectivityOrShowError())
        {
            if (endlessGame)
            {
                GooglePlayServicesManager.ShowEndlessGameLeaderboard();
            }
            else
            {
                GooglePlayServicesManager.ShowRegularGameLeaderboard();
            }
        }
    }

    public void MainMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}