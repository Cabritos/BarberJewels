using System;
using TMPro;
using UnityEngine;

public class YellowBirdPower : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;
    JewelManager jewelManager;
    FxSpawner fxSpawner;

    TMP_Text text;
    int remainingPowers = 0;

    [SerializeField] Transform ring1;
    [SerializeField] Transform ring2;
    [SerializeField] float speed1;
    [SerializeField] float speed2;

    private void Awake()
    {
        fxSpawner = levelManager.FxSpawner;
        jewelManager = levelManager.JewelManager;
        text = GetComponentInChildren<TMP_Text>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        ring1.Rotate(Vector3.forward * speed1 * Time.deltaTime);
        ring2.Rotate(Vector3.forward * speed2 * Time.deltaTime);
    }
    
    public void ReleaseYellowBirdPower()
    {
        if (jewelManager.NoJewelsAreInGameArea()) return;

        SpawnPower();
        RemoveOne();
    }

    private void RemoveOne()
    {
        remainingPowers--;
        UpdateUI();

        if (remainingPowers == 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void SpawnPower()
    {
        fxSpawner.ReleaseTrail();
    }
    
    public void PowerWon()
    {
        remainingPowers++;
        UpdateUI();
        SoundManager.Instance.PlayYellowPowerWonClip();
    }
    
    private void UpdateUI()
    {
        if (remainingPowers <= 1)
        {
            text.text = "";
        }
        else
        {
            text.text = "x" + remainingPowers;
        }
    }
}
