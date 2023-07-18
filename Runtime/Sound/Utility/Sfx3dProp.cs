using UnityEngine;

namespace CoffeyUtils.Sound
{
    public class Sfx3dProp
    {
        public const float DefaultSpatialBlend = 0f;
        public const float DefaultDopplerLevel = 1f;
        public const float DefaultSpread = 0f;
        public const AudioRolloffMode DefaultRolloffMode = AudioRolloffMode.Logarithmic;
        public const float DefaultMinDistance = 1f;
        public const float DefaultMaxDistance = 10f;
        
        public float SpatialBlend;
        public float DopplerLevel;
        public float Spread;
        public AudioRolloffMode RolloffMode;
        public float MinDistance;
        public float MaxDistance;

        public bool CustomCurves;
        public AnimationCurve RolloffCurve;
        public AnimationCurve PanLevelCurve;
        public AnimationCurve SpreadCurve;
        public AnimationCurve ReverbZoneMixCurve;

        public void SetProperties(float spatialBlend, float dopplerLevel, float spread, AudioRolloffMode rolloffMode, float minDist, float maxDist)
        {
            SpatialBlend = spatialBlend;
            DopplerLevel = dopplerLevel;
            Spread = spread;
            RolloffMode = rolloffMode;
            MinDistance = minDist;
            MaxDistance = maxDist;
        }

        public void SetCustomCurves(AnimationCurve rolloffCurve, AnimationCurve panLevelCurve, AnimationCurve spreadCurve, AnimationCurve reverbZoneMixCurve)
        {
            CustomCurves = true;
            RolloffCurve = rolloffCurve;
            PanLevelCurve = panLevelCurve;
            SpreadCurve = spreadCurve;
            ReverbZoneMixCurve = reverbZoneMixCurve;
        }
    }
}