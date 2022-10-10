using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBird : Bird
{
    float rewardTime = 10f;

    protected override void HitReward()
    {
        if (UnityEngine.Random.Range(0, 5) < 4)
        {
            RewardScoreMultiplier(2);
        }
        else
        {
            RewardScoreMultiplier(3);
            SoundManager.Instance.PlayBirdRandomRewardClip();
        }
    }

    private void RewardScoreMultiplier(int multiplier)
    {
        var score = levelManager.Score;
        score.SetScoreMultiplier(multiplier, rewardTime);
        DisplayFx(multiplier);
        SoundManager.Instance.PlayRedBirdClip();
    }

    private void DisplayFx(int multiplier)
    {
        var fxText = "Bonus x" + multiplier + "!";
        var fx = fxSpawner.DisplayPersistentText(fxText, true, rewardTime, transform.position, birdSpawner.birdsParent);
        fx.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }
}