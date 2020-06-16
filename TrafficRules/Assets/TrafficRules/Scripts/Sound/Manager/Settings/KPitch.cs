using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KAudio
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Audio Resource/ Random Pitch")]
    public class KPitch : AudioSettings
    {
        [Range(-3f, 3f)]
        public float Min = 1f;
        [Range(-3f, 3f)]
        public float Max = 1f;

        public override void Setting(AudioSource source)
        {
            base.Setting(source);
            source.pitch = UnityEngine.Random.Range(Min, Max);
        }
        public override void Setting(KAudioComponent Source)
        {
            base.Setting(Source);
            Setting(Source._Sound.source);
        }
    }
}