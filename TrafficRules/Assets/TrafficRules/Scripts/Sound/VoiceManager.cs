using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KAudio;
public interface IAudioManager
{
    AudioClip LoadAudioClip(string path, string soundFileExs);
    void ClearAll();
    void RemoveAudio();
    void PlayVoice(string fileName);

    void PauseAll();
}

public partial class VoiceManager : MonoSingleton<VoiceManager>, IAudioManager
{
    private static string PATH_VOICE = "Audio/Voices/";
    #region Variables

    protected Queue<KAudio.Audio> _MusicRequest;
    protected List<KAudio.IAudio> _VoiceCurrent;

    //protected 
    public List<SoundComponent> _Voices = new List<SoundComponent>();
    #endregion

    #region Properties
    public void ClearAll()
    {
        //ToDo something
    }
    #endregion

    public void PauseAll()
    {
        
    }

    #region Play

    public SoundComponent Play(string fileName, string soundFile, Vector3 position, bool isLoop = false, float DelayTime = 0f)
    {
        SoundComponent source = GetAudioSource3D(fileName);
        if (source == null)
            return null;

        source.gameObject.transform.position = position;
        source.gameObject.transform.localPosition = Vector3.zero;
        source._Sound.DelayTime = DelayTime;
        source.Play();

        return source;
    }

    public void PlayVoice(string fileName)
    {
        AudioClip voiceCall = null;
            voiceCall = LoadClip(fileName, PATH_VOICE);

        if (voiceCall != null)
        {
            Casstte.AddMusicToQueue(voiceCall);
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError(string.Format("File Voice {0} is missing", fileName));
#endif
        }

        Casstte.StartCasstte();
    }
    #endregion

    public void RemoveAudio()
    {
        
    }

    #region GetAudio
    protected SoundComponent GetAudioSource3D(string fileName)
    {
        GameObject ownerSource = Resources.Load("Audio3D") as GameObject;
        // Get AudioSource
        SoundComponent _SoundComponent = ownerSource.GetComponent<SoundComponent>();
        // Add Clip
        _SoundComponent._Sound.clip = LoadClip(fileName);

        if (_SoundComponent._Sound.clip == null)
        {
#if UNITY_EDITOR
            Debug.LogError(fileName);
#endif
            return null;
        }

        if (this.VoiceBus != null)
            if (this.VoiceBus.Add(_SoundComponent) == false)
                return null;

        // Add Cache
        if (_Voices == null)
        {
            _Voices = new List<SoundComponent>();
        }
        if (_Voices.Contains(_SoundComponent) == false)
            _Voices.Add(_SoundComponent);
        //Debug.LogWarning(fileName);
        return _SoundComponent;
    }
    #endregion

    #region LoadResource
    public AudioClip LoadAudioClip(string path, string soundFileExs)
    {
        path = path.TrimEnd('/') + "/";
        return Resources.Load<AudioClip>(path);
    }

    public AudioClip LoadClip(string fileName, string Root = "", string soundExt = ".mp3")
    {
        return KAudio.AudioManager.Instance[fileName];
    }
    #endregion
}
