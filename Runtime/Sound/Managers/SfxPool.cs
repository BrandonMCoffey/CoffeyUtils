using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace CoffeyUtils.Sound
{
    public class SfxPool : MonoBehaviour
    {
        
        [SerializeField] private AudioMixerGroup _group;
        [SerializeField, ReadOnly] private List<SfxPlayer> _pool = new List<SfxPlayer>();
        [SerializeField, ReadOnly] private List<SfxPlayer> _activePlayers = new List<SfxPlayer>();

        public AudioMixerGroup MixerGroup => _group;
        public SfxPlayer GetPlayer() => RemoveFromPool(_pool.Count > 0 ? _pool[0] : CreateNewPlayer());
        public void ReturnPlayer(SfxPlayer controller) => AddToPool(controller);

        #region Pool
        
        private const int InitialPoolSize = 5;
        
        public void BuildInitialPool()
        {
            _pool = _pool.Where(player => player != null).ToList();
            foreach (var player in transform.GetComponentsInChildren<SfxPlayer>(true))
            {
                AddToPool(player);
            }
            for (int i = _pool.Count; i < InitialPoolSize; ++i)
            {
                AddToPool(CreateNewPlayer());
            }
        }

        private SfxPlayer CreateNewPlayer()
        {
            var player = new GameObject(SoundManager.DefaultSfxPlayerName, typeof(SfxPlayer)).GetComponent<SfxPlayer>();
            player.transform.SetParent(transform);
            return player;
        }

        private void AddToPool(SfxPlayer player)
        {
            _activePlayers.Remove(player);
            player.Reset();
            player.gameObject.SetActive(false);
            if (!_pool.Contains(player)) _pool.Add(player);
        }

        private SfxPlayer RemoveFromPool(SfxPlayer player)
        {
            _pool.Remove(player);
            _activePlayers.Add(player);
            player.gameObject.SetActive(true);
            player.Reset();
            return player;
        }
        
        #endregion
        
        public SfxPlayer Play(AudioClip clip)
        {
            var player = GetPlayer();
            player.Play(clip);
            return player;
        }
        
        public SfxPlayer Play(Sfx2dProp properties2d)
        {
            if (properties2d.Null) return null;
            var player = GetPlayer();
            player.SetPropertiesAndPlay(properties2d);
            player.Play();
            return player;
        }

        [Button]
        public void StopAll()
        {
            // Make them all Fade Out!
            var activePlayers = _activePlayers.ToArray();
            foreach (var player in activePlayers)
            {
                player.Stop();
            }
        }
    }
}
