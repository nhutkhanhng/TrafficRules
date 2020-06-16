using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


// Custom yield instruction to wait until a callback is called
// by Jackson Dunstan, http://JacksonDunstan.com/articles/3678
// License: MIT
public class WaitForCallback<TResult> : CustomYieldInstruction
{
    /// <summary>
    /// If the callback has been called
    /// </summary>
    private bool done;

    /// <summary>
    /// Immediately calls the given Action and passes it another Action. Call that Action with the
    /// result of the callback function. Doing so will cause <see cref="keepWaiting"/> to be set to
    /// false and <see cref="Result"/> to be set to the value passed to the Action.
    /// </summary>
    /// <param name="callCallback">Action that calls the callback function. Pass the result to
    /// the Action passed to it to stop waiting.</param>
    public WaitForCallback(Action<Action<TResult>> callCallback)
    {
        callCallback(r => { Result = r; done = true; });
    }

    /// <summary>
    /// If the callback is still ongoing. This is set to false when the Action passed to the
    /// Action passed to the constructor is called.
    /// </summary>
    override public bool keepWaiting { get { return done == false; } }

    /// <summary>
    /// Result of the callback
    /// </summary>
    public TResult Result { get; private set; }
}

public class AudioCassette : MonoBehaviour
{
    [Header("Music Settings")]
    public AudioClip musicAudioClip;
    public AudioMixerGroup musicOutput;
    public bool musicPlayOnAwake = true;
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Header("Ambient Settings")]
    public AudioClip ambientAudioClip;
    public AudioMixerGroup ambientOutput;
    public bool ambientPlayOnAwake = true;
    [Range(0f, 1f)]
    public float ambientVolume = 1f;

    protected AudioSource m_MusicAudioSource;
    protected AudioSource m_AmbientAudioSource;

    protected bool m_TransferMusicTime, m_TransferAmbientTime;
    protected AudioCassette m_OldInstanceToDestroy = null;
    public static AudioCassette Instance
    {
        get
        {
            if (s_Instance != null)
                return s_Instance;

            s_Instance = FindObjectOfType<AudioCassette>();

            return s_Instance;
        }
    }

    protected static AudioCassette s_Instance;
    // Tải nhạc về lưu vào máy =]]/
    protected Queue<AudioClip> m_MusicQueue = new Queue<AudioClip>();

    protected IEnumerator MusicCoroutine;

    void Awake()
    {

        m_MusicAudioSource = gameObject.AddComponent<AudioSource>();
        m_MusicAudioSource.clip = musicAudioClip;
        m_MusicAudioSource.outputAudioMixerGroup = musicOutput;
        m_MusicAudioSource.loop = true;
        m_MusicAudioSource.volume = musicVolume;

        if (musicPlayOnAwake)
        {
            m_MusicAudioSource.time = 0f;
            m_MusicAudioSource.Play();
        }

        m_AmbientAudioSource = gameObject.AddComponent<AudioSource>();
        m_AmbientAudioSource.clip = ambientAudioClip;
        m_AmbientAudioSource.outputAudioMixerGroup = ambientOutput;
        m_AmbientAudioSource.loop = true;
        m_AmbientAudioSource.volume = ambientVolume;

        if (ambientPlayOnAwake)
        {
            m_AmbientAudioSource.time = 0f;
            m_AmbientAudioSource.Play();
        }
    }

    private void ChangeMusicWhenChangeScene()
    {
        // Todo somehting else
    }


    private void Start()
    {
        //if delete & trasnfer time only in Start so we avoid the small gap that doing everything at the same time in Awake would create 
        if (m_OldInstanceToDestroy != null)
        {
            if (m_TransferAmbientTime) m_AmbientAudioSource.timeSamples = m_OldInstanceToDestroy.m_AmbientAudioSource.timeSamples;
            if (m_TransferMusicTime) m_MusicAudioSource.timeSamples = m_OldInstanceToDestroy.m_MusicAudioSource.timeSamples;
            m_OldInstanceToDestroy.Stop();
            Destroy(m_OldInstanceToDestroy.gameObject);
        }
    }

    private void Update()
    {
        if (m_MusicQueue.Count > 0)
        {
            //isPlaying will be false once the current clip end up playing 
            if (!m_MusicAudioSource.isPlaying)
            {
                m_MusicQueue.Dequeue();
                if (m_MusicQueue.Count > 0)
                {
                    m_MusicAudioSource.clip = m_MusicQueue.Peek();
                    m_MusicAudioSource.Play();
                }
                else
                {//Back to looping music clip
                    m_MusicAudioSource.clip = musicAudioClip;
                    m_MusicAudioSource.loop = true;
                    m_MusicAudioSource.Play();
                }
            }
        }
    }

    [ContextMenu("Test")]
    public void Test()
    {
        ChangeMusicFade(KAudio.AudioManager.Instance["Music_Battle"], 2f);
    }


    protected class AudioQueue : IEnumerator
    {
        public AudioClip clips;

        public object Current => throw new NotImplementedException();

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    protected IEnumerator current;
    protected Queue<IEnumerator> queueMusic;
    private void CheckQueue()
    {
        if(queueMusic != null)
        {
            if (queueMusic.Count > 0)
            {
                if (current == null)
                {
                    current = queueMusic.Dequeue();
                    StartCoroutine(current);
                }
            }
        }
    }
    public void ChangeMusicFade(AudioClip clip, float fadeTime)
    {
        if(this.Available() == false)
        {
            StopAllCoroutines();
            return;
        }

        if (queueMusic == null)
            queueMusic = new Queue<IEnumerator>();

        if (clip == null)
        {
            return;
        }

        if (this.m_MusicAudioSource.gameObject == null)
        {
            return;
        }

        queueMusic.Enqueue(VolumeFade(this.m_MusicAudioSource, 0f, fadeTime, () =>
        {
            ChangeMusic(clip);
            PlayJustMusic();
            StartCoroutine(VolumeFade(this.m_MusicAudioSource, 1f, fadeTime,() =>
            {
                current = null;
                CheckQueue();
            }));
        }));

        if (current == null)
        {
            current = queueMusic.Dequeue();
            StartCoroutine(current);
        }
    }

    public void PushClip(AudioClip clip)
    {
        if (clip == null)
            return;

        m_MusicQueue.Enqueue(clip);
        m_MusicAudioSource.Stop();
        m_MusicAudioSource.loop = false;
        m_MusicAudioSource.time = 0;
        m_MusicAudioSource.clip = clip;
        m_MusicAudioSource.Play();
    }

    public void ChangeMusic(AudioClip clip)
    {
        if (clip == null)
            return;

        musicAudioClip = clip;
        m_MusicAudioSource.clip = clip;
    }

    public void ChangeAmbient(AudioClip clip)
    {
        ambientAudioClip = clip;
        m_AmbientAudioSource.clip = clip;
    }


    public void Play()
    {
        PlayJustAmbient();
        PlayJustMusic();
    }

    public void PlayJustMusic()
    {
        m_MusicAudioSource.time = 0f;
        m_MusicAudioSource.Play();
    }

    public void PlayJustAmbient()
    {
        m_AmbientAudioSource.Play();
    }

    public void Stop()
    {
        StopJustAmbient();
        StopJustMusic();
    }

    public void StopJustMusic()
    {
        m_MusicAudioSource.Stop();
    }

    public void StopJustAmbient()
    {
        m_AmbientAudioSource.Stop();
    }

    public void Mute()
    {
        MuteJustAmbient();
        MuteJustMusic();
    }

    public void MuteJustMusic()
    {
        m_MusicAudioSource.volume = 0f;
    }

    public void MuteJustAmbient()
    {
        m_AmbientAudioSource.volume = 0f;
    }

    public void Unmute()
    {
        UnmuteJustAmbient();
        UnmuteJustMustic();
    }

    public void UnmuteJustMustic()
    {
        m_MusicAudioSource.volume = musicVolume;
    }

    public void UnmuteJustAmbient()
    {
        m_AmbientAudioSource.volume = ambientVolume;
    }

    public void Mute(float fadeTime, System.Action callback = null)
    {
        MuteJustAmbient(fadeTime);
        MuteJustMusic(fadeTime, callback);
    }

    public void MuteJustMusic(float fadeTime, System.Action callback = null)
    {
        StartCoroutine(VolumeFade(m_MusicAudioSource, 0f, fadeTime, callback:callback));
    }

    public void MuteJustAmbient(float fadeTime, System.Action callback = null)
    {
        StartCoroutine(VolumeFade(m_AmbientAudioSource, 0f, fadeTime, callback));
    }

    public void Unmute(float fadeTime, System.Action callback = null)
    {
        UnmuteJustAmbient(fadeTime);
        UnmuteJustMusic(fadeTime, callback);
    }

    public void UnmuteJustMusic(float fadeTime, System.Action callback = null)
    {
        StartCoroutine(VolumeFade(m_MusicAudioSource, musicVolume, fadeTime, callback));
    }

    public void UnmuteJustAmbient(float fadeTime)
    {
        StartCoroutine(VolumeFade(m_AmbientAudioSource, ambientVolume, fadeTime));
    }

    protected IEnumerator VolumeFade(AudioSource source, float finalVolume, float fadeTime, System.Action callback = null)
    {
        float volumeDifference = Mathf.Abs(source.volume - finalVolume);
        float inverseFadeTime = 1f / fadeTime;

        while (!Mathf.Approximately(source.volume, finalVolume))
        {
            float delta = Time.deltaTime * volumeDifference * inverseFadeTime;
            source.volume = Mathf.MoveTowards(source.volume, finalVolume, delta);
            yield return null;
        }

        source.volume = finalVolume;
        callback?.Invoke();
    }
}