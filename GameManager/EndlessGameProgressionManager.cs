using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessGameProgressionManager : MonoBehaviour
{
    [SerializeField] LevelConfigListSO endlesLevelConfigList;
    LevelConfigSO baseConfig;
    LevelConfigSO newConfig;

    [SerializeField] public float VerticalSpeedIncrement;
    [SerializeField] public float RotationSpeedIncrement;
    [SerializeField] public float RotationSpeedMaxValue;
    [SerializeField] public float RingHeightDecrement;
    [SerializeField] public float RingHeightMinValue;
    [SerializeField] public float JewelsPerRingDecrement;
    [SerializeField] public float JewelsPerRingMaxValue;
    [SerializeField] public int AdditionalJewelsAmmount;
    [SerializeField] public int AdditionalJewelsAmmountMaxValue;
    [SerializeField] public float BirdsMinRandomSpawnDecrement;
    [SerializeField] public float BirdsMinRandomSpawnMinValue;
    [SerializeField] public float BirdsMaxRandomSpawnDecrement;
    [SerializeField] public float BirdsMaxRandomSpawnMinValue;

    int iterationNumber;

    private void Awake()
    {
        baseConfig = endlesLevelConfigList.GetLevelConfig(0);
        newConfig = endlesLevelConfigList.GetLevelConfig(1);

        ResetConfig();
    }

    private void ResetConfig()
    {
        newConfig.VerticalSpeed = baseConfig.VerticalSpeed;
        newConfig.RotationSpeed = baseConfig.RotationSpeed;
        newConfig.RingHeight = baseConfig.RingHeight;
        newConfig.JewelsPerRing = baseConfig.JewelsPerRing;
        newConfig.JewelsAmmount = baseConfig.JewelsAmmount;
        newConfig.BirdsMinRandomSpawnTime = baseConfig.BirdsMinRandomSpawnTime;
        newConfig.BirdsMaxRandomSpawnTime = baseConfig.BirdsMaxRandomSpawnTime;
    }

    public LevelConfigSO GetBaseConfig()
    {
        return baseConfig;
    }

    public LevelConfigSO GetNewConfig(int iterationNumber)
    {
        this.iterationNumber = iterationNumber;

        newConfig.VerticalSpeed = baseConfig.VerticalSpeed + (VerticalSpeedIncrement * iterationNumber);

        newConfig.RotationSpeed = Mathf.Clamp(baseConfig.RotationSpeed + (RotationSpeedIncrement * iterationNumber), 0, RotationSpeedMaxValue);

        newConfig.RingHeight = Mathf.Clamp(baseConfig.RingHeight - (RingHeightDecrement * iterationNumber), RingHeightMinValue, Mathf.Infinity);

        newConfig.JewelsPerRing = Mathf.Clamp(baseConfig.JewelsPerRing + (JewelsPerRingDecrement * iterationNumber), 0, JewelsPerRingMaxValue);

        newConfig.JewelsAmmount = Mathf.Clamp(baseConfig.JewelsAmmount + (AdditionalJewelsAmmount * iterationNumber), 0, AdditionalJewelsAmmountMaxValue);

        newConfig.BirdsMinRandomSpawnTime = Mathf.Clamp(baseConfig.BirdsMinRandomSpawnTime - (BirdsMinRandomSpawnDecrement * iterationNumber), BirdsMinRandomSpawnMinValue, Mathf.Infinity);

        newConfig.BirdsMaxRandomSpawnTime = Mathf.Clamp(baseConfig.BirdsMaxRandomSpawnTime - (BirdsMaxRandomSpawnDecrement * iterationNumber), BirdsMaxRandomSpawnMinValue, Mathf.Infinity);

        return newConfig;
    }
}
