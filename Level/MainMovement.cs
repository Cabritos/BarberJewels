using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMovement : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;
    JewelManager jewelManager;

    float verticalSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float rewindSpeed;
    [SerializeField] bool isPole;

    bool gameIsPaused = true;

    float rewindRamainingTime = 0f;
    float slowedEffectRamainingTime = 0f;

    private void Awake()
    {
        jewelManager = levelManager.JewelManager;
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        levelManager.Pause.GamePaused += PausedGame;
        levelManager.ResetingPositions += ResetPosition;
        levelManager.Rewinding += Rewind;
        levelManager.SlowingDown += SlowDown;
    }

    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    private void UnsubscribeToEvents()
    {
        if (levelManager == null) return;

        levelManager.Pause.GamePaused -= PausedGame;
        levelManager.ResetingPositions -= ResetPosition;
        levelManager.Rewinding -= Rewind;
        levelManager.SlowingDown -= SlowDown;
    }

    private void PausedGame(bool isPaused)
    {
        gameIsPaused = isPaused;
    }

    public void SetVerticalSpeed(float verticalSpeed)
    {
        this.verticalSpeed = verticalSpeed;
    }

    public void SetRotationSpeed(float rotationSpeed)
    {
        this.rotationSpeed = rotationSpeed;
    }

    private void Rewind(float duration)
    {
        rewindRamainingTime = duration;
    }

    private void SlowDown(float duration)
    {
        slowedEffectRamainingTime += duration;
    }

    private void Update()
    {
        if (gameIsPaused)
        {
            if (isPole)
            {
                Rotate(rotationSpeed);
            }

            return;
        }

        if (rewindRamainingTime > 0)
        {
            RewindMovement();
        }
        else if (slowedEffectRamainingTime > 0)
        {
            SlowDownMovement();
        }
        else
        {
            RegularMovement();
        }
    }

    private void RegularMovement()
    {
        MoveUp(verticalSpeed);
        Rotate(rotationSpeed);
    }

    private void Rotate(float speed)
    {
        if (isPole)
        {
            speed = gameIsPaused ? rotationSpeed * 1.2f : rotationSpeed * 1.3f;
            transform.Rotate(Vector3.up, speed * Time.unscaledDeltaTime / 10);
        }
        else
        {
            transform.Rotate(Vector3.up, -speed * Time.deltaTime / 10);
        }
    }

    private void InverseRotation(float speed)
    {
        transform.Rotate(Vector3.up, speed * rewindSpeed * Time.deltaTime / 10);
    }

    private void MoveUp(float speed)
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime / 100);
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.down * rewindSpeed * Time.deltaTime);
    }

    private void RewindMovement()
    {
        if (jewelManager.NoJewelsAreInGameArea())
        {
            rewindRamainingTime = 0;
        }
        else
        {
            MoveDown();
            InverseRotation(rotationSpeed);
            rewindRamainingTime -= Time.deltaTime;
        }
    }

    private void SlowDownMovement()
    {
        MoveUp(verticalSpeed / 2);
        Rotate(rotationSpeed / 1.5f);
        slowedEffectRamainingTime -= Time.deltaTime;
    }

    private void ResetPosition(float distance)
    {
        var position = transform.position;
        transform.position = new Vector3(position.x, position.y - distance, position.z);
    }
}