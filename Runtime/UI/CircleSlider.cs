using UnityEngine;
using UnityEngine.UI;

namespace CoffeyUtils
{
	public class CircleSlider : MonoBehaviour
	{
	    [SerializeField, Range(0, 1)] private float _value;
	    [SerializeField] private Color _valueColor = Color.green;
	    
	    [Header("Position")]
	    [SerializeField, Range(0, 1)] private float _offset;
		[SerializeField, Range(0, 1)] private float _max;
		[SerializeField] private bool _reverseFillDirection;
	    
	    [Header("References")]
	    [SerializeField] private Image _sliderBorder;
	    [SerializeField] private Image _sliderBackground;
	    [SerializeField] private Slider _slider;
		[SerializeField] private Image _sliderFill;
	
	    private void OnValidate()
	    {
	        UpdateFullSlider();
	    }
	    
		private Quaternion MakeRot(float value) => Quaternion.Euler(new Vector3(0f, 0f, value));
	
		private void UpdateFullSlider()
		{
			float offset = _reverseFillDirection ? - _offset - _max : -_offset;
			_slider.transform.rotation = MakeRot(offset * 360f);
			
			_sliderBorder.transform.localRotation = MakeRot(_reverseFillDirection ? -1f : 1f);
			_sliderBorder.fillAmount = _max + 0.005f;
			
			_sliderBackground.fillAmount = _max;
	        
	        _slider.value = _value * _max;
			_sliderFill.color = _valueColor;
	        
			_sliderBorder.fillClockwise = !_reverseFillDirection;
			_sliderBackground.fillClockwise = !_reverseFillDirection;
			_sliderFill.fillClockwise = !_reverseFillDirection;
		}
	    
		public void UpdateSlider(float value)
		{
			_value = value;
			_slider.value = _value * _max;
		}
	}
}