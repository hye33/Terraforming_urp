using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    private float _bgm = 1.0f;
    public float BGM { get { return _bgm; } set {  _bgm = value; } }
    private float _effect = 1.0f;
    public float Effect { get { return _effect; } set { _effect = value; } }

    // 추가된 audioSources
    private float _subBgm = 1.0f;
    public float SubBGM { get { return _subBgm; } set { _subBgm = value; } }
    private float _loopEffect = 1.0f;
    public float LoopEffect { get { return _loopEffect; } set { _loopEffect = value; } }

    private float _sound = 1.0f;
    public float Sound { get { return _sound; } set { _sound = value; } }
    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
            _audioSources[(int)Define.Sound.SubBgm].loop = true;
            _audioSources[(int)Define.Sound.LoopEffect].loop = true;
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    // MaxCount 개수만큼 audioSources 생성
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];

    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
        {
            Debug.Log("Audio Clip is null");
            return;
        }

        if (type != Define.Sound.Effect)
        {
            AudioSource audioSource = _audioSources[(int)type];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void Stop(Define.Sound type)
    {
        AudioSource audioSource = _audioSources[(int)type];
        audioSource.Stop();
    }

    public void SetVolume() //볼륨 조절 (임시)
    {
        AudioSource audioSourceBgm = _audioSources[(int)Define.Sound.Bgm];
        AudioSource audioSourceEffect = _audioSources[(int)Define.Sound.Effect];

        AudioSource audioSourceSubBgm = _audioSources[(int)Define.Sound.SubBgm];
        AudioSource audioSourceLoopEffect = _audioSources[(int)Define.Sound.LoopEffect];

        audioSourceBgm.volume = _bgm * _sound;
        audioSourceEffect.volume = _effect * _sound;

        audioSourceSubBgm.volume = _subBgm * _sound;
        audioSourceLoopEffect.volume = _loopEffect * _sound;
    }
 }
