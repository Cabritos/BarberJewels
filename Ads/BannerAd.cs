using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string adUnitId = "Banner_Android";
    bool isShowing = false;

    public void LoadAndShowAd()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;

        if (Advertisement.isInitialized)
        {
            if (Advertisement.Banner.isLoaded)
            {
                ShowBanner();
            }
            else
            {
                LoadBanner();
            }
        }
    }

    private void LoadBanner()
    {
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(adUnitId, options);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");
        ShowBanner();
    }

    private void OnBannerError(string message)
    {
        Debug.LogError($"Banner Error: {message}");
    }

    public void ShowBanner()
    {
        if (SceneManager.GetActiveScene().name == "Level") return;

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

    private void OnBannerClicked()
    {
        HideAd();
    }

    private void OnBannerShown()
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
        StopAllCoroutines();
        Destroy(gameObject);
    }
}