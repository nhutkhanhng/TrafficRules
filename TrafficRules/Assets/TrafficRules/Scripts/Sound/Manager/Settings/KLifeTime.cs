
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace KAudio
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Audio Resource/ Life Time")]
    public class KLifeTime : AudioSettings
    {
        [Range(0f, float.MaxValue)]
        [Tooltip("Audio Clip Play At this time")]
        public float StartTime = 0f;
        [Range(0f, float.MaxValue)]
        [Tooltip("Audio Clip Play Pause this time")]
        public float EndTime = 10f;
    }
}