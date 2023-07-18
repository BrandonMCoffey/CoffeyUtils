using System.Collections;
using System.Collections.Generic;
using CoffeyUtils.Sound;
using UnityEngine;

namespace CoffeyUtils
{
    public class PlayMusic : MonoBehaviour
    {
        [SerializeField] private MusicTrack _track;
        [SerializeField] private bool _playOnStart = true;
        
        public void Start()
        {
            if (_playOnStart) Play();
        }

        private void Play()
        {
            _track.Play();
        }
    }
}
