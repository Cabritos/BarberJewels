public class WhiteBird : Bird
{
    protected override void HitReward()
    {
        levelManager.JewelManager.ReplaceSomeJewels();
        SoundManager.Instance.PlayWhiteBirdClip();
    }
}