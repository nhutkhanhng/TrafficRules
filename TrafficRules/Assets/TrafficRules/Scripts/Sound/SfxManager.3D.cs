using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public partial class SfxManager
{
    protected List<SoundComponent> _AudioSources3D = new List<SoundComponent>();
    public AudioMixerGroup InGame;

    public SoundComponent Play(string fileName, string soundFile, Vector3 position, bool isLoop = false, float DelayTime = 0f)
    {
        SoundComponent source = LoadAndPlay(fileName, position, this.UIBus, DelayTime);
        if (source == null)
            return null;

        return source;
    }

    /// <summary>
    /// Nếu mà Duraion > 0 thì AudioSource tự động Loop
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="Position"></param>
    /// <param name="DelayTime"></param>
    /// <param name="Duration"></param>
    /// <returns></returns>
    public SoundComponent Play3D(string fileName, Vector3 Position, KAudio.KBusAudio bus = null, float DelayTime = 0f, float Duration = 0f)
    {
        var source = LoadAndPlay(fileName, Position, bus, DelayTime, Duration);

        if (source == null)
            return null;

        if (bus == null) {
            if (this.UIBus != null)
                source._Sound.source.outputAudioMixerGroup = this.UIBus.Mixer;
            else
                source._Sound.source.outputAudioMixerGroup = this.InGame;
        }

        if (Duration > 0)
            source._Sound.Loop = true;
        else
            source._Sound.Loop = false;

        return source;
    }

    public SoundComponent Play3D(string fileName, Vector3 Position, bool isLoop)
    {
        var source = LoadAndPlay(fileName, Position);

        source._Sound.Loop = isLoop;

        return source;
    }

    public SoundComponent Play3D(string fileName, Transform parent, bool isLoop = false)
    {
        var source = Play3D(fileName, parent.position);
        source.gameObject.transform.SetParent(parent, true);

        return source;
    }

    public SoundComponent Play3D(string fileName, string soundFile, Transform transf, bool isLoop = false)
    {
        SoundComponent source = GetAudioSource3D(fileName);
        if (source == null)
            return null;

        if (transf != null) {
            source.gameObject.transform.localPosition = transf.position;
            source.gameObject.transform.localRotation = transf.rotation;
        }
        else {
            source.gameObject.transform.localPosition = Vector3.zero;
        }
        source.Play();

        return source;
    }


    protected void ClearAudioSource3D()
    {
        if (_AudioSources3D != null) {
            for (int i = 0; i < _AudioSources3D.Count; i++) {
                Release(_AudioSources3D[i].gameObject);
            }
            _AudioSources3D.Clear();
        }
    }

    [ContextMenu("PauseAll3D")]
    public void StopAll3D()
    {
        if (_AudioSources3D != null) {
            for (int i = 0; i < _AudioSources3D.Count; i++) {

                if (PushToPool != null)
                    PushToPool(_AudioSources3D[i].gameObject.transform);
                else
                    Destroy(_AudioSources3D[i].gameObject);
            }
        }
    }

    public void PauseAll3D()
    {
        if (_AudioSources3D != null) {
            for (int i = 0; i < _AudioSources3D.Count; i++) {
                _AudioSources3D[i].Pause();
            }
        }
    }

    public void UnPauseAll3D()
    {
        //  Cái này sẽ dùng Audio Mixer sau.

        if (_AudioSources3D != null) {
            for (int i = 0; i < _AudioSources3D.Count; i++) {
                _AudioSources3D[i].UnPause();
            }
        }
    }

    protected SoundComponent GetAudioSource3D(string fileName)
    {
        // PoolManager.Pools[POOL.POOL_AUDIO].Spawn("Audio3D").gameObject;
        GameObject ownerSource = GetPool("Audio3D").gameObject;
        // Get AudioSource
        SoundComponent _SoundComponent = ownerSource.GetComponent<SoundComponent>();
        // Add Clip
        _SoundComponent._Sound.clip = LoadClip(fileName);

        if (_SoundComponent._Sound.clip == null) {
#if UNITY_EDITOR
            Debug.LogError(fileName);
#endif
            return null;
        }
        // Add Cache
        if (_AudioSources3D == null) {
            _AudioSources3D = new List<SoundComponent>();
        }
        if (_AudioSources3D.Contains(_SoundComponent) == false)
            _AudioSources3D.Add(_SoundComponent);

        //Debug.LogWarning(fileName);
        return _SoundComponent;
    }
}
