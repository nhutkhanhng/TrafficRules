using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public partial class VoiceManager
{
    public AudioMiniCasstte Casstte;

    [Header("Use for Mixer Voice")]
    public KAudio.KBusAudio VoiceBus;
    public AudioMixerGroup mixerGroup;

    public void PlayBuyChessAction(string fileName, string Root ="", string soundExt= ".mp3")
    {

        AudioClip voiceCall = null;
        if (string.IsNullOrEmpty(Root))
            voiceCall = LoadClip(fileName, PATH_VOICE, soundExt);
        else
            voiceCall = LoadClip(fileName, Root, soundExt);

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

    /// <summary>
    /// This is main function
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public SoundComponent PlayVoice(string fileName, Vector3 position)
    {
        SoundComponent source = GetAudioSource3D(fileName);

        if (source == null)
            return null;

        source.gameObject.transform.localPosition = position;
        source.Play();

        return source;
    }
}
