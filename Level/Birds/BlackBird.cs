using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBird : Bird
{
    int scoreReward = 100;

    protected override void HitReward()
    {
        var score = levelManager.Score;
        var finalScore = score.AddToScore(scoreReward);
        DisplayFx(finalScore);
        SoundManager.Instance.PlayBlackBirdClip();
    }

    private void DisplayFx(int score)
    {
        var fx = fxSpawner.DisplayPointsFx(score, true, transform.position);
        fx.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
    }
}
