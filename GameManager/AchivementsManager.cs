using UnityEngine;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using System;

public class AchivementsManager : MonoBehaviour
{
    int jewelsHit;
    int birdsHit;
    int birdCalls;
    int extraLives;
    int slowDowns;
    int rewinds;
    int orangePower;
    int yellowPowerEarned;

    LevelManager currentLevelManager;

    private void QuickReportAchivement(string achivementId)
    {
        Social.ReportProgress(achivementId, 100.0f, (bool success) => {
            // handle success or failure
        });
    }

    private void ReportAchivement(string achivementId)
    {
        if (!GameManager.Instance.ValidateConnectivity()) return;

        QuickReportAchivement(achivementId);
    }

    private void QuickReportIncementalProgress(string achivementId, int increment)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(achivementId, increment, (bool success) => {
            // handle success or failure
        });
    }

    private void ReportIncementalProgress(string achivementId, int increment)
    {
        if (!GameManager.Instance.ValidateConnectivity()) return;

        QuickReportIncementalProgress(achivementId, increment);
    }

    public void ReportNewGame()
    {
        QuickReportIncementalProgress(GPGSIds.achievement_rookie_numbers, 1);
        QuickReportIncementalProgress(GPGSIds.achievement_more, 1);
        QuickReportIncementalProgress(GPGSIds.achievement_i_love_it, 1);
        QuickReportIncementalProgress(GPGSIds.achievement_pro_gamer, 1);
        QuickReportIncementalProgress(GPGSIds.achievement_barber_jewler, 1);
    }

    private void ResetLevelProgress()
    {
        jewelsHit = 0;
        birdsHit = 0;
        birdCalls = 0;
        extraLives = 0;
        slowDowns = 0;
        rewinds = 0;
        orangePower = 0;
        yellowPowerEarned = 0;
    }

    public void SetupNewLevel(LevelManager levelManager)
    {
        ResetLevelProgress();
        currentLevelManager = levelManager;
        SubscribeToLevelsEvents();
    }

    private void SubscribeToLevelsEvents()
    {
        currentLevelManager.JewelManager.JewelHit += CountJewelHit;
        currentLevelManager.BirdSpawner.BirdHit += CountBirdHit;
        currentLevelManager.Rewinding += CountRewinds;
        currentLevelManager.SlowingDown += CountSlowDonws;
        currentLevelManager.Score.YellowBirdPowerEarned += CountYellowBirdPowersEarned;
        currentLevelManager.Lives.OnLivesWon += CountLivesWon;
        currentLevelManager.GetComponent<PlayerInput>().PowerButtonPressed += CountOrangePowers;
    }

    private void CountJewelHit(ColorType type, int combo)
    {
        jewelsHit++;
    }

    private void CountBirdHit(ColorType type)
    {
        birdsHit++;

        if (type == ColorType.pink)
        {
            birdCalls++;
        }
    }
    private void CountSlowDonws(float duration)
    {
        slowDowns++;
    }

    private void CountRewinds()
    {
        rewinds++;
    }

    private void CountYellowBirdPowersEarned()
    {
        yellowPowerEarned++;
    }

    private void CountLivesWon(int ammount)
    {
        extraLives += ammount;
    }

    private void CountOrangePowers(ColorType type)
    {
        orangePower++;
    }

    public void LevelWon(int level)
    {
        if (level == 5)
        {
            QuickReportAchivement(GPGSIds.achievement_bring_it_on);
        }
        else if (level == 10)
        {
            QuickReportAchivement(GPGSIds.achievement_ive_got_this);
        }
        else if (level == 15)
        {
            QuickReportAchivement(GPGSIds.achievement_jewel_crusher);
        }
        else if (level == 20)
        {
            QuickReportAchivement(GPGSIds.achievement_unstoppable);
        }
    }

    public void ReportLevelProgress()
    {
        QuickReportIncementalProgress(GPGSIds.achievement_and_so_it_begins, jewelsHit);
        QuickReportIncementalProgress(GPGSIds.achievement_amateur, jewelsHit);
        QuickReportIncementalProgress(GPGSIds.achievement_jewel_collector, jewelsHit);
        QuickReportIncementalProgress(GPGSIds.achievement_millonare, jewelsHit);
        QuickReportIncementalProgress(GPGSIds.achievement_royalty, jewelsHit);

        QuickReportIncementalProgress(GPGSIds.achievement_angry_birds, birdsHit);
        QuickReportIncementalProgress(GPGSIds.achievement_pigeon_lady, birdsHit);
        QuickReportIncementalProgress(GPGSIds.achievement_gotta_catch_em_all, birdsHit);
        QuickReportIncementalProgress(GPGSIds.achievement_sanctuary, birdsHit);

        QuickReportIncementalProgress(GPGSIds.achievement_one_with_nature, birdCalls);
        QuickReportIncementalProgress(GPGSIds.achievement_extra_lives, extraLives);
        QuickReportIncementalProgress(GPGSIds.achievement_quiet_please, slowDowns);
        QuickReportIncementalProgress(GPGSIds.achievement_the_other_way_arround, rewinds);
        QuickReportIncementalProgress(GPGSIds.achievement_its_a_kind_of_magic, orangePower);
        QuickReportIncementalProgress(GPGSIds.achievement_its_the_power_of_love, yellowPowerEarned);

        ResetLevelProgress();
    }

    public void ReportEndlessScore(int totalScore)
    {
        if (totalScore >= 5000)
        {
            QuickReportAchivement(GPGSIds.achievement_nice);
        }
        
        if (totalScore >= 10000)
        {
            QuickReportAchivement(GPGSIds.achievement_good_run);
        }
        
        if (totalScore >= 20000)
        {
            QuickReportAchivement(GPGSIds.achievement_thats_impressive);
        }

        if (totalScore >= 30000)
        {
            QuickReportAchivement(GPGSIds.achievement_truly_epic);
        }
    }

    public void ReportLostGame()
    {
        ReportAchivement(GPGSIds.achievement_the_game);
    }

    public void ReportEndlessGame()
    {
        ReportAchivement(GPGSIds.achievement_someone_wants_a_challenge);
    }

    private void OnDisable()
    {
        UnsubscribeToLevelEvents();
    }

    private void UnsubscribeToLevelEvents()
    {
        if (currentLevelManager == null) return;

        currentLevelManager.JewelManager.JewelHit -= CountJewelHit;
        currentLevelManager.BirdSpawner.BirdHit -= CountBirdHit;
        currentLevelManager.Rewinding -= CountRewinds;
        currentLevelManager.SlowingDown -= CountSlowDonws;
        currentLevelManager.Score.YellowBirdPowerEarned -= CountYellowBirdPowersEarned;
        currentLevelManager.Lives.OnLivesWon -= CountLivesWon;
        currentLevelManager.GetComponent<PlayerInput>().PowerButtonPressed -= CountOrangePowers;
    }
}