using System;
using System.Collections;
using UnityEngine;

public class FireworksSpawner : MonoBehaviour
{ 
    [SerializeField] GameObject fxPrefab;
    Vector3 fxPosition = new Vector3();
    Vector3 euerRotation = new Vector3();
    Quaternion fxRotation = new Quaternion();

    [SerializeField] float minRandomSpawnTime;
    [SerializeField] float maxRandomSpawnTime;
    float currentRandom = 0.3f;

    [SerializeField] float minRandomScale;
    [SerializeField] float maxRandomScale;

    [SerializeField] float maxRandomRotation;

    ObjectsPool objectsPool;

    private void Awake()
    {
        objectsPool = GetComponent<ObjectsPool>();
    }

    private IEnumerator Start()
    {
        SetCurrentRandom();

        while (true)
        {
            yield return new WaitForSeconds(currentRandom);
            InstantiateFireworks();
            SetCurrentRandom();
        }
    }

    private void SetCurrentRandom()
    {
        currentRandom = UnityEngine.Random.Range(minRandomSpawnTime, maxRandomSpawnTime);
    }

    private void InstantiateFireworks()
    {
        SetRandomPosition();
        SetRandomRotation();
        var fireworksGameObject = objectsPool.Instantiate(fxPrefab, fxPosition, fxRotation, transform);
        ScaleToRandomScale(fireworksGameObject);
        InjectObjectsPool(fireworksGameObject);
    }

    private void SetRandomPosition()
    {
        fxPosition.x = UnityEngine.Random.Range(-4, 6);
        fxPosition.y = UnityEngine.Random.Range(-7, 10);
        fxPosition.z = 5;
    }

    private void SetRandomRotation()
    {
        euerRotation.z = UnityEngine.Random.Range(-maxRandomRotation, maxRandomRotation);
        fxRotation = Quaternion.Euler(euerRotation);
    }

    private void ScaleToRandomScale(GameObject fireworksGameObject)
    {
        var randomScale = UnityEngine.Random.Range(minRandomScale, maxRandomScale);
        fireworksGameObject.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }

    private void InjectObjectsPool(GameObject fireworksGameObject)
    {
        var fireworksFx = fireworksGameObject.GetComponent<FireworksFx>();
        fireworksFx.SetObjectsPool(objectsPool);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}