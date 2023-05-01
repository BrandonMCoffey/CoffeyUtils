using System;

namespace CoffeyUtils
{
	public class MinMaxRangeAttribute : Attribute
	{
	    public float Min { get; }
	    public float Max { get; }
	
	    public MinMaxRangeAttribute(float min, float max)
	    {
	        Min = min;
	        Max = max;
	    }
	}
}