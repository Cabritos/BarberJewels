using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ChainedDestructionFx : MonoBehaviour
{
    LevelManager levelManager;
    JewelManager jewelManager;
    ObjectsPool objectsPool;
    Pause pause;

    Transform target;
    Vector3 targetPosition;

    [SerializeField] float speed = 2f;
    bool active = false;
    bool isPaused = false;

    AudioSource audioSource;
    [SerializeField] AudioClip startClip;
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioClip endClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = startClip;
    }

    public void Setup(LevelManager levelManager)
    {
        jewelManager = levelManager.JewelManager;
        if (!GetNewTarget()) Destroy(gameObject);

        this.levelManager = levelManager;
        pause = levelManager.Pause;

        pause.GamePaused += OnPause;
        levelManager.ResetingPositions += ResetPosition;

        active = true;
        audioSource.Play();
    }

    private bool GetNewTarget()
    {
        target = jewelManager.GetBottomJewelInPlayArea();

        return target != null;
    }

    private void ResetPosition(float distance)
    {
        var position = transform.position;
        transform.position = new Vector3(position.x, position.y - distance, position.z);
    }

    private void OnPause(bool isPaused)
    {
        this.isPaused = isPaused;
        if (audioSource == null) return;

        if (isPaused)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();
        }
    }

    private void Update()
    {
        if (!active || isPaused) return;

        GetTargetOrStop();

        targetPosition = target.position;
        MoveToTarget();

        if (Vector3.Distance(targetPosition, transform.position) < 0.8f)
        {
            target.GetComponent<Jewel>().HandleCorrectHit();
            SoundManager.Instance.PlayClip(hitClip);

            GetTargetOrStop();
        }
    }

    private void GetTargetOrStop()
    {
        if (!GetNewTarget())
        {
            StopTrail();
        }
    }

    private void StopTrail()
    {
        active = false;
        Invoke(nameof(StopTrailSound), 0.25f);
        Invoke(nameof(Destroy), 0.25f);
        SoundManager.Instance.PlayClip(endClip);
    }

    private void MoveToTarget()
    {
        transform.LookAt(targetPosition);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void StopTrailSound()
    {
        audioSource.Stop();
    }

    private void Destroy()
    {
        if (pause != null) pause.GamePaused -= OnPause;
        if (levelManager != null) levelManager.ResetingPositions -= ResetPosition;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}