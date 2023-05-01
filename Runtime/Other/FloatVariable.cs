using UnityEngine;

namespace CoffeyUtils
{
	[CreateAssetMenu]
	public class FloatVariable : ScriptableObject
	{
	    [SerializeField] private float _value;
	
	    private void OnValidate()
	    {
	        Value = _value;
	    }
	
	    public float Value
	    {
	        get => _value;
	        set
	        {
	            _value = value;
	            OnValueChanged?.Invoke(_value);
	        }
	    }
	
	    public event System.Action<float> OnValueChanged = delegate { };
	
	    public void SetValue(float value) => Value = value;
	    public void SetValue(FloatVariable value) => Value = value.Value;
	
	    public void Add(float amount) => Value += amount;
	    public void Add(FloatVariable amount) => Value += amount.Value;
	
	    public void Subtract(float amount) => Value -= amount;
	    public void Subtract(FloatVariable amount) => Value -= amount.Value;
	}
}