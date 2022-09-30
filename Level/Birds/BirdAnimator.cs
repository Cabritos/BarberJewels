using System.Collections;
using UnityEngine;

public class BirdAnimator : MonoBehaviour
{
    [SerializeField] SpriteRenderer topSpriteRender;
    [SerializeField] SpriteRenderer bottomSpriteRenderer;

    [SerializeField] Sprite[] topSprites;
    [SerializeField] Sprite[] bottomSprites;
    int currentSpriteIndex = 0;

    [SerializeField] float timeBetweenFrames;

    private void OnEnable()
    {
        StartCoroutine(FlyAnimation());
    }

    private IEnumerator FlyAnimation()
    {
        while (true)
        {
            topSpriteRender.sprite = topSprites[currentSpriteIndex];
            bottomSpriteRenderer.sprite = bottomSprites[currentSpriteIndex];

            yield return new WaitForSeconds(timeBetweenFrames);
            
            currentSpriteIndex++;
            if (currentSpriteIndex == topSprites.Length) currentSpriteIndex = 0;
        }
    }

    private void OnDisable()
    {
        StopCoroutine(FlyAnimation());
    }
}
