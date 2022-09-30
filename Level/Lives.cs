using UnityEngine;
using System;

public class Lives : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] bool debugInmoral;
#endif

    int remainigLives = 5;
    public event Action<int> OnLivesUpdated;
    public event Action<int> OnLivesWon;

    private void Start()
    {
        OnLivesUpdated?.Invoke(remainigLives);
    }

    public void SetAmmount(int ammount)
    {
        AddLives(ammount);
    }

    public void DebugAddOneLive()
    {
#if UNITY_EDITOR
        AddLives(1);
        Debug.Log("Live added");
#endif
    }

    public int AddLives(int ammount)
    {
        remainigLives += ammount;
        OnLivesWon?.Invoke(ammount);
        OnLivesUpdated?.Invoke(remainigLives);
        return remainigLives;
    }

    public int RemoveLives(int ammount)
    {
#if UNITY_EDITOR
        if (debugInmoral) remainigLives++;
#endif

        remainigLives -= ammount;
        OnLivesUpdated?.Invoke(remainigLives);
        return remainigLives;
    }

    private void OnDestroy()
    {
        OnLivesUpdated = null;
        OnLivesWon = null;
    }
}
