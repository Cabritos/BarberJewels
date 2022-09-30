using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfigList", menuName = "Scriptable Objects/Level Configuration List")]
public class LevelConfigListSO : ScriptableObject
{
    //The class is called list, but it is an array
    [SerializeField] LevelConfigSO[] configArray;

    public LevelConfigSO GetLevelConfig(int levelId)
    {
        if (levelId > configArray.Length)
        {
            Debug.LogError("Level requested Id was higher than levels listed."); //TODO progression
            return (configArray[configArray.Length]);
        }

        return configArray[levelId];
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CheckForDuplicates();
        CheckForWrongOrder();
    }

    private void CheckForDuplicates()
    {
        foreach (var configToCheck in configArray)
        {
            int count = 0;

            foreach (var typeInList in configArray)
            {
                if (configToCheck.Id == typeInList.Id) count++;
            }

            if (count > 1) Debug.LogError("Duplicate type SO loaded: " + configToCheck);
        }
    }

    private void CheckForWrongOrder()
    {
        for (int i = 0; i < configArray.Length; i++)
        {
            if (configArray[i].Id != i)
            {
                Debug.LogError("Template out of order in Template List: " + configArray[i].Id);
            }
        }
    }
#endif
}