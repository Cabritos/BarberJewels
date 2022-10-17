using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BirdSpawner : MonoBehaviour
{
    public event Action<ColorType> BirdHit;
    ObjectsPool objectsPool;

    LevelManager levelManager;
    Lives lives;

    [SerializeField] GameObject prefab;
    [SerializeField] public Transform birdsParent;
    [SerializeField] JewelTemplatesListSO jewelTemplatesListSO;

    protected Dictionary<ColorType, Color> birdsColorsDictionary = new Dictionary<ColorType, Color>();

    private int playableBirds;

    [SerializeField] float speed;

    [SerializeField] float initalXPosition;
    [SerializeField] float maxRandomHeight;
    [SerializeField] float minRandomHeight;
    [SerializeField] float zPosition = -10;

    [SerializeField] float minRandomSpawnTime;
    [SerializeField] float maxRandomSpawnTime;
    float currentRandomWait;

    bool isPaused = true;
    bool isSpawning = true;
    bool isOnMenu = false;

    private void Awake()
    {
        MainMenuSetup();
        CacheReferences();
    }

    private void MainMenuSetup()
    {
        isOnMenu = SceneManager.GetActiveScene().name == "MainMenu";

        if (isOnMenu)
        {
            playableBirds = 9;
            GenerateBirsColorsDictionary();
        }
    }

    private void GenerateBirsColorsDictionary()
    {
        birdsColorsDictionary.Clear();

        var jewelTemplates = jewelTemplatesListSO.GetJewelTemplates();

        for (int i = 1; i < playableBirds; i++)
        {
            birdsColorsDictionary.Add(jewelTemplates[i].jewelType, jewelTemplates[i].color);
        }

        birdsColorsDictionary.Add(ColorType.black, jewelTemplates[0].color);
    }

    private void CacheReferences()
    {
        objectsPool = GetComponent<ObjectsPool>();

        if (isOnMenu)
        {
            levelManager = null;
        }
        else
        {
            levelManager = GetComponent<LevelManager>();
            lives = levelManager.Lives;
        }
    }

    private void OnEnable()
    {
        SubscribeToPauseEvent();
    }

    private void SubscribeToPauseEvent()
    {
        var pause = GetComponent<Pause>();
        if (pause == null) return;
        pause.GamePaused += HandlePause;
    }

    private void HandlePause(bool isPaused)
    {
        this.isPaused = isPaused;
    }

    public void SetupBirdSpawner(int playableBirds, float minRandomSpawnTime, float maxRandomSpawnTime)
    {
        if (playableBirds == 0)
        {
            isSpawning = false;
        }
        else
        {
            this.playableBirds = playableBirds;
            this.minRandomSpawnTime = minRandomSpawnTime;
            this.maxRandomSpawnTime = minRandomSpawnTime;

            GenerateBirsColorsDictionary();
            isSpawning = true;
        }
    }

    private IEnumerator Start()
    {
        NewCurrentRandomWait();

        while (isSpawning || isOnMenu)
        {
            yield return new WaitForSeconds(currentRandomWait);

            if (!isPaused || isOnMenu) SpawnRandomizedBird(!isOnMenu);

            NewCurrentRandomWait();
        }
    }

    private void NewCurrentRandomWait()
    {
        currentRandomWait = UnityEngine.Random.Range(minRandomSpawnTime, maxRandomSpawnTime);
    }


    public GameObject SpawnBird(bool sound)
    {
        return SpawnRandomizedBird(sound);
    }

    public void SpawnBirds(int ammount)
    {
        Transform[] birds = new Transform[ammount];

        for (int i = 0; i < ammount; i++)
        {
            var bird = SpawnRandomizedBird(false);
            bird.GetComponent<Bird>().StopMovement();
            birds[i] = bird.transform;

            if (i == 0) continue;

            while (bird.transform.position.y < birds[i - 1].position.y + 0.8f && bird.transform.position.y > birds[i - 1].position.y - 0.8f)
            {
                bird.transform.position = new Vector3(bird.transform.position.x, GetRandomHeight(), bird.transform.position.z);
                birds[i] = bird.transform;
            }
        }

        StartCoroutine(RelaseBirdsRoutine(birds));
        currentRandomWait += 7;
    }

    private IEnumerator RelaseBirdsRoutine(Transform[] birds)
    {
        foreach (var bird in birds)
        {
            bird.GetComponent<Bird>().StartMovement();
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 3f));
        }
    }

    private GameObject SpawnRandomizedBird(bool sound)
    {
        var fliesToRight = GetRandomDirection();
        var position = GetStartingPosition(fliesToRight);
        var birdGameObject = GetBirdFromPool(position);
        var birdComponent = ApplyRandomTypeBirdComponent(birdGameObject);

        birdComponent.ReleaseNewBird(levelManager, objectsPool, speed, fliesToRight, isOnMenu);

        if (sound) PlaySpawnSound();
        return birdGameObject;
    }

    private bool GetRandomDirection()
    {
        return UnityEngine.Random.value > 0.5f;
    }

    private Vector3 GetStartingPosition(bool fliesToRight)
    {
        var x = fliesToRight ? -initalXPosition : initalXPosition;
        var y = GetRandomHeight();
        var position = new Vector3(x, y, zPosition);
        return position;
    }

    private float GetRandomHeight()
    {
        return UnityEngine.Random.Range(minRandomHeight, maxRandomHeight);
    }

    private GameObject GetBirdFromPool(Vector3 position)
    {
        var bird = objectsPool.Instantiate(prefab, position, Quaternion.identity, birdsParent);

        bird.transform.localPosition = position;
        bird.transform.localRotation = Quaternion.Euler(0, 0, 0);

        return bird;
    }

    private Bird ApplyRandomTypeBirdComponent(GameObject bird)
    {
        var type = GetRandomBirdType();
        Bird birdComponent = AddBirdComponent(bird, type);
        birdComponent.SetType(type, birdsColorsDictionary[type]);

        return birdComponent;
    }

    private ColorType GetRandomBirdType()
    {
        var maxValue = playableBirds;

        if (!isOnMenu && lives.GetLives() == 1)
        {
            maxValue =  playableBirds + 1;
        }

        var index = UnityEngine.Random.Range(0, maxValue);

        if (index == 0) return ColorType.black;
        if (index == playableBirds + 1) return ColorType.blue;
        return (ColorType)index;
    }

    private Bird AddBirdComponent(GameObject gameObject, ColorType type)
    {
        Bird birdComponent = gameObject.GetComponent<Bird>();

        if (birdComponent != null && birdComponent.Type == type) return birdComponent;
       
        Destroy(birdComponent);
        
        switch (type)
        {
            case ColorType.nullColor:
                Debug.LogError("Null color asinged to bird");
                break;
            case ColorType.red:
                birdComponent = gameObject.AddComponent<RedBird>();
                break;
            case ColorType.blue:
                birdComponent = gameObject.AddComponent<BlueBird>();
                break;
            case ColorType.yellow:
                birdComponent = gameObject.AddComponent<YellowBird>();
                break;
            case ColorType.green:
                birdComponent = gameObject.AddComponent<GreenBird>();
                break;
            case ColorType.white:
                birdComponent = gameObject.AddComponent<WhiteBird>();
                break;
            case ColorType.purple:
                birdComponent = gameObject.AddComponent<PurpleBird>();
                break;
            case ColorType.pink:
                birdComponent = gameObject.AddComponent<PinkBird>();
                break;
            case ColorType.orange:
                birdComponent = gameObject.AddComponent<OrangeBird>();
                break;
            case ColorType.black:
                birdComponent = gameObject.AddComponent<BlackBird>();
                break;
            default:
                break;
        }

        return birdComponent;
    }

    private void PlaySpawnSound()
    {
        SoundManager.Instance.PlayBirdSpawned();
    }

    public void NotifyBirdHit(ColorType type)
    {
        BirdHit?.Invoke(type);
    }

    private void OnDisable()
    {
        UnsubscribeToPauseEvent();
        StopAllCoroutines();
    }

    private void UnsubscribeToPauseEvent()
    {
        Pause pause;
        TryGetComponent(out pause);
        if (pause == null) return;
        pause.GamePaused -= HandlePause;
    }

    private void OnDestroy()
    {
        BirdHit = null;
    }
}