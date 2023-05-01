using UnityEngine;

namespace CoffeyUtils.Sound
{
    public abstract class SfxBase : ScriptableObject
    {
        public void Play() => PlayGetPlayer();
        public SfxPlayer PlayGetPlayer() => SoundManager.PlaySfx(GetSourceProperties());

        public void Play(SfxPlayer player)
        {
            player.SetPropertiesAndPlay(GetSourceProperties());
        }

        public abstract Sfx2dProp GetSourceProperties();
    }
}