using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    bool isConnectedToGooglePlayServices = false;
    public BannerAd BannerAd { get; private set; }
    AchivementsManager achivementManager;
    GoogleEventsReporter googleEventsReporter;
    ErrorUi errorUi;
    SaveManager saveManager;

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
        BannerAd = GetComponent<BannerAd>();
        googleEventsReporter = GetComponent<GoogleEventsReporter>();
        saveManager = GetComponent<SaveManager>();
    }
    private void Start()
    {
        BannerAd.LoadAndShowAd();
    }

    public void StartNewRegularGame()
    {
        Debug.Log("Starting new regular game"); ;
        SetupRegularGame();
        achivementManager.ReportNewGame();
        LoadNextLevel();
    }

    private void SetupRegularGame()
    {
        EndlessGame = false;
        HasLost = false;
        CurrentLevel = 0;
        ResetScores();
#if UNITY_EDITOR
        if (onLevelDebug) CurrentLevel = debugLevel - 1;
#endif
    }

    public void LoadNextLevel()
    {
        CurrentLevel++;
        googleEventsReporter.ReportLevel(CurrentLevel);
        saveManager.ClearSavedData();
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
        SetupEndlessGame(); achivementManager.ReportNewGame();
        achivementManager.ReportEndlessGame();
        LoadLevelScene();
    }

    private void SetupEndlessGame()
    {
        EndlessGame = true;
        HasLost = false;
        ResetScores();
        iterationNumber = 0;

#if UNITY_EDITOR
        if (onLevelDebug) iterationNumber = debugLevel - 1;
#endif
    }

    private void ResetScores()
    {
        LevelScore = 0;
        TotalScore = 0;
        PreviousTotalScore = 0;
    }

    public void HideBannerAd()
    {
        BannerAd.HideAd();
    }

    public bool HasSavedGame()
    {
        return saveManager.HasSavedGame();
    }

    public void ContinueGame()
    {
        SetupRegularGame();
        var saveData = saveManager.LoadGame();
        CurrentLevel = saveData.level;
        TotalScore = saveData.score;
        LoadNextLevel();
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
        ProcessScores(score, jewelHits);
        SaveGame();
        EndLevel();
    }

    public void LostLevel(int score, int jewelHits)
    {
        HasLost = true;
        saveManager.ClearSavedData();
        ProcessScores(score, jewelHits);
        achivementManager.ReportLostGame();
        EndLevel();
    }

    private void EndLevel()
    {
        Debug.Log("Ending level");
        Time.timeScale = 1;
        ReportProgress();
        SoundManager.Instance.EndLevel();
        LoadBalanceScene();
    }

    private void LoadBalanceScene()
    {
        SceneManager.LoadScene("Balance");
        BannerAd.LoadAndShowAd();
    }

    private void ReportProgress()
    {
        if (ValidateConnectivity())
        {
            ReportScore();
            ReportAchivements();
        }
    }

    private void SaveGame()
    {
        if (!EndlessGame)
        {
            var saveData = new SaveManager.SaveData(CurrentLevel, TotalScore);
            saveManager.SaveGame(saveData);
        }
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
        BannerAd.LoadAndShowAd();
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