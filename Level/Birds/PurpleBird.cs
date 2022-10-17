public class PurpleBird : Bird
{
    protected override void HitReward()
    {
        levelManager.Rewind(100);
    }
}