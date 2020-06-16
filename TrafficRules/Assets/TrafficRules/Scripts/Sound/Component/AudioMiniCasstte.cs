using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMiniCasstte : SoundComponent
{
    protected Queue<AudioClip> m_MusicQueue;
    public AudioMixerGroup MusicMixer;
    public AudioMixerGroup AmbientMixer;
    public Queue<AudioClip> MusicQueue { get { return m_MusicQueue; } }
    public SoundComponent ambientAudio;

    public bool IsInterupImmediate = true;

    public AudioClip AddMusicToQueue(AudioClip Clip)
    {
        if (m_MusicQueue == null)
            m_MusicQueue = new Queue<AudioClip>();

        if (Clip != null)
            this.m_MusicQueue.Enqueue(Clip);

        return Clip;
    }

    public void StartCasstte()
    {
        this.ambientAudio._Sound.Stop();
        this.ambientAudio._Sound.source.outputAudioMixerGroup = this.AmbientMixer;
        this._Sound.source.outputAudioMixerGroup = this.MusicMixer;

        if (this.IsInterupImmediate)
        {
            this._Sound.source.Stop();
        }

        this.ambientAudio._Sound.callback += () =>
        {
            if (m_MusicQueue != null)
            {
                if (m_MusicQueue.Count > 0)
                {
                    this._Sound.clip = m_MusicQueue.Dequeue();
                    if (this._Sound.clip != null)
                        this.Play();
                }
            }
        };

        if (this.ambientAudio._Sound.source.clip == null)
        {
            Debug.LogError("MIssing Audio Ambient");
            return;
        }

        this.ambientAudio.Play();
    }

    public override void Play(float timeDelay = 0)
    {
        if (_Sound.callback == null)
        {
            _Sound.callback += () =>
            {
                if (this.m_MusicQueue.Count > 0)
                    StartCasstte();
                else
                {
                    _Sound.source.Stop();
                    //_Sound.source.time = 0;
                    //_Sound.source.timeSamples = 0;
                    //PoolManager.Pools[AC_POOL.POOL_AUDIO].Despawn(this.gameObject.transform);
                }

                _Sound._Count = 0;
            };
        }
        //_Sound.IsFadeOut = true;
        //_Sound.IsFadeIn = true;
        //Debug.LogError(_Sound.clip.samples);
        if (_Sound.clip != null)
            StartCoroutine(WaitTime((float)_Sound.clip.samples / (float)_Sound.clip.frequency));
    }
    public void MuteCasstte(float fadeTime, float finalVolume = .3f)
    {
        StartCoroutine(VolumeFade(this._Sound.source, fadeTime, finalVolume));
    }
    protected IEnumerator VolumeFade(AudioSource source, float fadeTime = 1f, float finalVolume = 0.3f)
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
    }
}

