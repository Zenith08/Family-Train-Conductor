using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioManager
{
    public class AudioManager : MonoBehaviour
    {
        public const string NewRoute = "NewRoute";
        public const string SwitchToggle = "SwitchToggle";
        public const string TrainHorn = "TrainHorn";
        public const string TrainStopStation = "TrainStopStation";

        public AudioData[] m_backgroundMusicList;

        public AudioData[] m_sfxList;

        public static AudioManager m_instance;

        [SerializeField]
        private AudioData m_currentMusic;

        [SerializeField]
        private AudioData m_prevMusic;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            CreateInstance();

            SetUpAudioArray(m_backgroundMusicList);

            SetUpAudioArray(m_sfxList);

            ClearCurrentPrevMusic();
        }

        private void CreateInstance()
        {
            if (!m_instance)
            {
                m_instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void SetUpAudioArray(AudioData[] _array)
        {
            for (int i = 0; i < _array.Length; i++)
            {
                GameObject t_child = new GameObject(_array[i].m_name);
                t_child.transform.parent = transform;

                AudioSource t_audioSource = t_child.AddComponent<AudioSource>();

                _array[i].m_object = t_child;
                _array[i].m_audioSource = t_audioSource;

                _array[i].m_audioSource.clip = _array[i].m_audioClip;
                _array[i].m_audioSource.volume = _array[i].m_volume;
                _array[i].m_audioSource.pitch = _array[i].m_pitch;
                _array[i].m_audioSource.loop = _array[i].m_looping;
            }
        }

        private void ClearCurrentPrevMusic()
        {
            m_currentMusic = null;
            m_prevMusic = m_currentMusic;
        }

        public void PlayMusic(string _name)
        {
            Debug.Log("Playing Music");
            AudioData t_data = Array.Find(m_backgroundMusicList, bgm => bgm.m_name == _name);
            if (t_data == null)
            {
                Debug.LogError("Didnt find music");
                return;
            }
            else
            {
                m_prevMusic = m_currentMusic;
                m_currentMusic = t_data;
                PlayNextMusicTrack();
            }
        }

        public void PlayMusic(int _id)
        {
            if (_id >= 0 && _id < m_backgroundMusicList.Length)
            {
                Debug.Log("Playing Music");

                m_prevMusic = m_currentMusic;
                m_currentMusic = m_backgroundMusicList[_id];

                PlayNextMusicTrack();
            }
            else
            {
                Debug.LogError("Didnt find music");
                return;
            }
        }

        private void PlayNextMusicTrack()
        {
            m_currentMusic.m_audioSource.Play();
            if (!m_currentMusic.m_fade && m_prevMusic.m_audioSource != null)
            {
                m_prevMusic.m_audioSource.Stop();
            }
            if (m_currentMusic.m_fade)
            {
                StartCoroutine(FadeIn(m_currentMusic));
                if (m_prevMusic.m_audioSource != null)
                {
                    StartCoroutine(FadeOut(m_prevMusic));
                }
            }
        }

        public void PlaySFX(string _name)
        {
            AudioData t_data = Array.Find(m_sfxList, sfx => sfx.m_name == _name);
            if (t_data == null)
            {
                Debug.LogError("Didnt find sound");
                return;
            }
            else
            {
                t_data.m_audioSource.Play();
            }
        }

        public void PlaySFX(int _id)
        {
            if (_id >= 0 && _id < m_sfxList.Length)
            {
                Debug.Log("Playing sound");
                m_sfxList[_id].m_audioSource.Play();
            }
            else
            {
                Debug.LogError("Didnt find sound");
                return;
            }
        }

        private IEnumerator FadeIn(AudioData _audioData)
        {
            _audioData.m_audioSource.volume = 0;
            float t_volume = _audioData.m_audioSource.volume;

            while (_audioData.m_audioSource.volume < _audioData.m_volume)
            {
                t_volume += _audioData.m_fadeInSpeed;
                _audioData.m_audioSource.volume = t_volume;
                yield return new WaitForSeconds(0.1f);
            }
        }

        private IEnumerator FadeOut(AudioData _audioData)
        {
            float t_volume = _audioData.m_audioSource.volume;

            while (_audioData.m_audioSource.volume > 0)
            {
                t_volume -= _audioData.m_fadeOutSpeed;
                _audioData.m_audioSource.volume = t_volume;
                yield return new WaitForSeconds(0.1f);
            }
            if (_audioData.m_audioSource.volume == 0)
            {
                _audioData.m_audioSource.Stop();
                _audioData.m_audioSource.volume = _audioData.m_volume;
            }
        }
    }
}

namespace AudioManager
{
    [System.Serializable]
    public class AudioData
    {
        public string m_name;
        public AudioClip m_audioClip;

        public bool m_looping;

        [Range(0f, 1f)]
        public float m_volume;

        [Range(0.3f, 3f)]
        public float m_pitch;

        [Range(0f, 5f)]
        public float m_fadeInSpeed;

        [Range(0f, 5f)]
        public float m_fadeOutSpeed;

        public bool m_fade;

        [HideInInspector]
        public GameObject m_object;

        [HideInInspector]
        public AudioSource m_audioSource;
    }
}