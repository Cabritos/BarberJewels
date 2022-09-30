using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : SingletonMonobehaviour<SoundManager>
{
    public float SfxVolume { get; private set; }
    public float MusicVolume { get; private set; }

    [SerializeField] AudioSource sfxAudioSource;
    [SerializeField] AudioSource musicAudioSource;

    [SerializeField] AudioClip menuMusicIntro;
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip levelMusic;
    bool menuMusicIsPlaying = false;

    [SerializeField][Range(0, 1)] float jewelDestroyedVolume = 1f;
    [SerializeField] AudioClip jewelDestroyedClip;

    [SerializeField] AudioClip jewelScore1Clip;
    [SerializeField] AudioClip jewelScore2Clip;
    [SerializeField] AudioClip jewelScore3Clip;
    [SerializeField] AudioClip jewelScore4Clip;
    [SerializeField] AudioClip jewelScore5Clip;

    [SerializeField][Range(0, 1)] float jewelLostVolume = 1f;
    [SerializeField] AudioClip jewelLostClip;

    [SerializeField][Range(0, 1)] float birdSpawnedVolume = 1f;
    [SerializeField] AudioClip birdSpawnedClip;
    [SerializeField][Range(0, 1)] float birdDestroyedVolume = 1f;
    [SerializeField] AudioClip birdDestroyedClip;

    [SerializeField] [Range(0,1)] float redBirdVolume = 1f;
    [SerializeField] AudioClip redBirdClip;
    [SerializeField][Range(0, 1)] float blueBirdVolume = 1f;
    [SerializeField] AudioClip blueBirdClip;
    [SerializeField][Range(0, 1)] float greenBirdVolume = 1f;
    [SerializeField] AudioClip greenBirdClip;
    [SerializeField][Range(0, 1)] float yellowBirdVolume = 1f;
    [SerializeField] AudioClip yellowBirdClip;
    [SerializeField][Range(0, 1)] float whiteBirdVolume = 1f;
    [SerializeField] AudioClip whiteBirdClip;
    [SerializeField][Range(0, 1)] float purpleBirdVolume = 1f;
    [SerializeField] AudioClip purpleBirdClip;
    [SerializeField][Range(0, 1)] float pinkBirdVolume = 1f;
    [SerializeField] AudioClip pinkBirdClip;
    [SerializeField][Range(0, 1)] float orangeBirdVolume = 1f;
    [SerializeField] AudioClip orageBirdClip;
    [SerializeField][Range(0, 1)] float blackBirdVolume = 1f;
    [SerializeField] AudioClip blackBirdClip;

    [SerializeField][Range(0, 1)] float birdRandomRewardVolume = 1;
    [SerializeField] AudioClip birdRandomRewardClip;
    [SerializeField][Range(0, 1)] float yellowPowerRewardVolume = 1f;
    [SerializeField] AudioClip yellowPowerRewardClip;

    [SerializeField][Range(0, 1)] float orangePowerVolume = 1f;
    [SerializeField] AudioClip orangePowerClip;

    [SerializeField][Range(0, 1)] float readyVolume = 1f;
    [SerializeField] AudioClip readyClip;
    [SerializeField][Range(0, 1)] float goVolume = 1f;
    [SerializeField] AudioClip goClip;

    [SerializeField][Range(0, 1)] float menuSelectionVolume = 1f;
    [SerializeField] AudioClip menuSelectionClip;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        GetSavedVolumes();
    }

    private void GetSavedVolumes()
    {
        SfxVolume = PlayerPrefs.GetFloat(nameof(SfxVolume), 1);
        SetSfxAudioMixerVolume(SfxVolume);

        MusicVolume = PlayerPrefs.GetFloat(nameof(MusicVolume), 0.7f);
        SetMusicAudioMixerVolume(MusicVolume);
    }

    public void SetupNewLevel()
    {
        SubscribeToPauseEvent();
        SubscribeToJewelManagerEvent();
    }

    private void SubscribeToPauseEvent()
    {
        var pause = GetComponent<Pause>();
        if (pause == null) return;
        pause.OnPauseGame += PausedGame;
    }

    private void SubscribeToJewelManagerEvent()
    {
        var levelManager = GameManager.Instance.GetCurrentLevelManager();
        if (levelManager == null) return;
        levelManager.JewelManager.OnJewelHit += PlayJewelDestroyedClip;
    }

    public void EndLevel()
    {
        UnsubscribeToPauseEvent();
        UnubscribeToJewelManagerEvent();
    }

    private void UnsubscribeToPauseEvent()
    {
        var pause = GetComponent<Pause>();
        if (pause == null) return;
        pause.OnPauseGame -= PausedGame;
    }

    private void UnubscribeToJewelManagerEvent()
    {
        var levelManager = GameManager.Instance.GetCurrentLevelManager();
        if (levelManager == null) return;
        levelManager.JewelManager.OnJewelHit -= PlayJewelDestroyedClip;
    }

    public void SetSfxVolume(float volume)
    {
        if (volume > 1) volume = 1;
        if (volume < 0) volume = 0;

        SfxVolume = volume;
        SetSfxAudioMixerVolume(SfxVolume);
        PlayerPrefs.SetFloat(nameof(SfxVolume), SfxVolume);
    }

    private void SetSfxAudioMixerVolume(float volume)
    {
        sfxAudioSource.outputAudioMixerGroup.audioMixer.SetFloat("SFX volume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        if (volume > 1) volume = 1;
        if (volume < 0) volume = 0;

        MusicVolume = volume;
        SetMusicAudioMixerVolume(MusicVolume);
        PlayerPrefs.SetFloat(nameof(MusicVolume), MusicVolume);
    }

    private void SetMusicAudioMixerVolume(float volume)
    {
        musicAudioSource.outputAudioMixerGroup.audioMixer.SetFloat("Music volume", Mathf.Log10(volume) * 20);
    }

    private void PausedGame(bool isPaused)
    {
        
    }
    
    public void PlayClip(AudioClip clip)
    {
        PlayClip(clip, 1f);
    }

    public void PlayClip(AudioClip clip, float volume)
    {
        sfxAudioSource.PlayOneShot(clip, volume);
    }

    public void PlayJewelDestroyedClip(ColorType type, int combo)
    {
        var clipToPlay = combo % 5;

        if (clipToPlay == 1) PlayClip(jewelScore1Clip, jewelDestroyedVolume);
        else if (clipToPlay == 2) PlayClip(jewelScore2Clip, jewelDestroyedVolume);
        else if (clipToPlay == 3) PlayClip(jewelScore3Clip, jewelDestroyedVolume);
        else if (clipToPlay == 4) PlayClip(jewelScore4Clip, jewelDestroyedVolume);
        else if (clipToPlay == 0) PlayClip(jewelScore5Clip, jewelDestroyedVolume);
    }

    public void PlayJewelLostClip()
    {
        PlayClip(jewelLostClip, jewelLostVolume);
    }

    public void PlayBirdDestroyedClip()
    {
        PlayClip(birdDestroyedClip, birdDestroyedVolume);
    }

    public void PlayBirdSpawned()
    {
        PlayClip(birdSpawnedClip, birdSpawnedVolume);
    }

    public void PlayRedBirdClip()
    {
        PlayClip(redBirdClip, redBirdVolume);
    }

    public void PlayBlueBirdClip()
    {
        PlayClip(blueBirdClip, blueBirdVolume);
    }
    public void PlayGreenBirdClip()
    {
        PlayClip(greenBirdClip, greenBirdVolume);
    }
    public void PlayYellowBirdClip()
    {
        PlayClip(yellowBirdClip, yellowBirdVolume);
    }
    public void PlayWhiteBirdClip()
    {
        PlayClip(whiteBirdClip, whiteBirdVolume);
    }
    public void PlayPurpleBirdClip()
    {
        PlayClip(purpleBirdClip, purpleBirdVolume);
    }
    public void PlayPinkBirdClip()
    {
        PlayClip(pinkBirdClip, pinkBirdVolume);
    }
    public void PlayOrangeBirdClip()
    {
        PlayClip(orageBirdClip, orangeBirdVolume);
    }
    public void PlayBlackBirdClip()
    {
        PlayClip(jewelScore5Clip, blackBirdVolume);
    }

    public void PlayYellowPowerWonClip()
    {
        PlayClip(yellowPowerRewardClip, yellowPowerRewardVolume);
    }

    public void PlayBirdRandomRewardClip()
    {
        PlayClip(birdRandomRewardClip, birdRandomRewardVolume);
    }

    public void PlayOrangePowerClip()
    {
        PlayClip(orangePowerClip, orangePowerVolume);
    }

    public void PlayReadyClip()
    {
        PlayClip(readyClip, readyVolume);
    }

    public void PlayGoClip()
    {
        PlayClip(goClip, goVolume);
    }

    public void PlayMenuSelectionClip()
    {
        PlayClip(menuSelectionClip, menuSelectionVolume);
    }

    public void PlayLevelMusic()
    {
        menuMusicIsPlaying = false;
        ReplaceMusic(levelMusic);
    }

    public void PlayMenuMusic()
    {
        if (menuMusicIsPlaying) return;

        menuMusicIsPlaying = true;
        musicAudioSource.Stop();
        musicAudioSource.PlayOneShot(menuMusicIntro);
        musicAudioSource.clip = menuMusic;
        musicAudioSource.PlayDelayed(4f);
    }

    private void ReplaceMusic(AudioClip audioClip)
    {
        musicAudioSource.Stop();
        musicAudioSource.clip = audioClip;
        musicAudioSource.PlayDelayed(0.2f);
    }
}