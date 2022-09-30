using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeBird : Bird
{
    protected override void HitReward()
    {
        SoundManager.Instance.PlayOrangeBirdClip();
    }
}
