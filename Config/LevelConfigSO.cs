using UnityEngine;

[CreateAssetMenu(fileName = "ConfigLevel_", menuName = "Scriptable Objects/Level Configuration")]
public class LevelConfigSO : ScriptableObject
{
    [SerializeField] public int Id;
    [SerializeField] public string InternalName;
    [SerializeField] public bool EndlessMode;
    [SerializeField] public float VerticalSpeed;
    [SerializeField] public float RotationSpeed;
    [SerializeField] public float RingHeight;
    [SerializeField] public float JewelsPerRing;
    [SerializeField] public int PlayableJewels;
    [SerializeField] public int JewelsAmmount;
    [SerializeField] public int PlayableBirds;
    [SerializeField] public float BirdsMinRandomSpawnTime;
    [SerializeField] public float BirdsMaxRandomSpawnTime;
}
