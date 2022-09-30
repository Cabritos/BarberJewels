using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public event Action<bool> OnPauseGame;

    [SerializeField] GameObject pauseMenu;
    Animator pauseMenuAnimator;

    bool gameHasStarted = false;
    bool isPaused = true;


    private void Awake()
    {
        pauseMenuAnimator = pauseMenu.GetComponent<Animator>();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
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
        Time.timeScale = 0;
        isPaused = true;
        ShowMenu();
        OnPauseGame?.Invoke(isPaused);
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
        OnPauseGame?.Invoke(isPaused);
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    private void OnDestroy()
    {
        OnPauseGame = null;
    }

    public void StartGame()
    {
        gameHasStarted = true;
        UnpauseGame();
    }
}
