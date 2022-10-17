public class PinkBird : Bird
{
    protected override void HitReward()
    {
        birdSpawner.SpawnBirds(2);
        SoundManager.Instance.PlayPinkBirdClip();
    }
}