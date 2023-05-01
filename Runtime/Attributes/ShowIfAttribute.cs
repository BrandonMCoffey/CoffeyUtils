using System;
using UnityEngine;

namespace CoffeyUtils
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ShowIfAttribute : PropertyAttribute
	{
	    public readonly string[] Targets;
	
	    public ShowIfAttribute(params string[] target)
	    {
	        Targets = target;
	    }
	}
}