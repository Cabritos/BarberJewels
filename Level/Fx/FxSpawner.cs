using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxSpawner : MonoBehaviour
{
    [SerializeField] GameObject earningFxPrefab;
    [SerializeField] Transform scoreTarget;
    [SerializeField] Transform trailOrigin;
    [SerializeField] GameObject trailPrefab;
    LevelManager levelManger;
    ObjectsPool objectsPool;
    JewelManager jewelManager;
    Pause pause;

    private void Awake()
    {
        levelManger = GetComponent<LevelManager>();
        objectsPool = levelManger.ObjectsPool;
        jewelManager = levelManger.JewelManager;
        pause = levelManger.Pause;
    }

    private void OnEnable()
    {
        var playerInput = GetComponent<PlayerInput>();
    }

    private void OnDisable()
    {
        var playerInput = GetComponent<PlayerInput>();
    }

    public void ReleaseTrail()
    {
        var trail  = Instantiate(trailPrefab, trailOrigin.position, Quaternion.identity, trailOrigin);
        var fx = trail.GetComponent<ChainedDestructionFx>();
        
        fx.Setup(jewelManager.ReturnAllJewelTranformsInGameArea(), levelManger);
        SoundManager.Instance.PlayYellowBirdClip();
    }

    public EarningFx DisplayFx(Vector3 position)
    {
        var fx = GetFromPool(position);
        fx.PlayFx(objectsPool);
        return fx;
    }

    public EarningFx DisplayTravelingPointsFx(int score, bool highlighted, Vector3 position)
    {
        var fx = GetFromPool(position);
        fx.ScoreTravelingAnimation(score, highlighted, objectsPool, scoreTarget);
        return fx;
    }

    public EarningFx DisplayPersistentTextOnParent(string text, bool highlighted, float duration, Vector3 position, Transform parent)
    {
        var fx = DisplayPersistentText(text, highlighted, duration, position);
        fx.transform.SetParent(parent);
        return fx;
    }

    public EarningFx DisplayPersistentText(string text, bool highlighted, float duration, Vector3 position)
    {
        var fx = GetFromPool(position);
        fx.StartPersistentTextAnimation(text, highlighted, duration, objectsPool);
        return fx;
    }

    public EarningFx DisplayPointsFxOnParent(int score, bool highlighted, Vector3 position, Transform parent)
    {
        var fx = DisplayPointsFx(score, highlighted, position);
        fx.transform.SetParent(parent);
        return fx;
    }

    public EarningFx DisplayPointsFx(int score, bool highlighted, Vector3 position)
    {
        var fx = GetFromPool(position);
        fx.ScoreTravelingAnimation(score, highlighted, objectsPool, scoreTarget);
        return fx;
    }

    public EarningFx DisplayLivesFxOnParent(int ammount, bool highlighted, Vector3 position, Transform parent)
    {
        var fx = DisplayLivesFx(ammount, highlighted, position);
        fx.transform.SetParent(parent);
        return fx;
    }

    public EarningFx DisplayLivesFx(int ammount, bool highlighted, Vector3 position)
    {
        var fx = GetFromPool(position);
        fx.StartLivesFadingAnimation(ammount, highlighted, objectsPool);
        return fx;
    }

    private EarningFx GetFromPool(Vector3 position)
    {
        GameObject fxGameObject = objectsPool.Instantiate(earningFxPrefab, position, Quaternion.identity, transform.parent);
        var fx = fxGameObject.GetComponent<EarningFx>();
        return fx;
    }
}