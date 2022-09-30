using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBird : Bird
{
    protected override void HitReward()
    {
        levelManager.SlowDown(6);
        SoundManager.Instance.PlayGreenBirdClip();
    }
}
