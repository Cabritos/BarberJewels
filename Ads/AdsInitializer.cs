using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string androidGameId;
    [SerializeField] bool testMode = true;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (Advertisement.isSupported)
        {
            Debug.Log(Application.platform + " supported by Advertisement");
            Advertisement.Initialize(androidGameId, testMode, this);
        }
    }

    public void OnInitializationComplete()
    {   
        Debug.Log("Unity Ads initialization complete. Status: " + Advertisement.isInitialized);
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogWarning($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}