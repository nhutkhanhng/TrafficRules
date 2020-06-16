using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KAudio
{

    [Serializable]
    public class Sound
    {
        public string name;

        public SoundData Data;
        public AudioClip clip
        {
            get { return Data.musicAudioClip; }
            set
            {
                Data.musicAudioClip = value;
                if (source != null) source.clip = value;
            }
        }

        public byte _Count = 0;

        public AudioSource source;
        public Action callback;
        protected bool loop;
        public bool Loop
        {
            get { return loop; }
            set { loop = value;
                this.source.loop = loop;
            }
        }

        public bool interrupts;

        private HashSet<Sound> interruptedSounds = new HashSet<Sound>();
        /// returns a float from 0.0 to 1.0 representing how much
        /// of the sound has been played so far
        public float Progress
        {
            get
            {
                if (source == null || clip == null)
                    return 0f;

                return (float)source.timeSamples / (float)clip.samples;
            }
        }

        /// returns true if the sound has finished playing
        /// will always be false for looping sounds
        public bool Finished
        {
            get
            {
                return !loop && Progress >= 1f;
            }
        }

        public float DelayTime = 0f;
        /// returns true if the sound is currently playing,
        /// false if it is paused or finished
        /// can be set to true or false to play/pause the sound
        /// will register the sound before playing
        public bool Playing
        {
            get
            {
                return source != null && source.isPlaying;
            }
            set
            {
                _Count = 0;
                if (value)
                {
                    //audioManager.RegisterSound(this);
                }
                PlayOrPause(value, interrupts);
            }
        }

        /// Try to avoid calling this directly
        /// Use AudioManager.NewSound instead
        public Sound(string newName)
        {
            name = newName;
            clip = (AudioClip)Resources.Load(name, typeof(AudioClip));
            if (clip == null)
                Debug.LogError("Couldn't find AudioClip with name '" + name + "'. Are you sure the file is in a folder named 'Resources'?");
        }

        public void Update()
        {
            if (source != null)
                source.loop = loop;

            if (Finished)
                Finish();
        }

        /// Try to avoid calling this directly
        /// Use the Sound.playing property instead
        public void PlayOrPause(bool play, bool pauseOthers, KAudio.AudioType type = KAudio.AudioType.None)
        {
            if (pauseOthers)
            {
                if (play)
                {
                    //interruptedSounds = new HashSet<Sound>(audioManager.sounds.Where(snd => snd.Playing &&
                    //                                                                        snd != this));
                }
                interruptedSounds?.ToList().ForEach(sound => sound.PlayOrPause(!play, false));
            }

            if (source != null)
            {
                if (play && !source.isPlaying)
                {
                    source?.PlayDelayed(DelayTime);
                }
                else
                {
                    //source?.Pause();
                }
            }
        }

        public void Stop()
        {
            this.source?.Stop();
        }

        public void Pause()
        {
            PlayOrPause(false, false);
        }

        public void UnPause()
        {
            this.source?.UnPause();
        }
        /// performs necessary actions when a sound finishes
        public void Finish()
        {
            PlayOrPause(false, true);

            callback?.Invoke();

            _Count++;
            //MonoBehaviour.Destroy(source);
            //source = null;
        }

        /// Reset the sound to its beginning
        public void Reset()
        {
            source.time = 0f;
        }
        public float Volume { get { return this.source.volume; } set { this.source.volume = value; } }
        #region FadeOut
        public float VolumFadeOutTarget = 0.75f;
        [Range(0, 1)]
        public float TimePercentFadeOut = 0.75f;
        public bool IsFadeOut { get; set; } = false;
        public void FadeOutVolume()
        {
            if (IsFadeOut)
            {
                if (Progress >= TimePercentFadeOut)
                {
                    Volume = Mathf.Lerp(1f, VolumFadeOutTarget, (float)(Progress - TimePercentFadeOut) / (1f - TimePercentFadeOut));
                }
            }
        }
        #endregion

        #region FadeIn
        public float VolumFadeInStart = 0.5f;
        public float VolumeFadeInTarget = 1f;
        public float TimePercentFadeIn = 0f;
        public float TimePercentTargetFadeIn = 0.5f;
        protected bool _IsFadeIn = false;
        public bool IsFadeIn
        {
            get { return _IsFadeIn; }
            set
            {
                if (_IsFadeIn == true)
                {
                    source.volume = VolumFadeInStart;
                    VolumFadeInStart = 0.5f;
                    VolumeFadeInTarget = 1f;
                    TimePercentFadeIn = 0f;
                    TimePercentTargetFadeIn = 0.5f;
                }

                _IsFadeIn = value;
            }
        }
        public void FadeInVolume()
        {
            if (IsFadeIn)
            {
                Debug.LogError((float)(Progress) / (float)(TimePercentTargetFadeIn - TimePercentFadeIn));
                if (Progress >= TimePercentFadeIn && Progress <= TimePercentTargetFadeIn)
                {
                    Volume = Mathf.Lerp(VolumFadeInStart, VolumeFadeInTarget, (float)(Progress) / (float)(TimePercentTargetFadeIn - TimePercentFadeIn));
                }
            }
        }
        #endregion
    }

}