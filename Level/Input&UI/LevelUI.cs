using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUI : MonoBehaviour
{
    [SerializeField] GameObject level;

    [SerializeField] TMP_Text jewelsText;
    [SerializeField] TMP_Text jewelsLabel;
    [SerializeField] TMP_Text livesText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject greatigCanvas;
    [SerializeField] Text levelLabel;
    [SerializeField] TMP_Text levelText;
    TMP_Text greatingText;

    [SerializeField] private float greatingsDuration;

    private Dictionary<ColorType, Color> jewelColorsDictionary = new Dictionary<ColorType, Color>();
    [SerializeField] GameObject whiteSelectorButton;
    [SerializeField] GameObject purpleSelectorButton;
    [SerializeField] GameObject pinkSelectorButton;
    [SerializeField] GameObject orangeSelectorButton;

    [SerializeField] Image selectedColorImage;
    [SerializeField] Button powerButton;
    [SerializeField] GameObject powerButtonGameObject;

    [SerializeField] JewelTemplatesListSO jewelTemplatesList;
    [SerializeField] JewelManager jewelManager;

    int jewelsDestroyed = 0;
    bool endlessMode = false;

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        SubscribeToPauseEvent();
        SubscribeToInputEvents();
        SubscribeToScoreEvent();
        SubscribeToLivesEvent();
        SubscribeToRemainingJewels();
        SubscribeToBirdsDestroyed();
    }

    private void SubscribeToPauseEvent()
    {
        var pause = level.GetComponent<Pause>();
        pause.GamePaused += ActivatePauseCanvas;
    }

    private void SubscribeToInputEvents()
    {
        var playerInput = level.GetComponent<PlayerInput>();
        playerInput.ChangedSelection += UpdateSelection;
        playerInput.PowerButtonPressed += DeactivatePowerButton;
    }

    private void SubscribeToScoreEvent()
    {
        var score = level.GetComponent<Score>();
        score.ScoreUpdated += UpdateScoreText;
    }

    private void SubscribeToLivesEvent()
    {
        var lives = level.GetComponent<Lives>();
        lives.LivesUpdated += UpdateLivesText;
    }

    private void SubscribeToRemainingJewels()
    {
        var levelManager = level.GetComponent<JewelManager>();
        levelManager.RemainingJewelsUpdated += UpdateJewelsText;
    }

    private void SubscribeToBirdsDestroyed()
    {
        var birdSpawner = level.GetComponent<BirdSpawner>();
        birdSpawner.BirdHit += OnBirdHit;
    }

    private void OnDisable()
    {
        UnsubcribeToEvents();
    }

    private void UnsubcribeToEvents()
    {
        if (FindObjectOfType<LevelManager>() == null) return;

        UnsubscribeToPauseAndUnpauseEvents();
        UnubscribeToInputEvents();
        UnsubcribeToScoreEvent();
        UnsubscribeToLivesEvent();
        UnsubscribeToRemainingJewels();
        UnubscribeToBirdsDestroyed();
    }

    private void UnsubscribeToPauseAndUnpauseEvents()
    {
        var pause = level.GetComponent<Pause>();
        pause.GamePaused -= ActivatePauseCanvas;
    }
    private void UnubscribeToInputEvents()
    {
        var playerInput = level.GetComponent<PlayerInput>();
        playerInput.ChangedSelection -= UpdateSelection;
        playerInput.PowerButtonPressed -= DeactivatePowerButton;
    }

    private void UnsubcribeToScoreEvent()
    {
        var score = level.GetComponent<Score>();
        score.ScoreUpdated -= UpdateScoreText;
    }

    private void UnsubscribeToLivesEvent()
    {
        var lives = level.GetComponent<Lives>();
        lives.LivesUpdated -= UpdateLivesText;
    }

    private void UnsubscribeToRemainingJewels()
    {
        var levelManager = level.GetComponent<JewelManager>();
        levelManager.RemainingJewelsUpdated -= UpdateJewelsText;
    }

    private void UnubscribeToBirdsDestroyed()
    {
        var birdSpawner = level.GetComponent<BirdSpawner>();
        birdSpawner.BirdHit -= OnBirdHit;
    }

    private void Start()
    {
        GenerateJewelsDictionary();
        CheckUnusedColors();
        UpdateSelection(ColorType.red);
        StartCoroutine(Greating());
    }

    private IEnumerator Greating()
    {
        var levelManager = level.GetComponent<LevelManager>();
        SetLevelSignText(levelManager);

        greatingText = greatigCanvas.GetComponentInChildren<TMP_Text>();
        var parentGameObject = greatingText.gameObject.transform.parent.gameObject;

        SoundManager.Instance.PlayReadyClip();
        StartCoroutine(BumpAnimation(parentGameObject, greatingsDuration * 0.75f));

        yield return new WaitForSeconds(greatingsDuration * 0.75f);

        greatingText.SetText("Go!");
        SoundManager.Instance.PlayGoClip();
        StartCoroutine(BumpAnimation(parentGameObject, greatingsDuration * 0.25f));

        yield return new WaitForSeconds(greatingsDuration * 0.25f);

        greatingText.gameObject.SetActive(false);
        levelManager.StartLevel();

        yield return new WaitForSeconds(1);

        Destroy(greatigCanvas);
    }

    private void SetLevelSignText(LevelManager levelManager)
    {
        if (endlessMode)
        {
            levelText.enabled = false;
            levelLabel.text = " Endless";
        }
        else
        {
            levelText.text = "Level";
            var levelNumber = levelManager.GetCurrentLevel();
            levelText.text = levelNumber.ToString();
        }
    }

    private void ActivatePauseCanvas(bool isPaused)
    {
        if (isPaused)
        {
            pauseCanvas.SetActive(isPaused);
        }
    }

    private void GenerateJewelsDictionary()
    {
        jewelColorsDictionary.Clear();

        var jewelTemplates = jewelTemplatesList.GetJewelTemplates();
        var playableJewels = jewelManager.PlayableJewels;

        for (int i = 1; i <= playableJewels; i++)
        {
            jewelColorsDictionary.Add(jewelTemplates[i].jewelType, jewelTemplates[i].color);
        }
    }

    private void CheckUnusedColors()
    {
        DisableUnusedSelectorButton(ColorType.white, whiteSelectorButton);
        DisableUnusedSelectorButton(ColorType.purple,purpleSelectorButton);
        DisableUnusedSelectorButton(ColorType.pink, pinkSelectorButton);
        DisableUnusedSelectorButton(ColorType.orange, orangeSelectorButton);
    }

    private void DisableUnusedSelectorButton(ColorType color, GameObject button)
    {
        if (!jewelColorsDictionary.ContainsKey(color))
        {
            button.SetActive(false);
        }
    }

    public void UpdateSelection(ColorType type)
    {
        selectedColorImage.color = jewelColorsDictionary[type];
    }

    private void OnBirdHit(ColorType type)
    {
        if (type != ColorType.orange) return;
        HandleOrangeBirdHit();
    }

    private void HandleOrangeBirdHit()
    {
        if (powerButton.enabled) return;
        powerButton.enabled = true;
        powerButtonGameObject.SetActive(true);
    }

    private void DeactivatePowerButton(ColorType type)
    {
        powerButton.enabled = false;
        powerButtonGameObject.SetActive(false);
    }

    private void UpdateText(TMP_Text text, int newValue)
    {
        text.text = newValue.ToString();
    }

    public void UpdateJewelsText(int newValue)
    {
        UpdateText(jewelsText, newValue);
    }

    public void UpdateLivesText(int newValue)
    {
        UpdateText(livesText, newValue);
        StartCoroutine(BumpAnimation(livesText.gameObject, 0.1f));
    }

    public void UpdateScoreText(int newValue)
    {
        UpdateText(scoreText, newValue);
        Invoke(nameof(StartBumpAnimation),0.3f);
    }

    private void StartBumpAnimation()
    {
        StartCoroutine(BumpAnimation(scoreText.gameObject, 0.1f));
    }

    public IEnumerator BumpAnimation(GameObject text, float animDuration)
    {
        float elapsedTime = 0f;
        var localScale = new Vector3(1, 1, 1);

        while (elapsedTime < animDuration)
        {
            elapsedTime += Time.deltaTime;
            text.transform.localScale = Vector3.Lerp(localScale, localScale * 1.15f, elapsedTime / animDuration);
            yield return null;
        }
        
        text.transform.localScale = localScale;
    }

    public void GoToMainMenu()
    {
        level.GetComponent<LevelManager>().EndLevel(true);
    }

    public void SetToEndlessMode()
    {
        endlessMode = true;
        UpdateJewelsText(0);
        jewelsLabel.text = "Jewels:";

        var jewelManager = level.GetComponent<JewelManager>();
        jewelManager.RemainingJewelsUpdated -= UpdateJewelsText;
        jewelManager.JewelHit += OnJewelDestroyed;
    }

    private void OnJewelDestroyed(ColorType type, int mult)
    {
        jewelsDestroyed++;
        UpdateJewelsText(jewelsDestroyed);
    }
}