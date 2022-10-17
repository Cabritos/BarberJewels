using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessGameProgressionManager : MonoBehaviour
{
    [SerializeField] LevelConfigListSO endlesLevelConfigList;
    LevelConfigSO endlessBaseConfig;
    [SerializeField] LevelConfigSO level20Config;
    LevelConfigSO currentBaseConfig;
    LevelConfigSO newConfig;

    [SerializeField] ProgressionSettingsSO endlessSettings;
    [SerializeField] ProgressionSettingsSO fakeLevelsSettings;
    ProgressionSettingsSO currentSettings;

    private void Awake()
    {
        endlessBaseConfig = endlesLevelConfigList.GetLevelConfig(0);
        newConfig = endlesLevelConfigList.GetLevelConfig(1);
    }

    public LevelConfigSO GetBaseConfig()
    {
        return endlessBaseConfig;
    }

    public LevelConfigSO GetNewEndlessGameIterationConfig(int iterationNumber)
    {
        currentBaseConfig = endlessBaseConfig;
        currentSettings = endlessSettings;
        newConfig.EndlessMode = true;
        return GetNewConfig(iterationNumber);
    }

    public LevelConfigSO GetNewFakeLevelConfig(int levelNumber)
    {
        currentBaseConfig = level20Config;
        currentSettings = fakeLevelsSettings;
        newConfig.EndlessMode = false;
        newConfig.Id = levelNumber;
        return GetNewConfig(levelNumber - 20);
    }

    private LevelConfigSO GetNewConfig(int multiplier)
    {
        newConfig.VerticalSpeed = currentBaseConfig.VerticalSpeed + (currentSettings.VerticalSpeedIncrement * multiplier);

        newConfig.RotationSpeed = Mathf.Clamp(currentBaseConfig.RotationSpeed + (currentSettings.RotationSpeedIncrement * multiplier), 0, currentSettings.RotationSpeedMaxValue);

        newConfig.RingHeight = Mathf.Clamp(currentBaseConfig.RingHeight - (currentSettings.RingHeightDecrement * multiplier), currentSettings.RingHeightMinValue, Mathf.Infinity);

        newConfig.JewelsPerRing = Mathf.Clamp(currentBaseConfig.JewelsPerRing + (currentSettings.JewelsPerRingDecrement * multiplier), 0, currentSettings.JewelsPerRingMaxValue);

        newConfig.JewelsAmmount = Mathf.Clamp(currentBaseConfig.JewelsAmmount + (currentSettings.AdditionalJewelsAmmount * multiplier), 0, currentSettings.AdditionalJewelsAmmountMaxValue);

        newConfig.BirdsMinRandomSpawnTime = Mathf.Clamp(currentBaseConfig.BirdsMinRandomSpawnTime - (currentSettings.BirdsMinRandomSpawnDecrement * multiplier), currentSettings.BirdsMinRandomSpawnMinValue, Mathf.Infinity);

        newConfig.BirdsMaxRandomSpawnTime = Mathf.Clamp(currentBaseConfig.BirdsMaxRandomSpawnTime - (currentSettings.BirdsMaxRandomSpawnDecrement * multiplier), currentSettings.BirdsMaxRandomSpawnMinValue, Mathf.Infinity);

        return newConfig;
    }
}