using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bird : MonoBehaviour, IRecyclable, IHitable
{  
    public ColorType Type { get; private set; }
    float speed;
    bool fliesToRight;
    bool isFlying;

    protected LevelManager levelManager;
    protected BirdSpawner birdSpawner;
    protected ObjectsPool objectsPool;
    protected FxSpawner fxSpawner;
    Pause pause;
    SpriteRenderer spriteRenderer;

    bool isOnMenu;
    bool isPaused;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void HandlePause(bool isPaused)
    {
        this.isPaused = isPaused;
    }

    public void SetType(ColorType type, Color color)
    {
        this.Type = type;
        SetSpriteColor(color);
    }

    private void SetSpriteColor(Color color)
    {
        spriteRenderer.color = color;
    }

    public void ReleaseNewBird(LevelManager levelManager, ObjectsPool objectsPool, float speed, bool fliesToRight, bool isOnMenu)
    {
        this.speed = speed;
        this.fliesToRight = fliesToRight;
        this.isOnMenu = isOnMenu;

        SetBirdSpawnerRereferences(objectsPool, levelManager);
        SubscribeToPause();
        FlipSpriteToFlightDirection(fliesToRight);
        StartMovement();
    }

    private void SubscribeToPause()
    {
        if (isOnMenu) return;
        pause.OnPauseGame += HandlePause;
        isPaused = pause.IsPaused();
    }

    private void SetBirdSpawnerRereferences(ObjectsPool objectsPool, LevelManager levelManager)
    {
        this.objectsPool = objectsPool;

        if (levelManager == null) return;

        this.levelManager = levelManager;
        birdSpawner = levelManager.BirdSpawner;
        fxSpawner = levelManager.FxSpawner;
        pause = levelManager.Pause;
    }
        
    private void FlipSpriteToFlightDirection(bool fliesToRight)
    {
        foreach (var spriteRender in GetComponentsInChildren<SpriteRenderer>())
        {
            if (fliesToRight)
            {
                spriteRender.flipX = true;
            }
            else
            {
                spriteRender.flipX = false;
            }
        }
    }

    public void StartMovement()
    {
        isFlying = true;
    }

    public void StopMovement()
    {
        isFlying = false;
    }

    private void Update()
    {
        if (isPaused) return;
        if (!isFlying) return;

        CheckForBoundaries();
        FlyMovement();
    }

    private void CheckForBoundaries()
    {
        var xPosition = transform.localPosition.x;

        if (xPosition < -12 || xPosition > 12)
        {
            Recycle();
        }
    }

    private void FlyMovement()
    {
        if (fliesToRight)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
    }

    public void HandleHit(ColorType selectedType)
    {
        if (selectedType == Type || Type == ColorType.black) HandleCorrectHit();
    }

    public void HandleCorrectHit()
    {
        SoundManager.Instance.PlayBirdDestroyedClip();
        birdSpawner.NotifyBirdHit(Type);
        HitReward();
        Recycle();
    }

    protected virtual void HitReward()
    {

    }

    public void Recycle()
    {
        objectsPool.Recycle(gameObject);
    }

    public void OnDisable()
    {
        if (isOnMenu) return;
        pause.OnPauseGame -= HandlePause;
    }
}