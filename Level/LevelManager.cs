using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    public event Action<float> ResetingPositions;
    public event Action Rewinding;
    public event Action<float> SlowingDown;
    public event Action StartingLevel;

    public Score Score { get; private set; }
    public Lives Lives { get; private set; }
    public JewelManager JewelManager { get; private set; }
    public BirdSpawner BirdSpawner { get; private set; }
    public FxSpawner FxSpawner { get; private set; }
    public ObjectsPool ObjectsPool { get; private set; }
    public Pause Pause { get; private set; }

    [SerializeField] LevelUI levelUI;
    [SerializeField] MainMovement mainAxisMovement;
    [SerializeField] MainMovement poleMovement;
    float initialVerticalOffset;

    LevelConfigSO currentLevelConfig;

    private void Awake()
    {
        CacheReferences();
        GameManager.Instance.SetupNewLevel(this);
    }

    private void CacheReferences()
    {
        Score = GetComponent<Score>();
        Lives = GetComponent<Lives>();
        JewelManager = GetComponent<JewelManager>();
        BirdSpawner = GetComponent<BirdSpawner>();
        FxSpawner = GetComponent<FxSpawner>();
        ObjectsPool = GetComponent<ObjectsPool>();
        Pause = GetComponent<Pause>();
    }

    private void Start()
    {
        GetCurrentLevelConfig();
        SetupCanvas();
        SetupLevel();
        SetInitialVerticalOffset();
        JewelManager.StartLevel();
        SoundManager.Instance.PlayLevelMusic();
    }

    private void SetupLevel()
    {
        Debug.Log("Setting up level");
        SetupPoleAndCamera();
        SetupJewelsManager();
        SetupBirdsSpawner();
        Debug.Log("Level setup completed");
    }

    private void GetCurrentLevelConfig()
    {
        currentLevelConfig = GameManager.Instance.GetCurrentConfig();
    }

    private void SetupCanvas()
    {
        if (currentLevelConfig.EndlessMode)
        {
            levelUI.SetToEndlessMode();
        }
    }

    private void SetupPoleAndCamera()
    {
        mainAxisMovement.SetVerticalSpeed(currentLevelConfig.VerticalSpeed);
        mainAxisMovement.SetRotationSpeed(currentLevelConfig.RotationSpeed);

        poleMovement.SetVerticalSpeed(currentLevelConfig.VerticalSpeed);
        poleMovement.SetRotationSpeed(currentLevelConfig.RotationSpeed);
    }

    private void SetupJewelsManager()
    {
        JewelManager.SetConfiguration(currentLevelConfig);
    }

    private void SetupBirdsSpawner()
    {
        BirdSpawner.SetupBirdSpawner(
            currentLevelConfig.PlayableBirds,
            currentLevelConfig.BirdsMinRandomSpawnTime,
            currentLevelConfig.BirdsMaxRandomSpawnTime);
    }

    public void NewIteration()
    {
        ResetPositions();
        currentLevelConfig = GameManager.Instance.GetNewIterationConfig();
        SetupLevel();
    }

    private void ResetPositions()
    {
        var resetDistance = GetResetDistance();

        ResetingPositions?.Invoke(resetDistance);
    }

    private void SetInitialVerticalOffset()
    {
        initialVerticalOffset = mainAxisMovement.transform.position.y;
    }

    private float GetResetDistance()
    {
        return -initialVerticalOffset + mainAxisMovement.transform.position.y;
    }

    public void StartLevel()
    {
        StartingLevel?.Invoke();
        StartingLevel = null;
    }

    public void Rewind()
    {
        Rewinding?.Invoke();
    }

    public void SlowDown(float duration)
    {
        SlowingDown?.Invoke(duration);
    }

    public int GetCurrentLevel()
    {
        return currentLevelConfig.Id;
    }

    public void EndLevel(bool lost)
    {
        if (lost)
        {
            Debug.Log($"Level lost with {Score.CurrentScore} points and {JewelManager.JewelHits} jewels destroyed");
            GameManager.Instance.LostLevel(Score.CurrentScore, JewelManager.JewelHits);
        }
        else
        {
            Debug.Log($"Level won with {Score.CurrentScore} points and {JewelManager.JewelHits} jewels destroyed");
            GameManager.Instance.WonLevel(Score.CurrentScore, JewelManager.JewelHits);
        }
    }

    private void OnDisable()
    {
        ResetingPositions = null;
        Rewinding = null;
        SlowingDown = null;
    }
}