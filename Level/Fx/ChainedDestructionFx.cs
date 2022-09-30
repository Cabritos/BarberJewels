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

    public void Setup(IEnumerable<Transform> positions, LevelManager levelManager)
    {
        targets = positions.ToList();
        if (targets.Count == 0) Destroy(gameObject);
        
        pause.OnPauseGame += OnPause;
        levelManager.OnResetPositions += ResetPosition;

        this.levelManager = levelManager;
        pause = levelManager.Pause;

        active = true;
        audioSource.Play();
    }

    private void Update()
    {
        if (!active || isPaused) return;

        var targetPosition = targets[currentTarget].position;
        transform.LookAt(targetPosition);

        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (Vector3.Distance(targetPosition, transform.position) < 0.8f)
        {
            targets[currentTarget].GetComponent<Jewel>().HandleCorrectHit();
            SoundManager.Instance.PlayClip(hitClip);

            if (currentTarget == targets.Count - 1)
            {
                active = false;
                Invoke(nameof(Destroy), 0.25f);
                Invoke(nameof(StopTrailSound), 0.25f);
                SoundManager.Instance.PlayClip(endClip);
            }
            else
            {
                currentTarget++;
            }
        }
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

    private void StopTrailSound()
    {
        audioSource.Stop();
    }

    private void Destroy()
    {
        if (pause != null) pause.OnPauseGame -= OnPause;
        if (levelManager != null) levelManager.OnResetPositions -= ResetPosition;
        Destroy(gameObject);
    }

    public void Recycle()
    {
        objectsPool.Recycle(gameObject);
    }
}
