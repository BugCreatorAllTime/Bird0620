using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : BaseController
{
    private static AudioController _instance;

    public static AudioController Instance
    {
        get { return _instance; }
    }

    private AudioSource _audioSource;

    public AudioSource AudioSource
    {
        get { return _audioSource ? _audioSource : (_audioSource = GetComponent<AudioSource>()); }
    }

    [HideInInspector] public Dictionary<string, AudioClip> AudioClips = new Dictionary<string, AudioClip>();
    [HideInInspector] public List<string> SoundEffectLoop = new List<string>();

    private bool _isPlaying;

    private bool _haveOtherSoundPlay;

    private float _timeWaitClickButton = 0.07f;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        _instance = this;
    }

    public override void Start()
    {
        base.Start();
        DataManager.Instance.OnSettingChangeCallback.Add(OnSettingChangeCallback);
    }

    public void OnSettingChangeCallback()
    {
        if (AudioSource.clip != null)
        {
            if (_isPlaying && !DataManager.Instance.SettingData.Music) AudioSource.Stop();
            if (!_isPlaying && DataManager.Instance.SettingData.Music) AudioSource.Play();
            _isPlaying = DataManager.Instance.SettingData.Music;
        }
    }

    public void PlayLoop(string audioName)
    {
        if (!CheckClipExist(audioName)) return;
        AudioSource.clip = AudioClips[audioName];
        AudioSource.loop = true;
        if (!DataManager.Instance.SettingData.Music) return;
        _isPlaying = true;
        AudioSource.Play();
    }
    
    public void PlayLoop(AudioClip audioClip)
    {
        AudioSource.clip = audioClip;
        AudioSource.loop = true;
        if (!DataManager.Instance.SettingData.Music) return;
        _isPlaying = true;
        AudioSource.Play();
    }

    public void PlayLoopSoundEffect(string audioName, float volume = 1f)
    {
        if (!CheckClipExist(audioName)) return;
        if (!DataManager.Instance.SettingData.Sound) return;

        SoundEffectLoop.Add(audioName);
        StartCoroutine(_PlayLoopSoundEffect(audioName, volume));
    }

    public void StopLoopSoundEffect(string audioName)
    {
        SoundEffectLoop.Remove(audioName);
    }

    IEnumerator _PlayLoopSoundEffect(string audioName, float volume)
    {
        if (!SoundEffectLoop.Contains(audioName)) yield break;
        var clip = AudioClips[audioName];
        AudioSource.PlayOneShot(clip, volume);
        yield return new WaitForSeconds(clip.length);
        StartCoroutine(_PlayLoopSoundEffect(audioName, volume));
    }

    private IEnumerator _corAfterOtherSoundPlay;
    private IEnumerator _corPlayButtonClick;

    public void PlayOneShot(string audioName, float volume = 1f)
    {
        if (!CheckClipExist(audioName)) return;
        if (!DataManager.Instance.SettingData.Sound) return;

        if (audioName == DATA_RESOURCES.AUDIO.CLICK)
        {
            if (!_haveOtherSoundPlay)
            {
//				StopAllCoroutines();
                if (_corPlayButtonClick != null) StopCoroutine(_corPlayButtonClick);
                _corPlayButtonClick = PlayButtonClick();
                StartCoroutine(_corPlayButtonClick);
            }
        }
        else
        {
            _haveOtherSoundPlay = true;
            AudioSource.PlayOneShot(AudioClips[audioName], volume);

//			StopAllCoroutines();
            if (_corAfterOtherSoundPlay != null) StopCoroutine(_corAfterOtherSoundPlay);
            _corAfterOtherSoundPlay = AfterOtherSoundPlay();
            StartCoroutine(_corAfterOtherSoundPlay);
        }
    }

    public void PlayOneShot(AudioClip clip, float volume = 1)
    {
        if (!DataManager.Instance.SettingData.Sound) return;
        _haveOtherSoundPlay = true;


        AudioSource.PlayOneShot(clip, volume);

        if (_corAfterOtherSoundPlay != null) StopCoroutine(_corAfterOtherSoundPlay);
        _corAfterOtherSoundPlay = AfterOtherSoundPlay();
        StartCoroutine(_corAfterOtherSoundPlay);
    }

    IEnumerator PlayButtonClick()
    {
        _haveOtherSoundPlay = false;
        yield return new WaitForSeconds(_timeWaitClickButton);
        if (!_haveOtherSoundPlay) AudioSource.PlayOneShot(AudioClips[DATA_RESOURCES.AUDIO.CLICK]);
    }

    IEnumerator AfterOtherSoundPlay()
    {
        yield return new WaitForSeconds(_timeWaitClickButton);
        _haveOtherSoundPlay = false;
    }

    public bool CheckClipExist(string audioName)
    {
        if (AudioClips.ContainsKey(audioName)) return true;
        var clip = Resources.Load<AudioClip>(audioName);
        if (clip == null) return false;
        AudioClips[audioName] = clip;
        return true;
    }
}