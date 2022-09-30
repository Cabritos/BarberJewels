using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using TMPro;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] BannerAd banner;
    TMP_Text text;
    [SerializeField] string adUnitId = "Interstitial_Android";

    bool adIsLoaded = false;

    private void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        InhabilitateButton();

        if (Application.internetReachability == NetworkReachability.NotReachable) return;

        if (Advertisement.isInitialized)
        {
            LoadAd();
        }
        else
        {
            Debug.LogWarning("InterstitialAd failed load attempt. Advertisment was not initialized");
        }
    }

    private void InhabilitateButton()
    {
        GetComponent<Button>().interactable = false;
        var color = text.color;
        color.a = 0.5f;
        text.color = color;
    }

    private void HabilitateButton()
    {
        GetComponent<Button>().interactable = true;
        var color = text.color;
        color.a = 1;
        text.color = color;
    }

    public void LoadAd()
    {
        Debug.Log("Loading Ad: " + adUnitId);
        Advertisement.Load(adUnitId, this);
    }

    public void ShowAd()
    {
        if (adIsLoaded)
        {
            Debug.Log("Showing Ad: " + adUnitId);
            banner.HideAd();
            Advertisement.Show(adUnitId, this);
        }
        else
        {
            Debug.LogWarning("InterstitialAd couldn't be shown because it wasn't loaded");
        }

    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Add loaded: " + adUnitId);
        adIsLoaded = true;
        HabilitateButton();
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        Debug.Log("Showing ad: " + adUnitId);
    }

    public void OnUnityAdsShowClick(string adUnitId) { }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        banner.ShowBannerAd();
        InhabilitateButton();
        LoadAd();
    }
}