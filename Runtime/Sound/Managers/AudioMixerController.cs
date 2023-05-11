using UnityEngine;
using UnityEngine.Audio;

namespace CoffeyUtils.Sound
{
    public class AudioMixerController : MonoBehaviour
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private float _startValue = 0.75f;
        [SerializeField] private string _musicVolume = "MusicVolume";
        [SerializeField] private string _sfxVolume = "SfxVolume";

        private void Start()
        {
            SetMusicVolume(_startValue);
            SetSfxVolume(_startValue);
        }

        public void SetMusicVolume(float volume)
        {
            if (_mixer == null) return;
            _mixer.SetFloat(_musicVolume, Convert(volume));
        }

        public void SetSfxVolume(float volume)
        {
            if (_mixer == null) return;
            _mixer.SetFloat(_sfxVolume, Convert(volume));
        }

        private static float Convert(float volume) => volume == 0 ? -80 : Mathf.Log10(volume) * 20;
    }
}