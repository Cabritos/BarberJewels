using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public static class GooglePlayServicesManager
{
    public static void AuthenticateToGooglePlayServices()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Unable to connect to Google Play Services. No internet connection");
            GameManager.Instance.SetConnectedToGooglePlayServicesStatus(false);
            return;
        }

        Authenticate();
    }

    private static void Authenticate()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(OnSignInResult);
    }

    private static void OnSignInResult(SignInStatus signInStatus)
    {
        if (signInStatus == SignInStatus.Success)
        {
            Debug.Log("Sucessfull connection to Google Play Services");
            GameManager.Instance.SetConnectedToGooglePlayServicesStatus(true);
        }
        else
        {
            Debug.LogWarning($"Failed to connect to Google Play Services. Status: {signInStatus}");
            GameManager.Instance.SetConnectedToGooglePlayServicesStatus(false);
        }
    }

    public static void ShowLeaderboards()
    {
        Social.ShowLeaderboardUI();
    }

    public static void ShowRegularGameLeaderboard()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_top_scores);
    }

    public static void ShowEndlessGameLeaderboard()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_endless_game);
    }

    public static void ShowAchivements()
    {
        Social.ShowAchievementsUI();
    }

    public static void ReportRegularGameScore(int score)
    {
        ReportScore(GPGSIds.leaderboard_top_scores, score);
    }

    public static void ReportEndlessGameScore(int score)
    {
        ReportScore(GPGSIds.leaderboard_endless_game, score);
    }

    private static void ReportScore(string scoresId, int score)
    {
        Social.ReportScore(score, scoresId, (bool success) => {
            // handle success or failure
        });
    }

    public static void GooglePlayManualSignIn()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(OnSignInResult);
    }
}