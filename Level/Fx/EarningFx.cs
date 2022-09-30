using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EarningFx : MonoBehaviour, IRecyclable
{
    List<ParticleSystem> particles = new List<ParticleSystem>();
    ObjectsPool objectsPool;

    [SerializeField] GameObject trail;
    [SerializeField] TMP_Text regularText;
    [SerializeField] TMP_Text highlightedText;
    [SerializeField] TMP_Text heart;
    TMP_Text currentScoreText;

    [SerializeField] int maxRandomTilt = 20;
    private int randomTilt;
    bool hasHeart;
    bool rotate;
    Vector3 defaultScale = new Vector3(1, 1, 1);
    Color initialColor = new Color(1, 1, 1, 1f);

    [SerializeField] float rotationSpeed;
    [SerializeField] float fadeDuration;

    Transform target;
    bool isTraveling = false;
    [SerializeField] float flyingSpeed = 0.3f;
    Transform cameraTransform;


    private void Awake()
    {
        cameraTransform = Camera.main.transform;

        foreach (var particle in GetComponentsInChildren<ParticleSystem>())
        {
            particles.Add(particle);
        }
    }

    private void OnEnable()
    {
        SetToDefaultValues();
        InactivateTextsGameObjects();
    }

    private void SetToDefaultValues()
    {
        rotate = false;
        hasHeart = false;

        regularText.color = initialColor;
        highlightedText.color = initialColor;
        heart.color = initialColor;

        transform.localScale = defaultScale;
        regularText.transform.localScale = defaultScale;
        highlightedText.transform.localScale = defaultScale;
        heart.transform.localScale = defaultScale;

        isTraveling = false;
        trail.SetActive(false);
    }

    private void Update()
    {
        if (isTraveling) TravelToTarget();
        
        Rotate();
    }

    private void TravelToTarget()
    {
        transform.LookAt(target);
        transform.Translate(Vector3.forward * Time.deltaTime * flyingSpeed);

        currentScoreText.transform.rotation = Quaternion.LookRotation(currentScoreText.transform.position - cameraTransform.position);

        if (Vector3.Distance(target.position, transform.position) < 0.5f)
        {
            Recycle();
        }
    }

    private void Rotate()
    {
        if (!rotate) return;
        if (currentScoreText == null) return;
        currentScoreText.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        if (hasHeart) heart.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void PlayFx(ObjectsPool objectsPool)
    {
        this.objectsPool = objectsPool;
        PlayParticleEffects();
        Invoke(nameof(Recycle), 3f);
    }

    private void BasicSetup(bool highlighted, ObjectsPool objectsPool)
    {
        this.objectsPool = objectsPool;
        currentScoreText = highlighted ? highlightedText : regularText;
        SetNewRandomRotation();
        PlayParticleEffects();
    }

    private void InactivateTextsGameObjects()
    {
        regularText.gameObject.SetActive(false);
        highlightedText.gameObject.SetActive(false);
        heart.gameObject.SetActive(false);
    }

    private void SetText(TMP_Text TMPtext, string newText)
    {
        ActivateTMPText(TMPtext);
        TMPtext.text = newText;
    }

    private void SetPointsText(TMP_Text text, int score)
    {
        ActivateTMPText(text);
        text.text = "+" + score;
    }

    private void ActivateTMPText(TMP_Text text)
    {
        text.gameObject.SetActive(true);
        SetInnitialPosition(text.transform);
    }

    private void SetInnitialPosition(Transform transform)
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(Vector3.up, 180);
        transform.Rotate(Vector3.forward, randomTilt);
    }

    private void PlayParticleEffects()
    {
        foreach (var particle in particles)
        {
            particle.Play();
        }
    }

    private void SetNewRandomRotation()
    {
        randomTilt = Random.Range(-maxRandomTilt, maxRandomTilt);
    }

    public void StartLivesFadingAnimation(int points, bool highlighted, ObjectsPool objectsPool)
    {
        BasicSetup(highlighted, objectsPool);
        rotate = true;
        SetPointsText(currentScoreText, points);
        StartFadeAnimation(fadeDuration);
        StartHeartFadingAnimation();
    }

    private void StartHeartFadingAnimation()
    {
        hasHeart = true;
        ActivateTMPText(heart);
        StartCoroutine(FadeAnimation(heart, fadeDuration));
        Invoke(nameof(Recycle), fadeDuration);
    }

    public void StartPersistentTextAnimation(string text, bool highlighted, float duration, ObjectsPool objectsPool)
    {
        BasicSetup(highlighted, objectsPool);
        SetText(currentScoreText, text);
        StartFadeAnimation(duration);
        Invoke(nameof(Recycle), duration);
    }

    public void ScoreTravelingAnimation(int points, bool highlighted, ObjectsPool objectsPool, Transform target)
    {
        BasicSetup(highlighted, objectsPool);
        SetPointsText(currentScoreText, points);

        trail.SetActive(true);
        StartTravelAnimation(target);
        StartCoroutine(ShrinkAnimation(currentScoreText, 1.5f));
    }

    private void StartFadeAnimation(float duration)
    {
        StartCoroutine(FadeAnimation(currentScoreText, duration));
    }

    private IEnumerator FadeAnimation(TMP_Text target, float duration)
    {
        var transparentColor = new Color(0, 0, 0, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            target.color = Color.Lerp(Color.white, transparentColor, elapsedTime / duration);
            yield return null;
        }
    }

    private void StartTravelAnimation(Transform target)
    {
        this.target = target;
        isTraveling = true;
    }

    private IEnumerator ShrinkAnimation(TMP_Text target, float duration)
    {
        float elapsedTime = 0f;
        var targetScale = new Vector3(0, 0, 0);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(defaultScale, targetScale, elapsedTime / duration);
            yield return null;
        }
    }

    public void Recycle()
    {
        objectsPool.Recycle(gameObject);
    }
}