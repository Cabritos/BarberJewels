using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))] 
public class Logo : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] AudioClip mainClip;
    [SerializeField] AudioClip goatClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMainClip()
    {
        audioSource.PlayOneShot(mainClip);
    }

    public void PlayGoatClip()
    {
        audioSource.PlayOneShot(goatClip);
    }

    public void GoToNextScene()
    {
        SceneManager.LoadScene(1);
    }
}