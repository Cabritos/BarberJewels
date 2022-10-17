public class GreenBird : Bird
{
    protected override void HitReward()
    {
        levelManager.SlowDown(7);
        SoundManager.Instance.PlayGreenBirdClip();
    }
}