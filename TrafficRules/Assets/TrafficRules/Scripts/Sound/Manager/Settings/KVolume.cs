using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KAudio
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Audio Resource/ Random Volume")]
    public class KVolume : AudioSettings
    {
        [Range(0, 2f)]
        public float Min = 0f;
        [Range(min: 0, max: 2f)]
        public float Max = 1f;

        public override void Setting(AudioSource source)
        {
            base.Setting(source);
            source.volume = UnityEngine.Random.Range(Min, Max);
        }

        public override void Setting(KAudioComponent Source)
        {
            base.Setting(Source);
            Setting(Source._Sound.source);
        }
    }
}