using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public event Action<bool> GamePaused;

    [SerializeField] GameObject pauseMenu;

    bool gameHasStarted = false;
    bool isPaused = true;

    private void OnEnable()
    {
        GetComponent<LevelManager>().StartingLevel += StartLevel;
    }

    private void StartLevel()
    {
        GetComponent<LevelManager>().StartingLevel -= StartLevel;
        gameHasStarted = true;
        UnpauseGame();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && gameHasStarted)
        {
            PauseGame();
        }
    }

    public void TogglePause()
    {
        if (!isPaused)
        {
            PauseGame();
        }
        else
        {
            UnpauseGame();
        }
    }

    private void PauseGame()
    {
        Debug.Log("Game paused");
        GameManager.Instance.HideBannerAd();
        Time.timeScale = 0;
        isPaused = true;
        ShowMenu();
        GamePaused?.Invoke(isPaused);
    }

    private void ShowMenu()
    {
        if (gameHasStarted)
        {
            pauseMenu.SetActive(true);
        }
    }

    public void UnpauseGame()
    {
        if (gameHasStarted) ResumeGame();
    }

    public void ResumeGame()
    {
        Debug.Log("Game unpaused");
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        GamePaused?.Invoke(isPaused);
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    private void OnDestroy()
    {
        GamePaused = null;
    }
}