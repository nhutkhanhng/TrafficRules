using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

namespace KAudio
{
    /// <summary>
    /// Bus Audio dùng trực tiếp trên các AudioClip
    /// Chứ không phải Modify Mixer Group của Audio.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(menuName = "Audio Resource/ Bus Settings")]
    public class KBusAudio : ScriptableObject
    {
        public string BusName = "[Bus Name]";
        [SerializeField]
        public List<AudioSettings> Settings;
        public AudioMixerGroup Mixer;

        public uint LimitAudio = 50;
        [NonSerialized]
        protected List<KAudioComponent> Audios = new List<KAudioComponent>();
        public bool Add(KAudioComponent newAudio)
        {
            if (Audios == null)
                Audios = new List<KAudioComponent>();

            if (Audios.Count >= LimitAudio)
                return false;

            Audios.Add(newAudio);
            newAudio._Sound.source.outputAudioMixerGroup = this.Mixer;

            if (Settings != null)
            {
                foreach (var settings in Settings)
                {
                    if (settings != null)
                        settings.Setting(newAudio);
#if UNITY_EDITOR
                    //else
                        //Debug.LogError("Settings is missing");
#endif
                }
            }

            return true;
        }

        public void UnPause()
        {
            foreach (var source in Audios)
            {
                source.UnPause();
            }
        }
        public void Pause()
        {
            foreach (var source in Audios)
            {
                source.Pause();
            }
        }

        public bool Remove(SoundComponent source)
        {
            return Audios.Remove(source);
        }
    }
}