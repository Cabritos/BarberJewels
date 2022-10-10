using UnityEngine;
using System;

public class Lives : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] bool debugInmoral;
#endif

    int remainigLives = 5;
    public event Action<int> LivesUpdated;
    public event Action<int> OnLivesWon;

    private void Start()
    {
        LivesUpdated?.Invoke(remainigLives);
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
        LivesUpdated?.Invoke(remainigLives);
        return remainigLives;
    }

    public int RemoveLives(int ammount)
    {
#if UNITY_EDITOR
        if (debugInmoral) remainigLives++;
#endif

        remainigLives -= ammount;
        LivesUpdated?.Invoke(remainigLives);
        return remainigLives;
    }

    private void OnDestroy()
    {
        LivesUpdated = null;
        OnLivesWon = null;
    }
}
