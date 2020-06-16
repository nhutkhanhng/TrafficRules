using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

#region Define
namespace KAudio
{
    /// <summary>
    /// Enum representing the type of audio
    /// </summary>
    public enum AudioType
    {
        None,
        Music,
        Sound,
        UISound
    }
    public interface IAudio
    {
        void Play(float delayTime = 0f);
        void Pause(float delayTime = 0f);
        void Stop(float delayTime = 0f);
    }

    [Serializable]
    public class SoundData
    {
        [Header("Music Settings")]
        public AudioClip musicAudioClip;
        public AudioMixerGroup musicOutput;
    }
}
#endregion

#region Implement

namespace KAudio
{
    [Serializable]
    public class Audio
    {

        public static int audioCounter = 0;
        public string Path { get; set; }
        #region Properties
        /// <summary>
        /// The ID of the Audio
        /// </summary>
        public int AudioID { get; protected set; }

        /// <summary>
        /// The type of the Audio
        /// </summary>
        public AudioType Type { get; protected set; }

        /// <summary>
        /// Whether the audio is currently playing
        /// </summary>
        public bool IsPlaying { get; protected set; }
        /// <summary>
        /// Whether the audio is paused
        /// </summary>
        public bool Paused { get; protected set; }
        /// <summary>
        /// Whether the audio is stopping
        /// </summary>
        public bool Stopping { get; protected set; }


        /// <summary>
        /// Whether the audio is created and updated at least once. 
        /// </summary>
        public bool Activated { get; protected set; }
        /// <summary>
        /// The volume of the audio. Use SetVolume to change it.
        /// </summary>
        public float Volume { get; protected set; }

        protected AudioClip clip;
        /// <summary>
        /// Audio clip to play/is playing
        /// </summary>
        public AudioClip Clip
        {
            get { return clip; }
            set
            {
                clip = value;
                if (_AudioSource != null)
                {
                    _AudioSource.clip = clip;
                }
            }
        }

        /// <summary>
        /// The audio source that is responsible for this audio. Do not modify the audiosource directly as it could result to unpredictable behaviour. Use the Audio class instead.
        /// </summary>
        public AudioSource _AudioSource { get; protected set; }
        /// <summary>
        /// Whether the audio will be lopped
        /// </summary>
        public bool Loop
        {
            get { return loop; }
            set
            {
                loop = value;
                if (_AudioSource != null)
                {
                    _AudioSource.loop = loop;
                }
            }
        }
        /// <summary>
        /// Sets the priority of the audio
        /// </summary>
        public int Priority
        {
            get { return priority; }
            set
            {
                priority = Mathf.Clamp(value, 0, 256);
                if (_AudioSource != null)
                {
                    _AudioSource.priority = priority;
                }
            }
        }
        /// <summary>
        /// Whether the audio is muted
        /// </summary>
        public bool Mute
        {
            get { return mute; }
            set
            {
                mute = value;
                if (_AudioSource != null)
                {
                    _AudioSource.mute = mute;
                }
            }
        }
        /// <summary>
        /// The pitch of the audio
        /// </summary>
        public float Pitch
        {
            get { return pitch; }
            set
            {
                pitch = Mathf.Clamp(value, -3, 3);
                if (_AudioSource != null)
                {
                    _AudioSource.pitch = pitch;
                }
            }
        }

        /// <summary>
        /// Whether the audio persists in between scene changes
        /// </summary>
        public bool Persist { get; set; }

        /// <summary>
        /// How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)
        /// </summary>
        public float FadeInSeconds { get; set; }

        /// <summary>
        /// How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)
        /// </summary>
        public float FadeOutSeconds { get; set; }

        #endregion
        /// <summary>
        /// Stop playing audio clip
        /// </summary>
        public virtual void Stop()
        {
            fadeInterpolater = 0f;
            onFadeStartVolume = Volume;
            targetVolume = 0f;

            Stopping = true;
        }

        /// <summary>
        /// Pause playing audio clip
        /// </summary>
        public virtual void Pause()
        {
            _AudioSource.Pause();
            Paused = true;
        }

        /// <summary>
        /// Resume playing audio clip
        /// </summary>
        public virtual void UnPause()
        {
            _AudioSource.UnPause();
            Paused = false;
        }

        /// <summary>
        /// Resume playing audio clip
        /// </summary>
        public virtual void Resume()
        {
            _AudioSource.UnPause();
            Paused = false;
        }

        /// <summary>
        /// Sets the audio volume
        /// </summary>
        /// <param name="volume">The target volume</param>
        public virtual void SetVolume(float volume)
        {
            if (volume > targetVolume)
            {
                SetVolume(volume, FadeOutSeconds);
            }
            else
            {
                SetVolume(volume, FadeInSeconds);
            }
        }

