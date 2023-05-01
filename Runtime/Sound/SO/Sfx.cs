using UnityEngine;
using UnityEngine.Audio;

namespace CoffeyUtils.Sound
{
    [CreateAssetMenu(menuName = "Sound System/Sfx")]
    public class Sfx : SfxBase
    {
        [Header("Audio Clip")]
        [SerializeField] private SfxReference _clip = new SfxReference(true);
        [SerializeField] private AudioMixerGroup _mixerGroup;
        
        [Header("Volume Settings")]
        [SerializeField, Range(0f, 1f)] private float _volume = Sfx2dProp.DefaultVolume;
        [SerializeField, Range(.25f, 3f)] private float _pitch = Sfx2dProp.DefaultPitch;

        [Header("Extra Settings")]
        [SerializeField, Range(-1f, 1f)] private float _stereoPan = Sfx2dProp.DefaultStereoPan;
        [SerializeField, Range(0f, 1.1f)] private float _reverbZoneMix = Sfx2dProp.DefaultReverbZoneMix;
        [SerializeField, Range(0, 256)] private int _priority = Sfx2dProp.DefaultPriority;
        [SerializeField] private bool _bypassEffects = Sfx2dProp.DefaultBypassEffects;
        [SerializeField] private bool _bypassListenerEffects = Sfx2dProp.DefaultBypassListenerEffects;
        [SerializeField] private bool _bypassReverbZones = Sfx2dProp.DefaultBypassReverbZones;

        [Header("Spatial Settings")]
        [SerializeField] private bool _spatial;
        [SerializeField, ShowIf("_spatial"), Range(0f, 1f)] private float _spatialBlend = Sfx3dProp.DefaultSpatialBlend;
        [SerializeField, ShowIf("_spatial"), Range(0f, 5f)] private float _dopplerLevel = Sfx3dProp.DefaultDopplerLevel;
        [SerializeField, ShowIf("_spatial")] private float _spread = Sfx3dProp.DefaultSpread;
        [SerializeField, ShowIf("_spatial")] private AudioRolloffMode _rolloffMode = Sfx3dProp.DefaultRolloffMode;
        [SerializeField, ShowIf("_spatial")] private float _minDistance = Sfx3dProp.DefaultMinDistance;
        [SerializeField, ShowIf("_spatial")] private float _maxDistance = Sfx3dProp.DefaultMaxDistance;
        [SerializeField, HideInInspector] private bool _customRolloff;
        [SerializeField, ShowIf("_spatial", "_customRolloff")] private AnimationCurve _rolloffCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
        [SerializeField, ShowIf("_spatial", "_customRolloff")] private AnimationCurve _panLevelCurve = new AnimationCurve(new Keyframe());
        [SerializeField, ShowIf("_spatial", "_customRolloff")] private AnimationCurve _spreadCurve = new AnimationCurve(new Keyframe());
        [SerializeField, ShowIf("_spatial", "_customRolloff")] private AnimationCurve _reverbZoneMixCurve;

        private void OnValidate()
        {
            _customRolloff = _rolloffMode == AudioRolloffMode.Custom;
        }
        
        public override Sfx2dProp GetSourceProperties()
        {
            // Ensure clip is not null or the same (prevent recursion)
            if (_clip.Null() || _clip.TestSame(this)) return new Sfx2dProp(true);

            // Create Current Source Properties
            var myProperties = new Sfx2dProp(_mixerGroup, _volume, _pitch, _stereoPan, _reverbZoneMix, _priority, _bypassEffects, _bypassListenerEffects, _bypassReverbZones);
            
            // Add Spatial Properties (If Applicable)
            if (_spatial)
            {
                var spatial = new Sfx3dProp();
                spatial.SetProperties(_spatialBlend, _dopplerLevel, _spread, _rolloffMode, _minDistance, _maxDistance);
                if (_customRolloff) spatial.SetCustomCurves(_rolloffCurve, _panLevelCurve, _spreadCurve, _reverbZoneMixCurve);
                myProperties.Spatial = true;
                myProperties.SpatialProperties = spatial;
            }

            // If the clip does not reference another SfxBase, return the clip
            if (_clip.UseClip)
            {
                myProperties.Clip = _clip.Clip;
                myProperties.Null = myProperties.Clip == null;
                return myProperties;
            }

            // Add properties together and return
            return myProperties.AddProperties(_clip.GetSourceProperties());
        }
        
        
#if UNITY_EDITOR // Inspector Only Stuff
        [Button]
        private void PreviewSfx2D()
        {
            Play();
        }

        [Button(Spacing = 30)]
        private void CopyAudioSourceSettings(AudioSource source)
        {
            _volume = source.volume;
            _pitch = source.pitch;
            _reverbZoneMix = source.reverbZoneMix;
            _priority = source.priority;
            _bypassEffects = source.bypassEffects;
            _bypassListenerEffects = source.bypassListenerEffects;
            _bypassReverbZones = source.bypassReverbZones;
            _spatial = source.spatialBlend > 0;
            _stereoPan = source.panStereo;
            _spatialBlend = source.spatialBlend;
            _dopplerLevel = source.dopplerLevel;
            _spread = source.spread;
            _rolloffMode = source.rolloffMode;
            _minDistance = source.minDistance;
            _maxDistance = source.maxDistance;
            _rolloffCurve = source.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
            _panLevelCurve = source.GetCustomCurve(AudioSourceCurveType.SpatialBlend);
            _spreadCurve = source.GetCustomCurve(AudioSourceCurveType.Spread);
            _reverbZoneMixCurve = source.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix);
        }
#endif
    }
}