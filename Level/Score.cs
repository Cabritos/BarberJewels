using UnityEngine;
using System;
using System.Collections;

public class Score : MonoBehaviour
{
    public event Action<int> ScoreUpdated;
    public event Action YellowBirdPowerEarned;

    public int CurrentScore { get; private set; }
    ColorType lastDestroyed = ColorType.nullColor;
    int comboMultiplier;
    int doubleScore;

    [SerializeField] GameObject yellowPower;
    [SerializeField] int yellowPowerPrice = 3000;
    int acumulatedScoreToYellowPower;

    private void Start()
    {
        acumulatedScoreToYellowPower = 0;
        CurrentScore = 0;
        comboMultiplier = 1;
        doubleScore = 1;
    }

    public ScoreResult JewelDestroyed(ColorType type, int baseScore)
    {
        var finalScore = CalculateJewelScore(type, baseScore);
        AddToScore(finalScore);

        return new ScoreResult(finalScore, comboMultiplier);
    }

    private int CalculateJewelScore(ColorType type, int baseScore)
    {
        SetJewelComboMultiplier(type);

        int multipleOfFive = (int)Mathf.Floor(comboMultiplier / 5) * comboMultiplier * baseScore;
        int score = baseScore * comboMultiplier + multipleOfFive;
        var finalScore = CalculateScore(score);

        return finalScore;
    }

    private void SetJewelComboMultiplier(ColorType type)
    {
        if (lastDestroyed == type)
        {
            comboMultiplier++;
        }
        else
        {
            lastDestroyed = type;
            comboMultiplier = 1;
        }
    }

    public int AddToScore(int ammount)
    {
        var score = CalculateScore(ammount);
        AddToTotalScore(score);
        AddToYellowPowerScore(score);
        return score;
    }

    private int CalculateScore(int baseScore)
    {
        return baseScore * doubleScore;
    }

    private void AddToTotalScore(int ammount)
    {
        CurrentScore += ammount;
        ScoreUpdated?.Invoke(CurrentScore);
    }

    public void DebugAddScore()
    {
#if UNITY_EDITOR
        AddToScore(500);
        Debug.Log("Score added");
#endif
    }

    private void AddToYellowPowerScore(int score)
    {
        acumulatedScoreToYellowPower += score;

        if (acumulatedScoreToYellowPower >= yellowPowerPrice)
        {
            acumulatedScoreToYellowPower -= yellowPowerPrice;
            GetYellowPower();
        }
    }

    private void GetYellowPower()
    {
        yellowPower.SetActive(true);
        yellowPower.GetComponent<YellowPowerButtton>().PowerWon();
        YellowBirdPowerEarned?.Invoke();
    }

    public void SetScoreMultiplier(int multiplier, float time)
    {
        StartCoroutine(ScoreMultiplierRoutine(multiplier, time));
    }

    private IEnumerator ScoreMultiplierRoutine(int multiplier, float time)
    {
        doubleScore *= multiplier;
        yield return new WaitForSeconds(time);
        doubleScore /= multiplier;
    }

    public struct ScoreResult
    {
        public int Score { get; }
        public int ComboMultiplier { get; }

        public ScoreResult(int score, int comboMultiplier)
        {
            Score = score;
            ComboMultiplier = comboMultiplier;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        ScoreUpdated = null;
    }
}