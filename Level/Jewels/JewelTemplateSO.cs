using UnityEngine;

[CreateAssetMenu(fileName = "JewelTemplate_", menuName = "Scriptable Objects/Jewel Template")]
public class JewelTemplateSO : ScriptableObject
{
    public int Id;
    public ColorType jewelType;
    public Color color;
    public GameObject prefab;
}