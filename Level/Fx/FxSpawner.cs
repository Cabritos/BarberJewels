using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxSpawner : MonoBehaviour
{
    [SerializeField] Transform fxParent;
    [SerializeField] GameObject earningFxPrefab;
    [SerializeField] Transform scoreTarget;
    [SerializeField] Transform trailOrigin;
    [SerializeField] GameObject trailPrefab;
    LevelManager levelManger;
    ObjectsPool objectsPool;
    JewelManager jewelManager;

    private void Awake()
    {
        levelManger = GetComponent<LevelManager>();
        objectsPool = levelManger.ObjectsPool;
        jewelManager = levelManger.JewelManager;
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
        
        fx.Setup(levelManger);
        SoundManager.Instance.PlayYellowBirdClip();
    }

    public EarningFx DisplayFx(Vector3 position)
    {
        var fx = GetFromPool(position);
        fx.transform.SetParent(fxParent);
        fx.PlayFx(objectsPool);
        return fx;
    }

    public EarningFx DisplayTravelingPointsFx(int score, bool highlighted, Vector3 position)
    {
        var fx = GetFromPool(position);
        fx.transform.SetParent(fxParent);
        fx.ScoreTravelingAnimation(score, highlighted, objectsPool, scoreTarget);
        return fx;
    }

    public EarningFx DisplayPersistentText(string text, bool highlighted, float duration, Vector3 position, Transform parent)
    {
        var fx = GetFromPool(position);
        fx.transform.SetParent(fxParent);
        AdjustPosition(fx.transform, 4);
        fx.StartPersistentTextAnimation(text, highlighted, duration, objectsPool);
        return fx;
    }

    public EarningFx DisplayPointsFx(int score, bool highlighted, Vector3 position)
    {
        var fx = GetFromPool(position);
        fx.transform.SetParent(fxParent);
        fx.ScoreTravelingAnimation(score, highlighted, objectsPool, scoreTarget);
        return fx;
    }

    public EarningFx DisplayLivesFx(int ammount, bool highlighted, Vector3 position)
    {
        var fx = GetFromPool(position);
        fx.StartLivesFadingAnimation(ammount, highlighted, objectsPool);
        fx.transform.SetParent(fxParent);
        AdjustPosition(fx.transform, 5);
        return fx;
    }

    private void AdjustPosition(Transform transform, int maxLocalPosition)
    {
        float x = Mathf.Clamp(transform.localPosition.x, -maxLocalPosition, maxLocalPosition);
        Debug.Log(transform.localPosition.x + " " + x);
        transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
    }

    private EarningFx GetFromPool(Vector3 position)
    {
        GameObject fxGameObject = objectsPool.Instantiate(earningFxPrefab, position, Quaternion.identity, transform.parent);
        var fx = fxGameObject.GetComponent<EarningFx>();
        return fx;
    }
}