using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KAudio
{
    public class KAudioComponent : MonoBehaviour, IAudio
    {
        public Func<string, Transform> GetPool = null;
        public System.Action<Transform> PushToPool = null;

        //IEnumerator RoutineWaitAndDespawn(Transform tr)
        //{
        //    yield return new WaitUntil(() => { return PoolManager.Pools[AC_POOL.POOL_AUDIO] != null; });

        //    //PushToPool = new System.Action<Transform>(PoolManager.Pools[AC_POOL.POOL_AUDIO].Despawn);
        //    //GetPool = new Func<string, Transform>(PoolManager.Pools[AC_POOL.POOL_AUDIO].Spawn);
        //}

        private void Awake()
        {
            //StartCoroutine(RoutineWaitAndDespawn(transform));
        }
        [SerializeField]
        public Sound _Sound;
        public AudioClip Clip { get { return _Sound.clip; } set { _Sound.clip = value; } }
        public virtual bool IsPlaying { get { return _Sound.Playing; } }
        public virtual void Stop() { _Sound?.Stop(); }
        public virtual void Pause() { _Sound?.Pause(); }
        public virtual void UnPause() { _Sound?.UnPause(); }
        public virtual bool Mute { get { return _Sound.source.mute; } set { _Sound.source.mute = value; } }
        public virtual float Volume { get { return _Sound.source.volume; } set { _Sound.source.volume = value; } }
        public virtual void Play(float timeDelay = 0f)
        {
            this._Sound.DelayTime = timeDelay;
            //Debug.LogError(Clip.name);
            if (_Sound.callback == null) {
                _Sound.callback += () => {
                    _Sound.source.Stop();
                    //_Sound.source.time = 0;
                    //_Sound.source.timeSamples = 0;
                    _Sound._Count = 0;

                    if (PushToPool != null)
                        PushToPool.Invoke(this.transform);
                    else
                        Destroy(this.gameObject);
                };
            }

            if (timeDelay == 0f) {
                _Sound.Playing = true;
            }
            //_Sound.IsFadeOut = true;
            //_Sound.IsFadeIn = true;
            //Debug.LogError(_Sound.clip.samples);
            if (_Sound.clip != null)
                StartCoroutine(WaitTime((float)_Sound.clip.samples / (float)_Sound.clip.frequency));
        }
        public virtual void Play(float delayTime, float Duration)
        {
            _Sound.DelayTime = delayTime;

            if (_Sound.callback == null) {
                _Sound.callback += () => {
                    _Sound.source.Stop();
                    //_Sound.source.time = 0;
                    //_Sound.source.timeSamples = 0;
                    _Sound._Count = 0;

                    if (PushToPool != null)
                        PushToPool.Invoke(this.transform);
                    else
                        Destroy(this.gameObject);
                };
            }

            if (_Sound.clip != null) {
                StartCoroutine(WaitTime(Duration));
            }
        }
        protected IEnumerator WaitTime(float time)
        {
            float remainTime = time;

            if (_Sound.DelayTime > 0)
                yield return new WaitForSeconds(_Sound.DelayTime);
            _Sound.Playing = true;

            do {
                _Sound.FadeInVolume();
                _Sound.FadeOutVolume();

                remainTime -= Time.deltaTime * _Sound.clip.frequency / _Sound.clip.samples;
                yield return new WaitForEndOfFrame();
            }
            while (remainTime >= 0);

            _Sound.callback?.Invoke();
        }
        protected IEnumerator WaitFor(float Time, System.Action before = null, System.Action after = null)
        {
            before?.Invoke();

            yield return new WaitForSeconds(Time);

            after?.Invoke();
        }
        public void Pause(float delayTime = 0)
        {
            StartCoroutine(WaitFor((delayTime), null, this.Pause));
        }

        public void Stop(float delayTime = 0)
        {
            StartCoroutine(WaitFor(delayTime, null, this.Stop));
        }
    }
}