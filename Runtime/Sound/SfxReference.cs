using System;
using UnityEngine;
using CoffeyUtils.Sound;

namespace CoffeyUtils
{
	[Serializable]
	public class SfxReference
	{
	    public bool UseClip;
	    public AudioClip Clip;
	    public SfxBase Base;
	
	    public SfxReference(bool useClip = false)
	    {
	        UseClip = useClip;
	        Clip = null;
	    }
	    
	    public SfxPlayer Play()
	    {
	        return Null() ? null : PlayUsingSoundManager();
	    }
	
	    public SfxPlayer PlayAtPosition(Vector3 position)
	    {
	        if (Null()) return null;
	        var player = PlayUsingSoundManager();
	        player.SetPosition(position);
	        return player;
	    }
	
	    public SfxPlayer PlayAtParentAndFollow(Transform parent)
	    {
	        if (Null()) return null;
	        var player = PlayUsingSoundManager();
	        player.SetParent(parent);
	        return player;
	    }
	
	    private SfxPlayer PlayUsingSoundManager()
	    {
	        return UseClip ? SoundManager.PlaySfx(Clip) : Base.PlayGetPlayer();
	    }
	
	    public void Play(SfxPlayer player)
	    {
	        if (Null()) return;
	        if (UseClip) player.Play(Clip);
	        else Base.Play();
	    }
	    
	    public Sfx2dProp GetSourceProperties()
	    {
	        if (Null()) return new Sfx2dProp(true);
	        return UseClip ? new Sfx2dProp(Clip) : Base.GetSourceProperties();
	    }
	
	    public bool Null()
	    {
	        if (UseClip) {
	            return Clip == null;
	        }
	        return Base == null;
	    }
	
	    public bool TestSame(SfxBase other)
	    {
	        if (!UseClip) {
	            return Base == other;
	        }
	        return false;
	    }
	}
}