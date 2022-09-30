using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string adUnitId = "Banner_Android";
    bool isShowing = false;

    void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;

        if (Advertisement.isInitialized)
        {
            if (Advertisement.Banner.isLoaded) return;
            LoadBanner();
        }
    }

    public void LoadBanner()
    {
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(adUnitId, options);
    }

    void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");
        ShowBannerAd();
    }

    void OnBannerError(string message)
    {
        Debug.LogError($"Banner Error: {message}");
    }

    public void ShowBannerAd()
    {
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            showCallback = OnBannerShown
        };

        Advertisement.Banner.Hide(true);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(adUnitId, options);
        Debug.Log("Banner shown");
    }

    void OnBannerClicked() { }

    void OnBannerShown()
    {
        isShowing = true;
    }
        
    public void HideAd()
    {
        if (isShowing)
        {
            Advertisement.Banner.Hide(false);
            isShowing = false;
            Debug.Log("Banner hidden");
        }
    }

    private void OnDisable()
    {
        Advertisement.Banner.Hide(true);
        Destroy(gameObject);
    }
}
