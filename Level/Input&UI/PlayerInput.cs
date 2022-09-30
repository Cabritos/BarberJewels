using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class PlayerInput : MonoBehaviour
{
    public event Action<ColorType> OnChangeSelection;
    public event Action<ColorType> OnPowerButtonPress;

    [SerializeField] LayerMask raycastLayerMask;
    Camera mainCamera;
    EventSystem eventSystem;
    [SerializeField] LevelUI levelUI;

    [SerializeField] AudioClip changeSelectionClip;

    float rayLenght;
    ColorType selectedColorType = ColorType.red;
    bool gameIsPaused = false;

    private void Awake()
    {
        CacheReferences();
        CalculateRayLength();
    }

    private void CacheReferences()
    {
        mainCamera = Camera.main;
        if (eventSystem == null) eventSystem = FindObjectOfType<EventSystem>();
    }

    private void CalculateRayLength()
    {
        var parentPosition = mainCamera.transform.parent.transform.position;
        var distance = Vector3.Distance(transform.position, parentPosition);
        rayLenght = distance * 1.2f;
    }

    private void OnEnable()
    {
        SubscribeToPauseEvent();
    }

    private void SubscribeToPauseEvent()
    {
        var pause = GetComponent<Pause>();
        pause.OnPauseGame += PausedGame;
    }

    private void OnDisable()
    {
        UnsubscribeToPauseEvent();
    }

    private void UnsubscribeToPauseEvent()
    {
        var pause = GetComponent<Pause>();
        pause.OnPauseGame -= PausedGame;
    }

    private void PausedGame(bool paused)
    {
        gameIsPaused = paused;
    }

    private void Update()
    {
        if (gameIsPaused) return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            PressScreen(Input.mousePosition);
        }
#endif

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                PressScreen(touch.position);
            }
        }
    }

    private void PressScreen(Vector3 position)
    {
        if (eventSystem.IsPointerOverGameObject()) return;

        RaycastToGameArea(position);
    }

    private void RaycastToGameArea(Vector3 position)
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(position);

        Debug.DrawRay(ray.origin, ray.direction * rayLenght, Color.red, 2f);

        if (Physics.Raycast(ray, out hit, 40, raycastLayerMask))
        {
            HitObject(hit.collider.gameObject);
        }
    }

    private void HitObject(GameObject gameObject)
    {
        var hitable = gameObject.GetComponent<IHitable>();

        if (hitable != null) hitable.HandleHit(selectedColorType);
    }

    public void SelectColorType(ColorType type)
    {
        if (gameIsPaused) return;
        if (type == selectedColorType) return;

        selectedColorType = type;
        OnChangeSelection(selectedColorType);
        SoundManager.Instance.PlayClip(changeSelectionClip);
    }

    public void SelectRed()
    {
        SelectColorType(ColorType.red);
    }

    public void SelectBlue()
    {
        SelectColorType(ColorType.blue);
    }

    public void SelectGreen()
    {
        SelectColorType(ColorType.green);
    }

    public void SelectYellow()
    {
        SelectColorType(ColorType.yellow);
    }

    public void SelectWhite()
    {
        SelectColorType(ColorType.white);
    }

    public void SelectPink()
    {
        SelectColorType(ColorType.pink);
    }

    public void SelectOrange()
    {
        SelectColorType(ColorType.orange);
    }

    public void SelectPurple()
    {
        SelectColorType(ColorType.purple);
    }

    public void PowerButtonPress()
    {
        OnPowerButtonPress?.Invoke(selectedColorType);
    }

    private void OnDestroy()
    {
        OnChangeSelection = null;
    }
}
