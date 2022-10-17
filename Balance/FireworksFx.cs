using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworksFx : MonoBehaviour, IRecyclable
{
    private ParticleSystem particles;
    private ObjectsPool objectsPool;

    private void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        particles.Play();
        Invoke(nameof(Recycle), particles.main.duration);
    }

    public void Recycle()
    {
        objectsPool.Recycle(gameObject);
    }

    public void SetObjectsPool(ObjectsPool objectsPool)
    {
        this.objectsPool = objectsPool;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}