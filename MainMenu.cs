using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject newGamePanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject soundPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject supportPanel;
    GameObject currentPanel;

    private void Awake()
    {
        currentPanel = mainPanel;
        currentPanel.SetActive(true);
    }

    private void Start()
    {
        SoundManager.Instance.PlayMenuMusic();

        if (GameManager.Instance.HasSavedGame())
        {
            continueButton.SetActive(true);
        }
    }

    public void DisplayNewGamePanel()
    {
        ReplacePanel(newGamePanel);
    }

    public void StartNewRegularGame()
    {
        GameManager.Instance.StartNewRegularGame();
    }

    public void StartNewEndlessGame()
    {
        GameManager.Instance.StartNewEndlessGame();
    }

    public void ContinueGame()
    {
        GameManager.Instance.ContinueGame();
    }

    public void ReturnToMainPanel()
    {
        ReplacePanel(mainPanel);
    }

    public void DisplayOptionsPanel()
    {
        ReplacePanel(optionsPanel);
    }

    public void DisplaySoundPanel()
    {
        ReplacePanel(soundPanel);
    }

    public void DisplayCreditsPanel()
    {
        ReplacePanel(creditsPanel);
    }

    public void DisplaySupportPanel()
    {
        ReplacePanel(supportPanel);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
    }

    public void DisplayLeaderboardPanel()
    {
        if (GameManager.Instance.ValidateConnectivityOrShowError())
        {
            GooglePlayServicesManager.ShowLeaderboards();
        }
    }

    public void DisplayAchivementsPanel()
    {
        if (GameManager.Instance.ValidateConnectivityOrShowError())
        {
            GooglePlayServicesManager.ShowAchivements();
        }
    }

    public void GoogleManualSignIn()
    {
        GooglePlayServicesManager.GooglePlayManualSignIn();
    }

    private void ReplacePanel(GameObject newOptionsGroup)
    {
        currentPanel.SetActive(false);
        currentPanel = newOptionsGroup;
        currentPanel.SetActive(true);
        SoundManager.Instance.PlayMenuSelectionClip();
    }
}