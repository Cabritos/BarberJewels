using System;
using System.Collections;
using UnityEngine;

public class FireworksSpawner : MonoBehaviour
{
    public event Action OnFireworks;

    [SerializeField] GameObject prefab;

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
        Vector3 position;
        Quaternion rotation;
        SetRandomPositionAndRotation(out position, out rotation);
        var fireworksGameObject = objectsPool.Instantiate(prefab, position, rotation, transform);

        SetRandomScale(fireworksGameObject);
        InjectObjectsPool(fireworksGameObject);

        OnFireworks?.Invoke();
    }

    private void SetRandomPositionAndRotation(out Vector3 position, out Quaternion rotation)
    {
        position = new Vector3(UnityEngine.Random.Range(-4, 6), UnityEngine.Random.Range(-7, 10), 5);
        rotation = Quaternion.Euler(GetRandomRotation());
    }

    private void InjectObjectsPool(GameObject fireworksGameObject)
    {
        var fireworksFx = fireworksGameObject.GetComponent<FireworksFx>();
        fireworksFx.SetObjectsPool(objectsPool);
    }

    private void SetRandomScale(GameObject fireworksGameObject)
    {
        var randomScale = UnityEngine.Random.Range(minRandomScale, maxRandomScale);
        fireworksGameObject.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }

    private Vector3 GetRandomRotation()
    {
        return new Vector3 (0, 0, UnityEngine.Random.Range(-maxRandomRotation, maxRandomRotation));
    }

    private void OnDestroy()
    {
        OnFireworks = null;
    }
}
