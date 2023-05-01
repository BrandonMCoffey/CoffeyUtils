using UnityEngine;

namespace CoffeyUtils.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class SfxPlayer : MonoBehaviour
    {
        private AudioSource _source;

        [SerializeField, ReadOnly] private float _clipVolume;
        [SerializeField, ReadOnly] private bool _hasParent;
        [SerializeField, ReadOnly] private Transform _parent;
        
        private bool _reset;

        private AudioSource Source
        {
            get
            {
                if (_source == null) {
                    _source = GetComponent<AudioSource>();
                    if (_source == null) {
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
            CheckStop();
            if (_hasParent)
            {
                if (!_parent)
                {
                    Debug.LogError("Parent Object Destroyed for 3D Spatial Audio Clip: " + _source.clip.name);
                    _hasParent = false;
                    return;
                }
                transform.position = _parent.position;
            }
        }

        public void SetCustomVolume(float volume)
        {
            if (volume < 0) return;
            Source.volume = _clipVolume * volume;
        }

        public void Play(AudioClip clip)
        {
            var source = Source;
            if (source.isPlaying) source.Stop();
            Source.clip = clip;
            source.Play();
        }

        public void Play()
        {
            var source = Source;
            if (source.isPlaying) source.Stop();
            source.Play();
            Invoke(nameof(CheckStop), source.clip.length + 0.1f);
        }

        private void CheckStop()
        {
            if (!_source.isPlaying) Stop();
        }

        [Button]
        public void Stop()
        {
            Source.Stop();
            Reset();
            SoundManager.Sfx.ReturnPlayer(this);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetParent(Transform parent)
        {
            _hasParent = parent != null;
            _parent = parent;
        }

        public void SetPropertiesAndPlay(Sfx2dProp prop2d)
        {
            var source = Source;
            if (source.isPlaying) source.Stop();
            
            source.clip = prop2d.Clip;
            source.outputAudioMixerGroup = prop2d.MixerGroup ? prop2d.MixerGroup : SoundManager.Sfx.MixerGroup;
            source.volume = prop2d.Volume;
            source.pitch = prop2d.Pitch;
            source.loop = false;
            source.panStereo = prop2d.StereoPan;
            source.reverbZoneMix = prop2d.ReverbZoneMix;
            source.priority = prop2d.Priority;
            source.bypassEffects = prop2d.BypassEffects;
            source.bypassListenerEffects = prop2d.BypassListenerEffects;
            source.bypassReverbZones = prop2d.BypassReverbZones;

            if (prop2d.Spatial)
            {
                var prop3d = prop2d.SpatialProperties;
                source.spatialBlend = prop3d.SpatialBlend;
                source.dopplerLevel = prop3d.DopplerLevel;
                source.spread = prop3d.Spread;
                source.rolloffMode = prop3d.RolloffMode;
                source.minDistance = prop3d.MinDistance;
                source.maxDistance = prop3d.MaxDistance;
                if (prop3d.CustomCurves)
                {
                    source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, prop3d.RolloffCurve);
                    source.SetCustomCurve(AudioSourceCurveType.SpatialBlend, prop3d.PanLevelCurve);
                    source.SetCustomCurve(AudioSourceCurveType.Spread, prop3d.SpreadCurve);
                    source.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, prop3d.ReverbZoneMixCurve);
                }
            }
            else
            {
                Reset3dProperties();
            }

            _clipVolume = prop2d.Volume;
            _reset = false;
            
            source.Play();
        }

        public void Reset()
        {
            if (_reset) return;
            _reset = true;
            transform.localPosition = Vector3.zero;
            _hasParent = false;
            _parent = null;
            Reset2dProperties();
            Reset3dProperties();
        }

        private void Reset2dProperties()
        {
            var source = Source;
            source.clip = null;
            source.playOnAwake = false;
            source.loop = false;
            source.time = 0;
            source.volume = Sfx2dProp.DefaultVolume;
            source.pitch = Sfx2dProp.DefaultPitch;
            source.loop = false;
            source.panStereo = Sfx2dProp.DefaultStereoPan;
            source.reverbZoneMix = Sfx2dProp.DefaultReverbZoneMix;
            source.priority = Sfx2dProp.DefaultPriority;
            source.bypassEffects = Sfx2dProp.DefaultBypassEffects;
            source.bypassListenerEffects = Sfx2dProp.DefaultBypassListenerEffects;
            source.bypassReverbZones = Sfx2dProp.DefaultBypassReverbZones;

            _clipVolume = 1;
        }

        private void Reset3dProperties()
        {
            var source = Source;
            source.spatialBlend = Sfx3dProp.DefaultSpatialBlend;
            source.dopplerLevel = Sfx3dProp.DefaultDopplerLevel;
            source.spread = Sfx3dProp.DefaultSpread;
            source.rolloffMode = Sfx3dProp.DefaultRolloffMode;
            source.minDistance = Sfx3dProp.DefaultMinDistance;
            source.maxDistance = Sfx3dProp.DefaultMaxDistance;
        }
    }
}