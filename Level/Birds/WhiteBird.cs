using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBird : Bird
{
    protected override void HitReward()
    {
        levelManager.JewelManager.ReplaceSomeJewels();
        SoundManager.Instance.PlayWhiteBirdClip();
    }
}
