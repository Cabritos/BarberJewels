using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelManager : MonoBehaviour
{
    public event Action<ColorType, int> JewelHit;
    public event Action<int> RemainingJewelsUpdated;

    LevelManager levelManager;
    Score score;
    Lives lives;
    FxSpawner fxSpawner;
    public ObjectsPool ObjectsPool { get; private set; }

    public int PlayableJewels { get; private set; }
    public int JewelHits { get; private set; }
    [SerializeField] float ringRadius;
    int remaningJewels;
    float jewelsPerRing;
    float ringHeight;
    public const float TAU = 6.28318f;

    [SerializeField] Transform jewelsParent;
    [SerializeField] JewelTemplatesListSO jewelTemplatesList;
    Queue<Vector3> remainingJewelPositionsQueue = new Queue<Vector3>();
    private Vector3 nextJewelPosition = new Vector3();
    List<Transform> currentJewels = new List<Transform>();
    Transform bottomJewel;

    [SerializeField] Transform topBoundary;
    [SerializeField] Transform bottomBoundary;

    bool isEndlessMode = false;
    bool gameIsPaused = true;

    private void Awake()
    {
        CacheReferences();
    }

    private void CacheReferences()
    {
        score = GetComponent<Score>();
        lives = GetComponent<Lives>();
        fxSpawner = GetComponent<FxSpawner>();
        levelManager = GetComponent<LevelManager>();
        ObjectsPool = GetComponent<ObjectsPool>();
    }

    public void StartLevel()
    {
        ObjectsPool.GenerateJewelsPools(PlayableJewels);

        RemainingJewelsUpdated?.Invoke(remaningJewels);

        EnqueJewelPositions();
        IfNeededCreateJewelsInGameArea();
    }

    public void SetConfiguration(LevelConfigSO levelConfig)
    {
        Debug.Log("Setting up jewels manager configuration");
        isEndlessMode = levelConfig.EndlessMode;

        remaningJewels = levelConfig.JewelsAmmount;
        RemainingJewelsUpdated?.Invoke(remaningJewels);

        ringHeight = levelConfig.RingHeight;
        jewelsPerRing = levelConfig.JewelsPerRing;
        PlayableJewels = levelConfig.PlayableJewels;
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        var pause = GetComponent<Pause>();
        pause.GamePaused += OnPausedGame;

        var playerInput = GetComponent<PlayerInput>();
        playerInput.PowerButtonPressed += HitAllJewelsOfColor;

        levelManager.ResetingPositions += ResetToInitialPosition;
    }

    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    private void UnsubscribeToEvents()
    {
        var pause = GetComponent<Pause>();
        pause.GamePaused -= OnPausedGame;

        var playerInput = GetComponent<PlayerInput>();
        playerInput.PowerButtonPressed -= HitAllJewelsOfColor;

        levelManager.ResetingPositions -= ResetToInitialPosition;
    }

    private void OnPausedGame(bool isPaused)
    {
        gameIsPaused = isPaused;
    }

    private void Update()
    {
        if (gameIsPaused) return;

        if (currentJewels.Count > 0)
        {
            RemoveBottomJewels();
        }

        if (remaningJewels > 0)
        {
            IfNeededCreateJewelsInGameArea();
            return;
        }
        else if (isEndlessMode)
        {
            levelManager.NewIteration();
            EnqueJewelPositions();
        }
        else if (currentJewels.Count == 0)
        {
            levelManager.EndLevel(false);
        }
    }

    private void ResetToInitialPosition(float distance)
    {            
        foreach (var jewel in currentJewels)
        {
            var position = jewel.transform.position;
            jewel.transform.position = new Vector3(position.x, position.y - distance, position.z);
        }
    }
    private void EnqueJewelPositions()
    {
        foreach (var jewelPosition in CalculateJewelPositions())
        {
            remainingJewelPositionsQueue.Enqueue(jewelPosition);
        }

        nextJewelPosition = remainingJewelPositionsQueue.Peek();
    }

    float offset = 0;
    private IEnumerable<Vector3> CalculateJewelPositions()
    {
        var verticalDistance = ringHeight / jewelsPerRing;

        for (int i = 0; i < remaningJewels + 1; i++)
        {
            var angleInTurns = i / jewelsPerRing - 1;
            angleInTurns += offset;
            var angleInRadians = angleInTurns * TAU;

            var x = Mathf.Cos(angleInRadians) * ringRadius;
            var z = Mathf.Sin(angleInRadians) * ringRadius;

            var y = verticalDistance * i;

            var result = new Vector3(x, y, z);

            if (i == remaningJewels)
            {
                offset = angleInTurns;
                yield break;
            }

            yield return result;
        }
    }

    private void IfNeededCreateJewelsInGameArea()
    {
        bool jewelsAreInPlayHeight = true;

        while (jewelsAreInPlayHeight)
        {
            if (nextJewelPosition.y < topBoundary.position.y && remaningJewels > 0)
            {
                InstantiateNextRandomJewel();
            }
            else
            {
                jewelsAreInPlayHeight = false;
            }
        }
    }

    public bool NoJewelsAreInGameArea()
    {
        return currentJewels.Count == 0 || !IsInGameArea(bottomJewel);
    }

    private bool IsInGameArea(Transform transform)
    {
        return transform.position.y < topBoundary.position.y;
    }

    private void InstantiateNextRandomJewel()
    {
        var type = GetRandomJewelType();
        InstantiateNextJewel(type);
    }

    private void InstantiateNextJewel(ColorType type)
    {
        InstantiateNewJewel(type, nextJewelPosition);
        UpdateRemainingJewelsQueue();
    }

    private void InstantiateNewJewel(ColorType type, Vector3 position)
    {
        var prefab = GetJewelPrefabFromTemplates(type);
        var jewel = ObjectsPool.Instantiate(prefab, position, prefab.transform.rotation, jewelsParent);

        jewel.GetComponent<Jewel>().Initialize(type, this);

        currentJewels.Add(jewel.transform);
        UpdateBottomJewel();
    }

    private ColorType GetRandomJewelType()
    {
        var jewelTemplates = jewelTemplatesList.GetJewelTemplates();
        var templateType = jewelTemplates[UnityEngine.Random.Range(1, PlayableJewels + 1)];

        return templateType.jewelType;
    }

    private GameObject GetJewelPrefabFromTemplates(ColorType type)
    {
        foreach (var template in jewelTemplatesList.GetJewelTemplates())
        {
            if (template.jewelType == type)
            {
                return template.prefab;
            }
        }

        Debug.LogError("Can't find a matching prefab for this type");
        return null;
    }

    private void UpdateRemainingJewelsQueue()
    {
        if (remainingJewelPositionsQueue.Peek() == null) levelManager.EndLevel(false);

        remainingJewelPositionsQueue.Dequeue();
        remaningJewels--;
        RemainingJewelsUpdated?.Invoke(remaningJewels);

        if (remaningJewels > 0)
        {
            nextJewelPosition = remainingJewelPositionsQueue.Peek();
        }
    }

    public void HandleJewelHit(Jewel jewel)
    {
        var finalScore = score.JewelDestroyed(jewel.Type, jewel.GetScore());
        var position = jewel.gameObject.transform.position;

        RemoveJewel(jewel.gameObject);
        JewelHit?.Invoke(jewel.Type, finalScore.ComboMultiplier);
        JewelHits++;

        InstantiatePointsFx(finalScore.Score, finalScore.ComboMultiplier, position);
    }

    private void InstantiatePointsFx(int score, int comboMultiplier, Vector3 position)
    {
        var multipleofFive = comboMultiplier % 5 == 0;
        fxSpawner.DisplayPointsFx(score, multipleofFive, position);
    }

    private void UpdateBottomJewel()
    {
        if (currentJewels.Count == 0) return;

        bottomJewel = currentJewels.First();

        foreach (var jewel in currentJewels)
        {
            if (jewel.transform.position.y < bottomJewel.position.y)
            {
                bottomJewel = jewel;
            }
        }
    }

    private void RemoveBottomJewels()
    {
        if (currentJewels.Count == 0) return;

        if (bottomJewel.position.y < bottomBoundary.position.y)
        {
            JewelLost();
        }
    }

    private void JewelLost()
    {
        var remainingLives = lives.RemoveLives(1);
        RemoveJewel(bottomJewel.gameObject);
        SoundManager.Instance.PlayJewelLostClip();

        if (remainingLives <= 0) levelManager.EndLevel(true);
    }

    private void RemoveJewel(GameObject jewelGameObject)
    {
        var wasBottom = (jewelGameObject.transform == bottomJewel);

        currentJewels.Remove(jewelGameObject.transform);
        jewelGameObject.GetComponent<Jewel>().Recycle();

        if (wasBottom) UpdateBottomJewel();
    }

    public void HitAllJewels(int ammount)
    {
        StartCoroutine(HitAllJewelsRoutine(ammount));
    }

    private IEnumerator HitAllJewelsRoutine(int ammount)
    {
        for (int i = 0; i < ammount; i++)
        {
            if (currentJewels.Count == 0) yield break;

            HandleJewelHit(bottomJewel.gameObject.GetComponent<Jewel>());
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void HitAllJewelsOfColor(ColorType type)
    {
        SoundManager.Instance.PlayOrangePowerClip();
        StartCoroutine(HitAllJewelsOfColorRoutine(type));
    }

    private IEnumerator HitAllJewelsOfColorRoutine(ColorType type)
    {
        if (currentJewels.Count == 0) yield break;

        IEnumerable<Transform> currentJewelsTransforms = currentJewels.OrderBy(jewel => jewel.transform.position.y);

        foreach (var jewelTransform in currentJewelsTransforms)
        {
            var jewel = jewelTransform.GetComponent<Jewel>();

            if (jewel.Type == type)
            {
                HandleJewelHit(jewel);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    public IEnumerable<Transform> ReturnAllJewelTranformsByColor(ColorType type)
    {
        if (currentJewels.Count == 0) yield return null;

        IEnumerable<Transform> currentJewelsTransforms = currentJewels.OrderBy(jewel => jewel.transform.position.y);

        foreach (var jewelTransform in currentJewelsTransforms)
        {
            var jewel = jewelTransform.GetComponent<Jewel>();

            if (jewel.Type == type)
            {
                yield return jewelTransform;
            }
        }
    }

    public IEnumerable<Transform> ReturnAllJewelTranformsInGameArea()
    {
        if (currentJewels.Count == 0) yield return null;

        IEnumerable<Transform> currentJewelsTransforms = currentJewels;

        foreach (var jewelTransform in currentJewelsTransforms)
        {
            if (IsInGameArea(jewelTransform))
            yield return jewelTransform;
        }
    }

    public void ReplaceSomeJewels()
    {
        StartCoroutine(ReplaceJewelsRoutine());
    }

    private IEnumerator ReplaceJewelsRoutine()
    {
        var type = GetRandomJewelType();

        for (int i = 0; i < currentJewels.Count; i++)
        {
            if (UnityEngine.Random.Range(0, 9) > 7) continue;

            var jewel = currentJewels[i];
            if (!IsInGameArea(jewel.transform)) continue;

            var position = jewel.position;
            RemoveJewel(jewel.gameObject);
            InstantiateNewJewel(type, position);

            fxSpawner.DisplayFx(position);
            yield return new WaitForSeconds(0.03f);
        }
    }

    public JewelTemplatesListSO GetJewelTemplatesListSO()
    {
        return jewelTemplatesList;
    }

    private void OnDestroy()
    {
        JewelHit = null;
        RemainingJewelsUpdated = null;
    }
}
