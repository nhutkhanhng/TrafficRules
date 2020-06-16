using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KAudio
{
    [System.Serializable]
    public class AudioSettings : ScriptableObject
    {
        public virtual void Setting(AudioSource source) { }
        public virtual void Setting(KAudioComponent Source) { }
    }

    public class KRandom<T> where T : class
    {
        AudioConfiguration audioConfiguration = new AudioConfiguration();
    }
}
