#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace CoffeyUtils.Editor.Attributes
{
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyAttributeDrawer : PropertyDrawer
	{
		// TODO: Also hide / disable the SIZE of arrays and list in inspector
	
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attr = (ReadOnlyAttribute)attribute;
			bool readonlyActive = attr.Mode.IsActive();
			
			using (new EditorGUI.DisabledScope(readonlyActive))
			{
				EditorGUI.PropertyField(position, property, label, true);
			}
		}
	}
}
#endif
