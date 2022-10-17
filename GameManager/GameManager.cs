using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    bool isConnectedToGooglePlayServices = false;
    AchivementsManager achivementManager;
    GoogleEventsReporter googleEventsReporter;
    BannerAd bannerAd;
    ErrorUi errorUi;

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
        CacheReferences();
        GooglePlayServicesManager.AuthenticateToGooglePlayServices();
    }

    private void CacheReferences()
    {
        errorUi = GetComponent<ErrorUi>();
        progressionManager = GetComponent<EndlessGameProgressionManager>();
        achivementManager = GetComponent<AchivementsManager>();
        bannerAd = GetComponent<BannerAd>();
        googleEventsReporter = GetComponent<GoogleEventsReporter>();
    }

    private void Start()
    {
        bannerAd.LoadAndShowAd();
    }

    public void StartNewRegularGame()
    {
        Debug.Log("Starting new regular game");;
        EndlessGame = false;
        HasLost = false;
        CurrentLevel = 0;

#if UNITY_EDITOR
        if (onLevelDebug) CurrentLevel = debugLevel - 1;
#endif

        ResetScores();
        achivementManager.ReportNewGame();
        LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        CurrentLevel++;
        googleEventsReporter.ReportLevel(CurrentLevel);
        LoadLevelScene();
    }

    private void LoadLevelScene()
    {
        Debug.Log("Loading level scene");
        HideBannerAd();
        Time.timeScale = 1;
        SceneManager.LoadScene("Level");
    }

    public void StartNewEndlessGame()
    {
        Debug.Log("Starting new endless game");
        EndlessGame = true;
        HasLost = false;
        iterationNumber = 0;

#if UNITY_EDITOR
        if (onLevelDebug) iterationNumber = debugLevel - 1;
#endif
        ResetScores();
        achivementManager.ReportNewGame();
        achivementManager.ReportEndlessGame();
        LoadLevelScene();
    }

    private void ResetScores()
    {
        LevelScore = 0;
        TotalScore = 0;
        PreviousTotalScore = 0;
    }

    public void HideBannerAd()
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
        return isConnectedToGooglePlayServices && Application.internetReachability != NetworkReachability.NotReachable;
    }

    public bool ValidateConnectivityOrShowError()
    {
        if (isConnectedToGooglePlayServices)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                errorUi.ShowError(Errors.InternetConnectionNeededGeneric);
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
            errorUi.ShowError(Errors.GooglePlayConnectionNeeded);
            Debug.LogWarning("Failed to proceed because Google Play Services is not connected");
            return false;
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        bannerAd.LoadAndShowAd();
    }

    public LevelConfigSO GetCurrentConfig()
    {
        if (EndlessGame)
        {
            googleEventsReporter.ReportIteration(0);
            return progressionManager.GetBaseConfig();
        }
        else
        {
            return GetLevelConfig(CurrentLevel);
        }
    }

    public LevelConfigSO GetLevelConfig(int levelId)
    {
        if (CurrentLevel <= 20)
        {
            return levelConfigSO.GetLevelConfig(levelId);
        }
        else
        {
            return progressionManager.GetNewFakeLevelConfig(levelId);
        }
    }


    public LevelConfigSO GetNewIterationConfig()
    {
        iterationNumber++;
        Debug.Log("New iteration: " + iterationNumber);
        googleEventsReporter.ReportIteration(iterationNumber);
        return progressionManager.GetNewEndlessGameIterationConfig(iterationNumber);
    }

    public void SetConnectedToGooglePlayServicesStatus(bool status)
    {
        isConnectedToGooglePlayServices = status;
    }
}