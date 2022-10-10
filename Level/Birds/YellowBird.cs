using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBird : Bird
{
    protected override void HitReward()
    {
        var jewelManager = levelManager.JewelManager;
        if (jewelManager.NoJewelsAreInGameArea()) return;

        fxSpawner.ReleaseTrail();
    }
}   