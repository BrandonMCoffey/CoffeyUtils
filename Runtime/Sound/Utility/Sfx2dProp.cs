using UnityEngine;
using UnityEngine.Audio;

namespace CoffeyUtils.Sound
{
    public struct Sfx2dProp
    {
        public const float DefaultVolume = 1f;
        public const float DefaultPitch = 1f;
        public const float DefaultStereoPan = 0f;
        public const float DefaultReverbZoneMix = 1f;
        public const int DefaultPriority = 128;
        public const bool DefaultBypassEffects = false;
        public const bool DefaultBypassListenerEffects = false;
        public const bool DefaultBypassReverbZones = false;
        
        public bool Null;
        public AudioClip Clip;
        public AudioMixerGroup MixerGroup;

        public float Volume;
        public float Pitch;

        public float StereoPan;
        public float ReverbZoneMix;
        public int Priority;
        public bool BypassEffects;
        public bool BypassListenerEffects;
        public bool BypassReverbZones;

        public bool Spatial;
        public Sfx3dProp SpatialProperties;

        public Sfx2dProp AddProperties(Sfx2dProp other)
        {
            if (Clip == null) Clip = other.Clip;
            if (Clip != null) Null = false;
            
            Volume *= other.Volume;
            Pitch *= other.Pitch;
            
            StereoPan = Mathf.Clamp(StereoPan + other.StereoPan, -1f, 1f);
            ReverbZoneMix *= other.ReverbZoneMix;
            
            // Note: The other is prioritized for any of these specifics. This means that the
            // Sfx asset that is closest to related to the actual audio clip will be used for these.
            Priority = other.Priority;
            BypassEffects = other.BypassEffects;
            BypassListenerEffects = other.BypassListenerEffects;
            BypassReverbZones = other.BypassReverbZones;
            if (other.Spatial)
            {
                Spatial = true;
                SpatialProperties = other.SpatialProperties;
            }
            return this;
        }

        public Sfx2dProp(bool invalid = true)
        {
            Null = invalid;
            Clip = null;
            MixerGroup = null;
            
            Volume = DefaultVolume;
            Pitch = DefaultPitch;
            
            StereoPan = DefaultStereoPan;
            ReverbZoneMix = DefaultReverbZoneMix;
            Priority = DefaultPriority;
            BypassEffects = DefaultBypassEffects;
            BypassListenerEffects = DefaultBypassListenerEffects;
            BypassReverbZones = DefaultBypassReverbZones;
            
            Spatial = false;
            SpatialProperties = null;
        }

        public Sfx2dProp(AudioClip clip)
        {
            if (clip == null)
            {
                Null = true;
                Clip = null;
            }
            else
            {
                Null = false;
                Clip = clip;
            }
            MixerGroup = null;
            
            Volume = DefaultVolume;
            Pitch = DefaultPitch;
            
            StereoPan = DefaultStereoPan;
            ReverbZoneMix = DefaultReverbZoneMix;
            Priority = DefaultPriority;
            BypassEffects = DefaultBypassEffects;
            BypassListenerEffects = DefaultBypassListenerEffects;
            BypassReverbZones = DefaultBypassReverbZones;
            
            Spatial = false;
            SpatialProperties = null;
        }

        public Sfx2dProp(AudioMixerGroup mixerGroup, float volume, float pitch, float stereoPan, float reverbMix, int priority, bool bypassEffects, bool bypassListenerEffects, bool bypassReverbZones)
        {
            Null = true;
            Clip = null;
            MixerGroup = mixerGroup;
            
            Volume = volume;
            Pitch = pitch;
            
            StereoPan = stereoPan;
            ReverbZoneMix = reverbMix;
            Priority = priority;
            BypassEffects = bypassEffects;
            BypassListenerEffects = bypassListenerEffects;
            BypassReverbZones = bypassReverbZones;
            
            Spatial = false;
            SpatialProperties = null;
        }
    }
}