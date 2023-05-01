#if UNITY_EDITOR
using UnityEditor;

namespace CoffeyUtils.Editor.Attributes
{
	[CustomPropertyDrawer(typeof(HighlightIfAttribute))]
	public class HighlightIfAttributeDrawer : HighlightableAttributeDrawer
	{
	    protected override bool ShouldHighlight(SerializedProperty property)
	    {
	        if (attribute is not HighlightIfAttribute attr) return true;
	        var target = property.serializedObject.targetObject;
	        return ShowIfEditorHelper.ShouldShow(target, attr.Targets);
	    }
	}
}
#endif