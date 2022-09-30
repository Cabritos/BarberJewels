using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JewelTemplatesList", menuName = "Scriptable Objects/Jewel Templates List")]
public class JewelTemplatesListSO : ScriptableObject
{
    //The class is called list, but it is an array
    [SerializeField] JewelTemplateSO[] typesArray;

    public JewelTemplateSO[] GetJewelTemplates()
    {
        return typesArray;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CheckForDuplicates();
        CheckForWrongOrder();
    }

    private void CheckForDuplicates()
    {
        foreach (var typeToCheck in typesArray)
        {
            int count = 0;

            foreach (var typeInList in typesArray)
            {
                if (typeToCheck.jewelType == typeInList.jewelType) count++;
            }

            if (count > 1) Debug.LogError("Duplicate type SO loaded: " + typeToCheck);
        }
    }

    private void CheckForWrongOrder()
    {
        for (int i = 0; i < typesArray.Length; i++)
        {
            if (typesArray[i].Id != i)
            {
                Debug.LogError("Template out of order in Template List: " + typesArray[i].Id);
            }
        }
    }
#endif
}
