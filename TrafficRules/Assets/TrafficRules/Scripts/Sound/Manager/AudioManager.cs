using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KAudio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        public Func<string, Transform> GetPool = null;
        public System.Action<Transform> PushToPool = null;
        // This function load Pool
        //public Func<string, Transform> GetPool = new Func<string, Transform>(PoolManager.Pools[AC_POOL.POOL_AUDIO].Spawn);
        protected Dictionary<string, AudioClip> _Audioes = new Dictionary<string, AudioClip>();
        public AudioMixer Mixer;
        public void SetVolume(float volume)
        {
            Mixer.SetFloat("MasterVolume", volume);
        }

        public void GetCachingSetting()
        {
            if (Mixer == null)
                return;

            if (PlayerPrefs.GetInt("use_music", 1) == 1)
                Mixer.SetFloat("MusicVolume", 1);
            else
                Mixer.SetFloat("MusicVolume", -80f);


            if (PlayerPrefs.GetInt("use_sfx", 1) == 1)
                Mixer.SetFloat("SFXMasterVolume", 1);
            else
                Mixer.SetFloat("SFXMasterVolume", -80f);
        }

        public void Off()
        {
            Mixer.SetFloat("MasterVolume", 0f);
        }


        public AudioClip this[string Key] { get { return GetClip(Key.ToLower()); } }

        public static string PATH_AUDIO = "Audio/SoundFXs/Characters/";
        public override void Init()
        {
            base.Init();
            GetCachingSetting();
#if ASSET_BUNDLE
            return;
#endif

            //GetPool = new Func<string, Transform>(PoolManager.Pools[AC_POOL.POOL_AUDIO].Spawn);
            _Audioes = new Dictionary<string, AudioClip>();

            if (_InspectorAudio != null && _InspectorAudio.Count > 0) {
                foreach (var _audio in _InspectorAudio) {
                    this._Audioes.TryAdd(_audio.name.ToLower(), _audio);
                }
            }
            //Debug.LogError(_Audioes.Count);
        }

        public void LoadAudiosFromAssetBundle(AssetBundle data)
        {
            foreach (var clip in data.LoadAllAssets<AudioClip>()) {
                this._Audioes.TryAdd(clip.name.ToLower(), clip);
            }
        }
        public List<AudioClip> _InspectorAudio = new List<AudioClip>();
#if UNITY_EDITOR
        public string[] Paths;

        [ContextMenu ("PreLoadAudioClip")]
        public void PreloadAudio () {
            _InspectorAudio = new List<AudioClip> ();
            this._Audioes = new Dictionary<string, AudioClip> ();

            var guids = AssetDatabase.FindAssets ("t:AudioClip", Paths);

            for (int i = 0; i < guids.Length; i++) {
                string path = AssetDatabase.GUIDToAssetPath (guids[i]);
                var clip = AssetDatabase.LoadAssetAtPath (path, typeof (AudioClip)) as AudioClip;
                this._Audioes.TryAdd (clip.name, clip);
                //var prefab = Resources.Load(path);
            }

            _InspectorAudio = this._Audioes.Values.ToList ();
        }
#endif
        public AudioClip GetClip(string fileName)
        {
            return this.GetClip(fileName, PATH_AUDIO);
        }
        public AudioClip GetClip(string fileName, string RootPath)
        {
            return Load(fileName, RootPath);
        }

        public void ClearAll()
        {
            foreach (var clip in _Audioes) {
                if (clip.Value?.UnloadAudioData() == true) {
                    _Audioes.Remove(clip.Key);
                }
            }
        }

        protected AudioClip Load(string fileName, string RootPath)
        {
            if (this._Audioes != null) {
                if (_Audioes.ContainsKey(fileName)) {
                    var clip = _Audioes[fileName] ?? Modeling.LoadResource<AudioClip>(RootPath + fileName);

#if UNITY_EDITOR
                    //if (clip == null)
                    //   Debug.LogError("Mising audioclupo + " + fileName);
#endif

                    _Audioes.TryAdd(fileName, clip);
                    return clip;
                }
                else {
                    var clip = Modeling.LoadResource<AudioClip>(RootPath + fileName);
#if UNITY_EDITOR
                    //if (clip == null)
                    //    Debug.LogError("Mising audioclupo + " + fileName);
#endif
                    _Audioes.TryAdd(fileName, clip);
                    return clip;
                }
            }

#if UNITY_EDITOR
            //Debug.LogError("Mising audioclupo + " + fileName);
#endif
            return null;
        }
    }
}