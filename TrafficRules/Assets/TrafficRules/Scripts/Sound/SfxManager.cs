using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using KAudio;

public partial class SfxManager : MonoSingleton<SfxManager>, IAudioManager
{
    protected static string PATH_SFX = "Audio/SoundFXs/Characters/";
    protected static string PATH_SFX_UI = "Audio/SoundFXs/UI/";
    protected static string MIXER_SFX = "SFX";

    public Func<string, Transform> GetPool;
    public System.Action<Transform> PushToPool;

    protected Dictionary<string, AudioClip> _Sfx = new Dictionary<string, AudioClip>();

    public override void Init()
    {
        base.Init();
        //GetPool = new Func<string, Transform>(PoolManager.Pools[AC_POOL.POOL_AUDIO].Spawn);
        //PushToPool = new System.Action<Transform>(PoolManager.Pools[AC_POOL.POOL_AUDIO].Despawn);
    }

    public float Volume { get; set; } = 1f;
    public AudioMixer Mixer;
    public void ClearAll()
    {
#if !LOCAL
        return;
#endif
#pragma warning disable CS0162 // Unreachable code detected
        ClearAudioAsset();
        ClearAudioSource2D();
        ClearAudioSource3D();
#pragma warning restore CS0162 // Unreachable code detected
    }

    protected AudioClip FindClipInSfx(string name)
    {
        if (_Sfx == null)
            return null;
        if (_Sfx.ContainsKey(name))
            return _Sfx[name];
        else
            return null;
    }
    private void ClearAudioAsset()
    {
        if (_Sfx != null)
        {
            foreach (var pair in _Sfx)
            {
                Resources.UnloadAsset(pair.Value);
            }
        }
        _Sfx.Clear();
    }

    public void Play(string fileNam, Transform _Tranfs = null, bool IsLoop = false)
    {

    }

    protected SoundComponent LoadAndPlay(string fileName, Vector3 Position, KBusAudio bus = null, float DelayTime = 0f, float Duration = -1f)
    {
        SoundComponent source = GetAudioSource3D(fileName);
        if (source == null)
        {
            return null;
        }

        if (bus != null)
        {
            if (bus.Add(source) == false)
            {
                if (PushToPool != null)
                    PushToPool.Invoke(source.transform);
                else
                    Destroy(source.transform.gameObject);

                return null;
            }
            else
            {
                // Please Add All CallbackFunction
                // Do dùng cái Pool nên sợ Add thê nhiều lần
                // if (_Sound.callback == null)
                //      :: Todo
                // Ignore add to Pool of KAudioCompoenent
                source._Sound.callback = () =>
                {
                    bus.Remove(source);

                    if (PushToPool != null)
                        PushToPool.Invoke(source.transform);
                    else
                        Destroy(source.transform.gameObject);
                };
            }
        }

        source.gameObject.transform.localPosition = Position;

        if (Duration > 0)
        {
            source.Play(DelayTime, Duration);
        }
        else
            source.Play(DelayTime);

        return source;
    }

    public AudioClip LoadAudioClip(string path, string soundFileExs)
    {
        path = path.TrimEnd('/') + "/";
        return Resources.Load<AudioClip>(path);
    }

    public AudioClip LoadClip(string fileName, string Root = "", string soundExt = ".mp3")
    {
        return KAudio.AudioManager.Instance[fileName];
    }

    protected virtual AudioClip LoadSfx(string path, string fileExt)
    {
        return Modeling.LoadResource<AudioClip>(path);
    }

    public void PlayVoice(string fileName)
    {
        
    }

    public void RemoveAudio()
    {
        
    }

    public void PauseAll()
    {
        
    }

    protected AudioSource GetAudioSource()
    {
        AudioSource temp = this.gameObject.AddComponent<AudioSource>();
        temp.playOnAwake = false;

        return temp;
    }
}

