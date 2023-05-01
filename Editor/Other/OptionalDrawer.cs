#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace CoffeyUtils.Editor
{
	[CustomPropertyDrawer(typeof(Optional<>))]
	public class OptionalDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var valueProp = property.FindPropertyRelative("Value");
			var enabledProp = property.FindPropertyRelative("Enabled");
			
			EditorGUI.BeginProperty(position, label, property);
			
			position.width -= 24;
			EditorGUI.BeginDisabledGroup(!enabledProp.boolValue);
			EditorGUI.PropertyField(position, valueProp, label, true);
			EditorGUI.EndDisabledGroup();
			
			position.x += position.width + 24;
			position.width = position.height = EditorGUI.GetPropertyHeight(enabledProp);
			position.x -= position.width;
			EditorGUI.PropertyField(position, enabledProp, GUIContent.none, true);
			
			EditorGUI.EndProperty();
		}
	}
}
#endif