        /// <summary>
        /// Sets the audio volume
        /// </summary>
        /// <param name="volume">The target volume</param>
        /// <param name="fadeSeconds">How many seconds it needs for the audio to fade in/out to reach target volume. If passed, it will override the Audio's fade in/out seconds, but only for this transition</param>
        public virtual void SetVolume(float volume, float fadeSeconds)
        {
            SetVolume(volume, fadeSeconds, this.Volume);
        }

        /// <summary>
        /// Sets the audio volume
        /// </summary>
        /// <param name="volume">The target volume</param>
        /// <param name="fadeSeconds">How many seconds it needs for the audio to fade in/out to reach target volume. If passed, it will override the Audio's fade in/out seconds, but only for this transition</param>
        /// <param name="startVolume">Immediately set the volume to this value before beginning the fade. If not passed, the Audio will start fading from the current volume towards the target volume</param>
        public virtual void SetVolume(float volume, float fadeSeconds, float startVolume)
        {
            targetVolume = Mathf.Clamp01(volume);
            fadeInterpolater = 0;
            onFadeStartVolume = startVolume;
            tempFadeSeconds = fadeSeconds;
        }

        /// <summary>
        /// Creates and initializes the audiosource component with the appropriate values
        /// </summary>
        public virtual void Create()
        {
            _AudioSource = Modeling.LoadResource<AudioSource>(this.Path) as AudioSource;
            _AudioSource.clip = this.Clip;
            _AudioSource.loop = this.Loop;
            _AudioSource.mute = this.Mute;
            _AudioSource.volume = this.Volume;
            _AudioSource.priority = this.Priority;
            _AudioSource.pitch = this.Pitch;
        }

        /// <summary>
        /// Start playing audio clip from the beginning
        /// </summary>
        public virtual void Play()
        {
            Play(initTargetVolume);
        }

        /// <summary>
        /// Start playing audio clip from the beggining
        /// </summary>
        /// <param name="volume">The target volume</param>
        public virtual void Play(float volume)
        {
            // Recreate audiosource if it does not exist
            if (_AudioSource == null)
            {
                Create();
            }

            _AudioSource.Play();
            IsPlaying = true;

            fadeInterpolater = 0f;
            onFadeStartVolume = this.Volume;
            targetVolume = volume;
        }
        /// <summary>
        /// Update loop of the Audio. This is automatically called from the sound manager itself. Do not use this function anywhere else, as it may lead to unwanted behaviour.
        /// </summary>
        public virtual void Update()
        {
            if (_AudioSource == null)
            {
                return;
            }

            Activated = true;

            // Increase/decrease volume to reach the current target
            if (Volume != targetVolume)
            {
                float fadeValue;
                fadeInterpolater += Time.deltaTime;
                if (Volume > targetVolume)
                {
                    fadeValue = tempFadeSeconds != -1 ? tempFadeSeconds : FadeOutSeconds;
                }
                else
                {
                    fadeValue = tempFadeSeconds != -1 ? tempFadeSeconds : FadeInSeconds;
                }

                Volume = Mathf.Lerp(onFadeStartVolume, targetVolume, fadeInterpolater / fadeValue);
            }
            else if (tempFadeSeconds != -1)
            {
                tempFadeSeconds = -1;
            }

            // Set the volume, taking into account the global volumes as well.
            switch (Type)
            {
                case AudioType.Music:
                    {
                        _AudioSource.volume = Volume;
                        break;
                    }
                case AudioType.Sound:
                    {
                        _AudioSource.volume = Volume;
                        break;
                    }
                case AudioType.UISound:
                    {
                        _AudioSource.volume = Volume;
                        break;
                    }
            }

            // Completely stop audio if it finished the process of stopping
            if (Volume == 0f && Stopping)
            {
                _AudioSource.Stop();
                Stopping = false;
                IsPlaying = false;
                Paused = false;
            }

            // Update playing status
            if (_AudioSource.isPlaying != IsPlaying && Application.isFocused)
            {
                IsPlaying = _AudioSource.isPlaying;
            }
        }
        #region Private Variables
        protected bool loop;
        protected bool mute;
        protected int priority;
        protected float pitch;
        protected float targetVolume;
        protected float initTargetVolume;
        protected float tempFadeSeconds;
        protected float fadeInterpolater;
        protected float onFadeStartVolume;


        protected float stereoPan;
        protected float spatialBlend;
        protected float reverbZoneMix;
        protected float dopplerLevel;
        protected float spread;
        protected AudioRolloffMode rolloffMode;
        protected float max3DDistance;
        protected float min3DDistance;
        #endregion
    }

    public class Audio2D : Audio
    {
        /// <summary>
        /// Pans a playing sound in a stereo way (left or right). This only applies to sounds that are Mono or Stereo.
        /// </summary>
        public float StereoPan
        {
            get { return stereoPan; }
            set
            {
                stereoPan = Mathf.Clamp(value, -1, 1);
                if (_AudioSource != null)
                {
                    _AudioSource.panStereo = stereoPan;
                }
            }
        }
    }
    /// <summary>
    /// The audio object
    /// </summary>
    public class Audio3D : Audio2D
    {


