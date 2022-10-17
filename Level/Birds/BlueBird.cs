using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBird : Bird
{
    protected override void HitReward()
    {
        if (UnityEngine.Random.Range(0, 5) < 4)
        {
            RewardLives(1);
        }
        else
        {
            RewardLives(3);
            SoundManager.Instance.PlayBirdRandomRewardClip();
        }
    }

    private void RewardLives(int ammount)
    {
        var lives = levelManager.Lives;
        lives.AddLives(ammount);
        DisplayFx(ammount);
        SoundManager.Instance.PlayBlueBirdClip();
    }

    private void DisplayFx(int ammount)
    {
        var fx = fxSpawner.DisplayLivesFx(ammount, false, transform.position);
        fx.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
    }
}