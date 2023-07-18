using System.Collections;
using UnityEngine;

namespace CoffeyUtils.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField, ReadOnly] private MusicTrack _track;
        [SerializeField, ReadOnly] private float _clipVolume;
	    [SerializeField, ReadOnly] private bool _active;
	    [SerializeField, ReadOnly] private bool _playedNextQueued;

        private Coroutine _fadeRoutine;
        private AudioSource _source;
        
        private AudioSource Source
        {
            get
            {
                if (!_source) {
                    _source = GetComponent<AudioSource>();
                    if (!_source) {
                        _source = gameObject.AddComponent<AudioSource>();
                    }
                }
                return _source;
            }
        }

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        private void LateUpdate()
	    {
		    if (!_playedNextQueued && _source.time > _track.FromStartWhenToPlayNextSong)
		    {
			    _playedNextQueued = true;
			    SoundManager.Music.PlayQueuedSong();
		    }
            if (!_source.isPlaying)
            {
                Stop();
                if (!_playedNextQueued) SoundManager.Music.PlayQueuedSong();
            }
        }

        public void Play(MusicTrack track, float offset = 0, bool fade = true)
        {
            _track = track;
            var source = Source;
            if (source.isPlaying) source.Stop();

            track.SetSourceProperties(source);
            _clipVolume = source.volume;

            if (offset > 0)
            {
                source.time = offset;
            }
            
            source.Play();
            _active = true;

            if (fade && track.VolumeFadeAtStart || track.VolumeFadeAtEnd)
            {
                _fadeRoutine = StartCoroutine(FadeIn());
            }
        }
        
        private IEnumerator FadeIn()
        {
            if (_track.VolumeFadeAtStart)
            {
                float deltaMultiplier1 = 1f / _track.VolumeFadeStartTime;
                for (float t = 0; t < _track.VolumeFadeStartTime; t += Time.deltaTime)
                {
                    float delta = t * deltaMultiplier1;
                    float volume = _track.VolumeFadeStartCurve.Evaluate(delta);
                    SetCustomVolume(volume);
                    yield return null;
                }
                SetCustomVolume(1);

                if (!_track.VolumeFadeAtEnd) yield break;
                
                float fadeEndTime = _track.Clip.length - _track.VolumeFadeStartTime - _track.VolumeFadeEndTime - 0.01f;
                yield return new WaitForSecondsRealtime(fadeEndTime);
            }
            else
            {
                float fadeEndTime = _track.Clip.length - _track.VolumeFadeEndTime - 0.01f;
                yield return new WaitForSecondsRealtime(fadeEndTime);
            }

            _active = false;

            float deltaMultiplier2 = 1f / _track.VolumeFadeEndTime;
            for (float t = 0; t < _track.VolumeFadeEndTime; t += Time.deltaTime)
            {
                float delta = t * deltaMultiplier2;
                float volume = _track.VolumeFadeEndCurve.Evaluate(delta);
                SetCustomVolume(volume);
                yield return null;
            }
        }

        public void ForceFadeOutNow(float time)
        {
            if (!_active) return;
            _active = false;
            if (_fadeRoutine != null)
            {
                StopCoroutine(_fadeRoutine);
                _fadeRoutine = null;
            }
            if (time <= 0)
            {
                Stop();
                return;
            }
            _fadeRoutine = StartCoroutine(FadeOut(time));
        }
        
        
        private IEnumerator FadeOut(float time)
        {
            float volume = Source.volume;
            float timeMult = 1f / time;
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                float delta = t * timeMult;
                SetCustomVolume(volume * (1 - delta));
                yield return null;
            }
            Stop();
        }
        
        public void SetCustomVolume(float volume)
        {
            Source.volume = _clipVolume * volume;
        }

        [Button]
        public void Stop()
        {
            _active = false;
            _track = null;
            Source.Stop();
            Reset();
            SoundManager.Music.ReturnPlayer(this);
            _fadeRoutine = null;
        }

        public void Reset()
        {
            var source = Source;
            source.clip = null;
            source.playOnAwake = false;
            source.time = 0;
            source.loop = false;
            source.volume = 1;
	        source.outputAudioMixerGroup = SoundManager.Music.MixerGroup;
	        _playedNextQueued = false;
        }
    }
}
