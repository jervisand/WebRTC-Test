using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Nagih
{
    public class AudioManager : MonoBehaviour
    {
        private GameObject _gameObject;
        private Dictionary<Enum.AudioSource, AudioSource> _audioSourceDictionary;
        private Dictionary<Enum.Sound, AudioClip> _soundDictionary;

        private void Awake()
        {
            _gameObject = new GameObject("Audio Controller");
            _gameObject.transform.SetParent(transform);

            _audioSourceDictionary = new Dictionary<Enum.AudioSource, AudioSource>();
            _soundDictionary = new Dictionary<Enum.Sound, AudioClip>();

            System.Array audioTypes = System.Enum.GetValues(typeof(Enum.AudioSource));
            foreach (Enum.AudioSource type in audioTypes)
            {
                AudioSource source = _gameObject.AddComponent<AudioSource>();
                source.volume = 0f;
                source.playOnAwake = false;

                _audioSourceDictionary[type] = source;
            }

            _audioSourceDictionary[Enum.AudioSource.BGM].loop = true;
        }

        public IEnumerator LoadAsset(YieldInstruction instruction = null)
        {
            System.Array soundTypes = System.Enum.GetValues(typeof(Enum.Sound));
            foreach (Enum.Sound type in soundTypes)
            {
                AudioClip clip = Resources.Load<AudioClip>("Audio/" + type);
                _soundDictionary[type] = clip;
                yield return instruction;
            }
        }

        public void PlayClip(Enum.Sound sound)
        {
            GetAudioSource(sound).Play();
        }

        public void PlayOneShot(Enum.Sound sound)
        {
            _audioSourceDictionary[Enum.AudioSource.OneShot].PlayOneShot(GetClip(sound));
        }

        public AudioSource GetAudioSource(Enum.Sound sound)
        {
            Enum.AudioSource audioType = DataStatic.GetInstance().GameDataSO.GetAudioType(sound);
            _audioSourceDictionary[audioType].clip = GetClip(sound);
            return _audioSourceDictionary[audioType];
        }

        public AudioClip GetClip(Enum.Sound sound)
        {
            return _soundDictionary[sound];
        }

        public void SyncVolume()
        {
            DeviceData device = DataSelf.GetInstance().Device;
            SetVolume(Enum.AudioSource.BGM, device.BGM);
            SetVolume(Enum.AudioSource.SFX, device.SFX);
            SetVolume(Enum.AudioSource.OneShot, device.SFX);
        }

        public void SetVolume(Enum.AudioSource audioType, float volume)
        {
            _audioSourceDictionary[audioType].volume = volume;
        }

        public void Mute()
        {
            SetVolume(Enum.AudioSource.BGM, 0f);
            SetVolume(Enum.AudioSource.SFX, 0f);
            SetVolume(Enum.AudioSource.OneShot, 0f);
        }
    }
}