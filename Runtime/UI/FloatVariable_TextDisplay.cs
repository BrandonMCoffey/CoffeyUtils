using TMPro;
using UnityEngine;

namespace CoffeyUtils
{
	public class FloatVariable_TextDisplay : MonoBehaviour
	{
	    [SerializeField] private FloatVariable _float;
	    [SerializeField] private TMP_Text _text;
	
	    [SerializeField] private bool _customString;
	    [SerializeField, ShowIf("_customString")] private string _beforeFloat = "Value: ";
	    [SerializeField, ShowIf("_customString")] private string _afterFloat = "";
	
	    private void OnValidate()
	    {
	        if (!_text) _text = GetComponent<TMP_Text>();
	    }
	
	    private void OnEnable()
	    {
	        if (_float) _float.OnValueChanged += SetValue;
	    }
	
	    private void OnDisable()
	    {
	        if (_float) _float.OnValueChanged -= SetValue;
	    }
	
	    private void SetValue(float value)
	    {
	        _text.text = _customString ? (_beforeFloat + _float + _afterFloat) : value.ToString();
	    }
	}
}