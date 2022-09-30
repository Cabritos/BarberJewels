using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonobehaviour<GameManager>
{
#if UNITY_EDITOR
    [SerializeField] bool onLevelDebug = false;
    [SerializeField] int debugLevel;
#endif

    public int TotalScore { get; private set; }
    public int PreviousTotalScore { get; private set; }
    public int LevelScore { get; private set; }
    public int JewelHits { get; private set; }
    public bool HasLost { get; private set; }
    public bool EndlessGame { get; private set; }
    public bool IsConnectedToGooglePlayServices { get; private set; }
    public ErrorUi ErrorUi { get; private set; }

    AchivementsManager achivementManager;

    [SerializeField] LevelConfigListSO levelConfigSO;

    public int CurrentLevel { get; private set; }

    private EndlessGameProgressionManager progressionManager;
    int iterationNumber;

    LevelManager currentLevelManager;

    protected override void Awake()
    {
        base.Awake();
        progressionManager = GetComponent<EndlessGameProgressionManager>();
        achivementManager = GetComponent<AchivementsManager>();
        ErrorUi = GetComponent<ErrorUi>();
        GooglePlayServicesManager.AuthenticateToGooglePlayServices();
    }

    public void StartNewRegularGame()
    {
        Debug.Log("Starting new regular game");;
        ReportNewGame();
        EndlessGame = false;
        CurrentLevel = 0;

#if UNITY_EDITOR
        if (onLevelDebug) CurrentLevel = debugLevel - 1;
#endif

        HasLost = false;
        LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        Debug.Log("Lading level");
        TotalScore += LevelScore;
        LevelScore = 0;
        FindObjectOfType<BannerAd>().gameObject.SetActive(false);
        CurrentLevel++;
        Time.timeScale = 1;
        SceneManager.LoadScene("Level");
    }

    private void ReportNewGame()
    {
        if (!ValidateConnectivity())
        {
            Debug.LogWarning("Unable to report new game");
            return;
        }

        achivementManager.ReportNewGame();
    }

    public void StartNewEndlessGame()
    {
        Debug.Log("Starting new endless game");
        ReportNewGame();
        achivementManager.ReportEndlessGame();
        EndlessGame = true;
        iterationNumber = 0;

#if UNITY_EDITOR
        if (onLevelDebug) iterationNumber = debugLevel - 1;
#endif
        Time.timeScale = 1;
        SceneManager.LoadScene("Level");
    }

    public void SetupNewLevel(LevelManager levelManager)
    {
        Debug.Log("Setting up new level in game manager");
        currentLevelManager = levelManager;
        SoundManager.Instance.SetupNewLevel();
        achivementManager.SetupNewLevel(levelManager);
    }

    public LevelManager GetCurrentLevelManager()
    {
        if (currentLevelManager == null)
        {
            currentLevelManager = FindObjectOfType<LevelManager>();

            if (currentLevelManager == null)
            {
                Debug.LogError("LevelManager could not be found");
            }
        }

        return currentLevelManager;
    }

    public void WonLevel(int score, int jewelHits)
    {
        Debug.Log("Level won");
        EndLevel(score, jewelHits);
    }
    public void LostLevel(int score, int jewelHits)
    {
        Debug.Log("Level lost");
        HasLost = true;
        achivementManager.ReportLostGame();
        EndLevel(score, jewelHits);
    }

    private void EndLevel(int score, int jewelHits)
    {
        Debug.Log("Ending level. Setting scores");

        Time.timeScale = 1;
        ProcessScores(score, jewelHits);

        if (ValidateConnectivity())
        {
            ReportScore();
            ReportAchivements();
        }

        SoundManager.Instance.EndLevel();
        SceneManager.LoadScene("Balance");
    }

    private void ProcessScores(int score, int jewelHits)
    {
        PreviousTotalScore = TotalScore;
        LevelScore = score;
        TotalScore += score;
        JewelHits = jewelHits;
    }

    private void ReportAchivements()
    {
        achivementManager.ReportLevelProgress();
        if (!EndlessGame) achivementManager.LevelWon(CurrentLevel);
    }

    private void ReportScore()
    {
        if (EndlessGame)
        {
            GooglePlayServicesManager.ReportEndlessGameScore(TotalScore);
            achivementManager.ReportEndlessScore(TotalScore);
        }
        else
        {
            GooglePlayServicesManager.ReportRegularGameScore(TotalScore);
        }
    }

    public bool ValidateConnectivity()
    {
        return IsConnectedToGooglePlayServices || Application.internetReachability != NetworkReachability.NotReachable;
    }

    public bool ValidateConnectivityOrShowError()
    {
        if (IsConnectedToGooglePlayServices)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ErrorUi.ShowError(Errors.InternetConnectionNeededGeneric);
                Debug.LogWarning("Failed to proceed due lack of internet connection");
                return false;
            }
            else
            {
                Debug.Log("Connection validated");
                return true;
            }
        }
        else
        {
            ErrorUi.ShowError(Errors.GooglePlayConnectionNeeded);
            Debug.LogWarning("Failed to proceed because Google Play Services is not connected");
            return false;
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public LevelConfigSO GetNewIterationConfig()
    {
            iterationNumber++;
            Debug.Log("New iteration: " + iterationNumber);
        return progressionManager.GetNewConfig(iterationNumber);
    }

    public LevelConfigSO GetCurrentConfig()
    {
        if (EndlessGame)
        {
            return progressionManager.GetBaseConfig();
        }
        else
        {
            return GetLevelConfig(CurrentLevel);
        }
    }

    public LevelConfigSO GetLevelConfig(int levelId)
    {
        return levelConfigSO.GetLevelConfig(levelId);
    }

    public void SetConnectedToGooglePlayServicesStatus(bool status)
    {
        IsConnectedToGooglePlayServices = status;
    }
}