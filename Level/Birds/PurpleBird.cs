using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PurpleBird : Bird
{
    protected override void HitReward()
    {
        levelManager.Rewind();
        SoundManager.Instance.PlayPurpleBirdClip();
    }
}
