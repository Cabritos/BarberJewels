using System.Collections;
using TMPro;
using UnityEngine;

public class PowerButtonAnimation : MonoBehaviour
{
    TMP_Text text;
    float size;
    [SerializeField] int sizeVariation = 10;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        size = text.fontSize;
    }

    private void OnEnable()
    {
        text.fontSize = size;
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        while (true)
        {   
            text.fontSize += sizeVariation;
            yield return new WaitForSeconds(0.1f);
            text.fontSize -= sizeVariation;
            yield return new WaitForSeconds(0.1f);
            text.fontSize += sizeVariation;
            yield return new WaitForSeconds(0.1f);
            text.fontSize -= sizeVariation;    
            yield return new WaitForSeconds(0.1f);
            text.fontSize += sizeVariation;
            yield return new WaitForSeconds(0.1f);
            text.fontSize -= sizeVariation;
            yield return new WaitForSeconds(4f);
        }
    }

    private void OnDisable()
    {
        StopCoroutine(Animate());
    }
}
