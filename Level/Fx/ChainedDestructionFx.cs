using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ChainedDestructionFx : MonoBehaviour
{
    ObjectsPool objectsPool;
    List<Transform> targets;
    LevelManager levelManager;
    Pause pause;
    [SerializeField] float speed = 2f;
    int currentTarget = 0;
    bool active = false;
    bool isPaused = false;

    [SerializeField] AudioClip startClip;
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioClip endClip;
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = startClip;
    }

    private void ResetPosition(float distance)
    {
        var position = transform.position;
        transform.position = new Vector3(position.x, position.y - distance, position.z);
    }

    public void Setup(LevelManager levelManager)
    {
        GetTargets(levelManager);

        this.levelManager = levelManager;
        pause = levelManager.Pause;

        pause.GamePaused += OnPause;
        levelManager.ResetingPositions += ResetPosition;

        active = true;
        audioSource.Play();
    }

    private void GetTargets(LevelManager levelManager)
    {
        var jewelManager = levelManager.JewelManager;
        targets = jewelManager.ReturnAllJewelTranformsInGameArea().ToList();
        targets.OrderBy(transform => transform.position.y);
        if (targets.Count == 0) Destroy(gameObject);
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

        if (targets.Count == 0 || targets[currentTarget] == null) StopTrail();

        var targetPosition = targets[currentTarget].position;
        transform.LookAt(targetPosition);

        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (Vector3.Distance(targetPosition, transform.position) < 0.8f)
        {
            targets[currentTarget].GetComponent<Jewel>().HandleCorrectHit();
            SoundManager.Instance.PlayClip(hitClip);

            if (currentTarget == targets.Count - 1)
            {
                StopTrail();
            }
            else
            {
                currentTarget++;
            }
        }
    }

    private void StopTrail()
    {
        active = false;
        Invoke(nameof(Destroy), 0.25f);
        Invoke(nameof(StopTrailSound), 0.25f);
        SoundManager.Instance.PlayClip(endClip);
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

    public void Recycle()
    {
        objectsPool.Recycle(gameObject);
    }
}
