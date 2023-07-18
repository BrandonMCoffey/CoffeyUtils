using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace CoffeyUtils.Sound
{
    [CreateAssetMenu(menuName = "Sound System/Music Track")]
    public class MusicTrack : ScriptableObject
    {
        [Header("Music Track Settings")]
        [SerializeField] private AudioClip _track;
        [SerializeField, ReadOnly] private float _trackLength;
        [SerializeField, Range(0, 1)] private float _volume = 1;
        [SerializeField] private AudioMixerGroup _mixerGroup;
        [SerializeField] private MusicTrack _queueTrackNextWhenPlayed;

        [Header("Overlap Settings")]
        [Tooltip("When the song should start if a previous song was playing (Skips to the specified time to not do an unnecessary fade in.)")]
        [SerializeField] private float _startTimeIfLooping;
        [Tooltip("At what point in a song should it start playing the next queued song (Itself again if no other song specified). Refer to _startTimeIfLooping for where it starts playing it.")]
        [SerializeField] private float _fromEndWhenToPlayNextSong;
        [SerializeField, ReadOnly] private float _fromStartWhenToPlayNextSong;

        [Header("Volume Fade Settings")]
        [SerializeField] private bool _volumeFadeAtStart;
        [SerializeField, ShowIf("_volumeFadeAtStart")] private float _volumeFadeStartTime = 1;
        [SerializeField, ShowIf("_volumeFadeAtStart")] private AnimationCurve _volumeFadeStartCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private bool _volumeFadeAtEnd;
        [SerializeField, ShowIf("_volumeFadeAtEnd")] private float _volumeFadeEndTime = 1;
        [SerializeField, ShowIf("_volumeFadeAtEnd")] private AnimationCurve _volumeFadeEndCurve = AnimationCurve.Linear(0, 1, 1, 0);

        public AudioClip Clip => _track;
	    public float StartTimeIfLooping => _startTimeIfLooping;
	    public float FromStartWhenToPlayNextSong => _fromStartWhenToPlayNextSong;
        public bool VolumeFadeAtStart => _volumeFadeAtStart;
        public bool VolumeFadeAtEnd => _volumeFadeAtEnd;
        public float VolumeFadeStartTime => _volumeFadeStartTime;
        public AnimationCurve VolumeFadeStartCurve => _volumeFadeStartCurve;
        public float VolumeFadeEndTime => _volumeFadeEndTime;
        public AnimationCurve VolumeFadeEndCurve => _volumeFadeEndCurve;

        private void OnValidate()
        {
            _trackLength = _track ? _track.length : 0;
            _fromEndWhenToPlayNextSong = Mathf.Clamp(_fromEndWhenToPlayNextSong, 0, _trackLength);
            _fromStartWhenToPlayNextSong = _trackLength - _fromEndWhenToPlayNextSong;
        }

        public void Play()
        {
            SoundManager.PlayMusicNow(this);
            if (_queueTrackNextWhenPlayed != null) _queueTrackNextWhenPlayed.Queue();
        }
        public void Queue() => SoundManager.QueueMusic(this);
        
        public void SetSourceProperties(AudioSource source)
        {
            source.clip = _track;
            source.volume = _volume;
            source.outputAudioMixerGroup = _mixerGroup;
        }

#if UNITY_EDITOR
        [Button(Spacing = 10)]
        private void TestPlay()
        {
            SoundManager.PlayMusicNow(this);
        }

        private void TestQueue()
        {
            SoundManager.QueueMusic(this);
        }

        [Button]
        private void TestStop()
        {
            SoundManager.StopAllMusic();
        }

        [Button]
        private void TestLoop()
        {
            var player = SoundManager.Music.GetPlayer();
            player.Play(this, _fromStartWhenToPlayNextSong - 1, false);
            SoundManager.QueueMusic(this);
            player.StartCoroutine(TestLoopRoutine());
        }

        private static IEnumerator TestLoopRoutine()
        {
            yield return new WaitForSecondsRealtime(1);
            SoundManager.Music.PlayQueuedSong();
        }
#endif
    }
}