        /// <summary>
        /// Whether the audio is currently pooled. Do not modify this value, it is specifically used by EazySoundManager.
        /// </summary>
        public bool Pooled { get; set; }



        /// <summary>
        /// The source transform of the audio.
        /// </summary>
        public Transform SourceTransform
        {
            get { return sourceTransform; }
            set
            {
                if (value == null)
                {
                    sourceTransform = null;
                }
                else
                {
                    sourceTransform = value;
                }
            }
        }
        /// <summary>
        /// Sets how much this AudioSource is affected by 3D spatialisation calculations (attenuation, doppler etc). 0.0 makes the sound full 2D, 1.0 makes it full 3D.
        /// </summary>
        public float SpatialBlend
        {
            get { return spatialBlend; }
            set
            {
                spatialBlend = Mathf.Clamp01(value);
                if (_AudioSource != null)
                {
                    _AudioSource.spatialBlend = spatialBlend;
                }
            }
        }

        /// <summary>
        /// The amount by which the signal from the AudioSource will be mixed into the global reverb associated with the Reverb Zones.
        /// </summary>
        public float ReverbZoneMix
        {
            get { return reverbZoneMix; }
            set
            {
                reverbZoneMix = Mathf.Clamp(value, 0, 1.1f);
                if (_AudioSource != null)
                {
                    _AudioSource.reverbZoneMix = reverbZoneMix;
                }
            }
        }

        /// <summary>
        /// The doppler scale of the audio
        /// </summary>
        public float DopplerLevel
        {
            get { return dopplerLevel; }
            set
            {
                dopplerLevel = Mathf.Clamp(value, 0, 5);
                if (_AudioSource != null)
                {
                    _AudioSource.dopplerLevel = dopplerLevel;
                }
            }
        }

        /// <summary>
        /// The spread angle (in degrees) of a 3d stereo or multichannel sound in speaker space.
        /// </summary>
        public float Spread
        {
            get { return spread; }
            set
            {
                spread = Mathf.Clamp(value, 0, 360);
                if (_AudioSource != null)
                {
                    _AudioSource.spread = spread;
                }
            }
        }

        /// <summary>
        /// How the audio attenuates over distance
        /// </summary>
        public AudioRolloffMode RolloffMode
        {
            get { return rolloffMode; }
            set
            {
                rolloffMode = value;
                if (_AudioSource != null)
                {
                    _AudioSource.rolloffMode = rolloffMode;
                }
            }
        }

        /// <summary>
        /// (Logarithmic rolloff) MaxDistance is the distance a sound stops attenuating at.
        /// </summary>
        public float Max3DDistance
        {
            get { return max3DDistance; }
            set
            {
                max3DDistance = Mathf.Max(value, 0.01f);
                if (_AudioSource != null)
                {
                    _AudioSource.maxDistance = max3DDistance;
                }
            }
        }

        /// <summary>
        /// Within the Min distance the audio will cease to grow louder in volume.
        /// </summary>
        public float Min3DDistance
        {
            get { return min3DDistance; }
            set
            {
                min3DDistance = Mathf.Max(value, 0);
                if (_AudioSource != null)
                {
                    _AudioSource.minDistance = min3DDistance;
                }
            }
        }

        private Transform sourceTransform;

        public Audio3D(AudioType audioType, AudioClip clip, bool loop, bool persist, float volume, float fadeInValue, float fadeOutValue, Transform sourceTransform)
        {
            // Set unique audio ID
            AudioID = audioCounter;
            audioCounter++;

            // Initialize values
            this.Type = audioType;
            this.Clip = clip;
            this.SourceTransform = sourceTransform;
            this.Loop = loop;
            this.Persist = persist;
            this.targetVolume = volume;
            this.initTargetVolume = volume;
            this.tempFadeSeconds = -1;
            this.FadeInSeconds = fadeInValue;
            this.FadeOutSeconds = fadeOutValue;

            Volume = 0f;
            Pooled = false;

            // Set audiosource default values
            Mute = false;
            Priority = 128;
            Pitch = 1;
            StereoPan = 0;
            if (sourceTransform != null && sourceTransform != null)
            {
                SpatialBlend = 1;
            }
            ReverbZoneMix = 1;
            DopplerLevel = 1;
            Spread = 0;
            RolloffMode = AudioRolloffMode.Logarithmic;
            Min3DDistance = 1;
            Max3DDistance = 500;

            // Initliaze states
            IsPlaying = false;
            Paused = false;
            Activated = false;
        }



        /// <summary>
        /// Sets the Audio 3D distances
        /// </summary>
        /// <param name="min">the min distance</param>
        /// <param name="max">the max distance</param>
        public void Set3DDistances(float min, float max)
        {
            Min3DDistance = min;
            Max3DDistance = max;
        }
    }

}
#endregion