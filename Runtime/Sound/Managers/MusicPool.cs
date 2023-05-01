using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace CoffeyUtils.Sound
{
    public class MusicPool : MonoBehaviour
    {
        [SerializeField] private float _crossFade = 1;
        [SerializeField] private AudioMixerGroup _group;
        [SerializeField, ReadOnly] private MusicTrack _queuedTrack;
        [SerializeField, ReadOnly] private List<MusicPlayer> _pool = new List<MusicPlayer>();
        [SerializeField, ReadOnly] private List<MusicPlayer> _activePlayers = new List<MusicPlayer>();

        public AudioMixerGroup MixerGroup => _group;
        public MusicPlayer GetPlayer() => RemoveFromPool(_pool.Count > 0 ? _pool[0] : CreateNewPlayer());
        public void ReturnPlayer(MusicPlayer controller) => AddToPool(controller);

        #region Pool
        
        private const int InitialPoolSize = 2;

        public void BuildInitialPool()
        {
            _pool = _pool.Where(player => player != null).ToList();
            foreach (var player in transform.GetComponentsInChildren<MusicPlayer>(true))
            {
                AddToPool(player);
            }
            for (int i = _pool.Count; i < InitialPoolSize; ++i)
            {
                AddToPool(CreateNewPlayer());
            }
        }

        private MusicPlayer CreateNewPlayer()
        {
            var player = new GameObject(SoundManager.DefaultMusicPlayerName, typeof(MusicPlayer)).GetComponent<MusicPlayer>();
            player.transform.SetParent(transform);
            return player;
        }

        private void AddToPool(MusicPlayer player)
        {
            _activePlayers.Remove(player);
            player.Reset();
            player.gameObject.SetActive(false);
            if (!_pool.Contains(player)) _pool.Add(player);
        }

        private MusicPlayer RemoveFromPool(MusicPlayer player)
        {
            _pool.Remove(player);
            _activePlayers.Add(player);
            player.gameObject.SetActive(true);
            player.Reset();
            return player;
        }
        
        #endregion
        
        public MusicPlayer Play(MusicTrack track)
        {
            StopAll();
            var player = GetPlayer();
            player.Play(track);
            QueueTrack(track);
            return player;
        }

        public void QueueTrack(MusicTrack track)
        {
            _queuedTrack = track;
        }

        public void PlayQueuedSong() => Play(_queuedTrack);

        [Button]
        public void StopAll()
        {
            var activePlayers = _activePlayers.ToArray();
            foreach (var player in activePlayers)
            {
                player.ForceFadeOutNow(_crossFade);
            }
        }

        private IEnumerator FadeIn(MusicPlayer sourceController, float fadeIn)
        {
            for (float t = 0; t < fadeIn; t += Time.deltaTime) {
                sourceController.SetCustomVolume(t / fadeIn);
                yield return null;
            }
            sourceController.SetCustomVolume(1);
        }
    }
}