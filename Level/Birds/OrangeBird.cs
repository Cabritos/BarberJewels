public class OrangeBird : Bird
{
    protected override void HitReward()
    {
        SoundManager.Instance.PlayOrangeBirdClip();
    }
}
