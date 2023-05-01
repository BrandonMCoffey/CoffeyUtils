using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace CoffeyUtils.Sound
{
    [CreateAssetMenu(menuName = "Sound System/Sfx Random")]
    public class SfxRandom : SfxBase
    {
        [Header("Audio Clips")]
        [SerializeField] private List<SfxReference> _clips = new List<SfxReference> {new SfxReference(true)};
        [SerializeField] private AudioMixerGroup _mixerGroup;

        [Header("Volume Settings")]
        [SerializeField, MinMaxRange(0f, 1f)] private RangedFloat _volume = new RangedFloat(0.6f, 0.8f);
        [SerializeField, MinMaxRange(0.25f, 3f)] private RangedFloat _pitch = new RangedFloat(0.9f, 1.1f);

        [Header("Extra Settings")]
        [SerializeField, MinMaxRange(-1f, 1f)] private RangedFloat _stereoPan = new RangedFloat(Sfx2dProp.DefaultStereoPan);
        [SerializeField, MinMaxRange(0f, 1.1f)] private RangedFloat _reverbZoneMix = new RangedFloat(Sfx2dProp.DefaultReverbZoneMix);
        [SerializeField, Range(0, 256)] private int _priority = Sfx2dProp.DefaultPriority;
        [SerializeField] private bool _bypassEffects = Sfx2dProp.DefaultBypassEffects;
        [SerializeField] private bool _bypassListenerEffects = Sfx2dProp.DefaultBypassListenerEffects;
        [SerializeField] private bool _bypassReverbZones = Sfx2dProp.DefaultBypassReverbZones;

        [Header("Spatial Settings")]
        [SerializeField] private bool _spatial;
        [SerializeField, MinMaxRange(0f, 1f)] private RangedFloat _spatialBlend = new RangedFloat(Sfx3dProp.DefaultSpatialBlend);
        [SerializeField, MinMaxRange(0f, 5f)] private RangedFloat _dopplerLevel = new RangedFloat(Sfx3dProp.DefaultDopplerLevel);
        [SerializeField, MinMaxRange(0f, 360f)] private RangedFloat _spread = new RangedFloat(Sfx3dProp.DefaultSpread);
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
            // Remove any clips that are null and remove the sfx reference if it is the same as this sfx event (prevent recursion)
            _clips = _clips.Where(clip => clip != null && !clip.Null() && !clip.TestSame(this)).ToList();

            // If there are no clips, return an empty reference
            if (_clips.Count == 0) {
                return new Sfx2dProp(true);
            }

            // Get Random Clip (Ensured that it is valid from top line)
            var clip = _clips[Random.Range(0, _clips.Count)];

            // Create Current Source Properties
            var myProperties = new Sfx2dProp(_mixerGroup, _volume.Random, _pitch.Random, _stereoPan.Random, _reverbZoneMix.Random, _priority, _bypassEffects, _bypassListenerEffects, _bypassReverbZones);
            
            // Add Spatial Properties (If Applicable)
            if (_spatial)
            {
                var spatial = new Sfx3dProp();
                spatial.SetProperties(_spatialBlend.Random, _dopplerLevel.Random, _spread.Random, _rolloffMode, _minDistance, _maxDistance);
                if (_customRolloff) spatial.SetCustomCurves(_rolloffCurve, _panLevelCurve, _spreadCurve, _reverbZoneMixCurve);
                myProperties.Spatial = true;
                myProperties.SpatialProperties = spatial;
            }

            // If the clip does not reference another SfxBase, return the clip
            if (clip.UseClip)
            {
                myProperties.Clip = clip.Clip;
                myProperties.Null = myProperties.Clip == null;
                return myProperties;
            }
            
            // Add properties together and return
            return myProperties.AddProperties(clip.GetSourceProperties());
        }
        
#if UNITY_EDITOR // Inspector Only Stuff
        [Button]
        private void PreviewSfx2D()
        {
            Play();
        }

        [Button(Spacing = 30)]
        private void CopyAudioSourceSpatialSettings(AudioSource source)
        {
            if (!source) return;
            _stereoPan = new RangedFloat(source.panStereo);
            _spatialBlend = new RangedFloat(source.spatialBlend);
            _dopplerLevel = new RangedFloat(source.dopplerLevel);
            _spread = new RangedFloat(source.spread);
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