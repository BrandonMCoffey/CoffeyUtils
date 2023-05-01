using System;
using UnityEngine;

namespace CoffeyUtils
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ReadOnlyAttribute : PropertyAttribute
	{
		public RuntimeMode Mode = RuntimeMode.Always;
	}
}