using System;
using UnityEngine;

namespace CoffeyUtils
{
	[Serializable]
	public struct RangedFloat
	{
	    public float MinValue;
	    public float MaxValue;
	
	    public RangedFloat(float value) {
	        MinValue = value;
	        MaxValue = value;
	    }
	
	    public RangedFloat(float min, float max)
	    {
	        MinValue = min;
	        MaxValue = max;
	    }
	
	    public float Random => UnityEngine.Random.Range(MinValue, MaxValue);
	
	    public float Clamp(float value) => Mathf.Clamp(value, MinValue, MaxValue);
	}
}