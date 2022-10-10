using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    public bool IsConnectedToGooglePlayServices { get; private set; }
    public ErrorUi ErrorUi { get; private set; }
    AchivementsManager achivementManager;
    BannerAd bannerAd;

    LevelManager currentLevelManager;
    [SerializeField] LevelConfigListSO levelConfigSO;
    EndlessGameProgressionManager progressionManager;
    public int CurrentLevel { get; private set; }
    int iterationNumber;

    public int TotalScore { get; private set; }
    public int PreviousTotalScore { get; private set; }
    public int LevelScore { get; private set; }
    public int JewelHits { get; private set; }
    public bool HasLost { get; private set; }
    public bool EndlessGame { get; private set; }

#if UNITY_EDITOR
    [SerializeField] bool onLevelDebug = false;
    [SerializeField] int debugLevel;
#endif

    protected override void Awake()
    {
        base.Awake();
        ErrorUi = GetComponent<ErrorUi>();
        progressionManager = GetComponent<EndlessGameProgressionManager>();
        achivementManager = GetComponent<AchivementsManager>();
        bannerAd = GetComponent<BannerAd>();
        GooglePlayServicesManager.AuthenticateToGooglePlayServices();
    }

    private void Start()
    {
        bannerAd.LoadAndShowAd();
    }

    public void StartNewRegularGame()
    {
        Debug.Log("Starting new regular game");;
        ReportNewGame();
        EndlessGame = false;
        HasLost = false;
        ResetScores();
        CurrentLevel = 0;

#if UNITY_EDITOR
        if (onLevelDebug) CurrentLevel = debugLevel - 1;
#endif

        LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        Debug.Log("Loading level");
        CurrentLevel++;
        LoadLevelScene();
    }

    private void LoadLevelScene()
    {
        HideBannerAd();
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
        ResetScores();
        EndlessGame = true;
        achivementManager.ReportEndlessGame();
        iterationNumber = 0;

#if UNITY_EDITOR
        if (onLevelDebug) iterationNumber = debugLevel - 1;
#endif

        LoadLevelScene();
    }

    private void ResetScores()
    {
        LevelScore = 0;
        TotalScore = 0;
        PreviousTotalScore = 0;
    }

    private void HideBannerAd()
    {
        bannerAd.HideAd();
    }

    private void OnStartLevel()
    {
        currentLevelManager.StartingLevel -= OnStartLevel;
        HideBannerAd();
    }

    public void SetupNewLevel(LevelManager levelManager)
    {
        Debug.Log("Setting up new level in game manager");
        currentLevelManager = levelManager;
        currentLevelManager.StartingLevel += OnStartLevel;
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
        EndLevel(score, jewelHits);
    }

    public void LostLevel(int score, int jewelHits)
    {
        HasLost = true;
        achivementManager.ReportLostGame();
        EndLevel(score, jewelHits);
    }

    private void EndLevel(int score, int jewelHits)
    {
        Debug.Log("Ending level");

        Time.timeScale = 1;
        ProcessScores(score, jewelHits);

        if (ValidateConnectivity())
        {
            ReportScore();
            ReportAchivements();
        }

        SoundManager.Instance.EndLevel();
        SceneManager.LoadScene("Balance");
        bannerAd.LoadAndShowAd();
    }

    private void ProcessScores(int score, int jewelHits)
    {
        LevelScore = score;
        PreviousTotalScore = TotalScore;
        TotalScore += score;
        JewelHits = jewelHits;
    }

    private void ReportAchivements()
    {
        Debug.Log("Reporting achivements.");
        achivementManager.ReportLevelProgress();
        if (!EndlessGame) achivementManager.LevelWon(CurrentLevel);
    }

    private void ReportScore()
    {
        Debug.Log("Reporting scores.");
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
        bannerAd.LoadAndShowAd();
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