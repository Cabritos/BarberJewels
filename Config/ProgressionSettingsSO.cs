using UnityEngine;

[CreateAssetMenu(fileName = "ProgressionSettings_", menuName = "Scriptable Objects/Progression settings")]
public class ProgressionSettingsSO : ScriptableObject
{
    public float VerticalSpeedIncrement;
    public float RotationSpeedIncrement;
    public float RotationSpeedMaxValue;
    public float RingHeightDecrement;
    public float RingHeightMinValue;
    public float JewelsPerRingDecrement;
    public float JewelsPerRingMaxValue;
    public int AdditionalJewelsAmmount;
    public int AdditionalJewelsAmmountMaxValue;
    public float BirdsMinRandomSpawnDecrement;
    public float BirdsMinRandomSpawnMinValue;
    public float BirdsMaxRandomSpawnDecrement;
    public float BirdsMaxRandomSpawnMinValue;
}