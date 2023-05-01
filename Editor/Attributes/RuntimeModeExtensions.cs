#if UNITY_EDITOR
using UnityEditor;

namespace CoffeyUtils.Editor.Attributes
{
	public static class RuntimeModeExtensions
	{
		public static bool IsActive(this RuntimeMode mode)
		{
			bool inPlayMode = EditorApplication.isPlaying;
			
			switch (mode)
			{
			case RuntimeMode.Always:
				return true;
			case RuntimeMode.OnlyPlaying:
				return inPlayMode;
			case RuntimeMode.OnlyEditor:
				return !inPlayMode;
			default:
				return false;
			}
		}
	}
}
#endif