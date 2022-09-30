using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSlidersController : MonoBehaviour
{
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider musicSlider;

    float soundVolume;
    float musicVolume;

    private void Awake()
    {
        SetVolumeSlidersValues();
    }

    private void SetVolumeSlidersValues()
    {
        soundVolume = SoundManager.Instance.SfxVolume;
        UpdateSoundUI(soundVolume);

        musicVolume = SoundManager.Instance.MusicVolume;
        UpdateMusicUI(musicVolume);
    }

    private void Start()
    {
        soundSlider.onValueChanged.AddListener(SoundVolumeChange);
        musicSlider.onValueChanged.AddListener(MusicVolumeChange);
    }

    private void OnDestroy()
    {
        soundSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
    }

    private void SoundVolumeChange(float value)
    {
        SoundManager.Instance.SetSfxVolume(value);
    }

    private void MusicVolumeChange(float value)
    {
        SoundManager.Instance.SetMusicVolume(value);
    }

    private void UpdateSoundUI(float value)
    {
        UpadteUI(value, soundSlider);
    }

    private void UpdateMusicUI(float value)
    {
        UpadteUI(value, musicSlider);
    }

    private void UpadteUI(float value, Slider slider)
    {
        slider.value = value;
    }
}
