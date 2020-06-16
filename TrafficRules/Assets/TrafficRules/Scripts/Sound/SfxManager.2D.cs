using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public partial class SfxManager
{
    protected List<AudioSource> _AudioSources2D = new List<AudioSource>();

    [Header("For UI")]
    public KAudio.KBusAudio UIBus;
    public AudioMixerGroup UIMixer;

    protected void ClearAudioSource2D()
    {
        if (_AudioSources2D != null)
        {
            for (int i = 0; i < _AudioSources2D.Count; i++)
            {
                Release(_AudioSources2D[i].gameObject);
            }
            _AudioSources2D.Clear();
        }
    }

    protected void Release(GameObject target)
    {
        Object.Destroy(target);
    }

    public AudioSource Play2D(string fileName, float FadeOutTime)
    {
        var source = Play2D(fileName, false);

        if (source != null)
            StartCoroutine(VolumeFade(source, 0f, FadeOutTime, source.Stop));

        return source;
    }

    public AudioSource Play2D(string fileName, float Wait, float FadeOutTime)
    {
        var source = Play2D(fileName);

        if (source != null)
        {
            StartCoroutine(WaitFor(Wait, null, () =>
            {
                StartCoroutine(VolumeFade(source, 0f, FadeOutTime, source.Stop));
            }));
        }

        return source;
    }

    protected IEnumerator WaitFor(float Time, System.Action before = null, System.Action after = null)
    {
        before?.Invoke();

        yield return new WaitForSeconds(Time);

        after?.Invoke();
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

    public AudioSource Play2D(string fileName, bool isLoop = false)
    {
        var clip = KAudio.AudioManager.Instance[fileName];
        AudioSource _Source = GetAudio2D();

        if (this.UIBus != null)
            _Source.outputAudioMixerGroup = this.UIBus.Mixer;
        else
            _Source.outputAudioMixerGroup = this.UIMixer;

        _Source.volume = this.Volume;
        _Source.clip = clip;
        _Source.loop = isLoop;
        _Source.Play();

        return _Source;
    }

    protected AudioSource GetAudio2D()
    {
        if (_AudioSources2D == null)
        {
            _AudioSources2D = new List<AudioSource>();
            return CreateNew2D();
        }

        for (int i = 0; i < _AudioSources2D.Count; i++)
        {
            if (_AudioSources2D[i] == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Missing audio file");
#endif
                continue;
            }

            if (_AudioSources2D[i].clip == null
                || (_AudioSources2D[i].isPlaying == false && _AudioSources2D[i].time == 0))
                    return _AudioSources2D[i];            
        }

        return CreateNew2D();
    }

    protected AudioSource CreateNew2D()
    {
        AudioSource _Source = this.gameObject.AddComponent<AudioSource>();
        _Source.playOnAwake = false;

        _AudioSources2D.Add(_Source);

        return _Source;
    }
    public void StopAll2D()
    {
        if (_AudioSources2D == null)
            return;

        for (int i = 0; i < _AudioSources2D.Count; i++)
        {
            _AudioSources2D[i].Stop();
        }
    }

    public void PauseAll2D()
    {
        if (_AudioSources2D == null)
            return;

        for (int i = 0; i < _AudioSources2D.Count; i++)
        {
            _AudioSources2D[i].Pause();
        }
    }

    public void UnPauseAll2D()
    {
        if (_AudioSources2D == null)
            return;

        for (int i = 0; i < _AudioSources2D.Count; i++)
        {
            _AudioSources2D[i].UnPause();
        }
    }

    public void MuteAll(bool isMute)
    {
        if (_AudioSources2D == null)
            return;

        for (int i = 0; i < _AudioSources2D.Count; i++)
        {
            _AudioSources2D[i].mute = isMute;
        }
    }


    public void SetVolumeAll2D(float Volume)
    {
        if (_AudioSources2D == null)
            return;

        for (int i = 0; i < _AudioSources2D.Count; i++)
        {
            _AudioSources2D[i].volume = Volume;
        }
    }
}
