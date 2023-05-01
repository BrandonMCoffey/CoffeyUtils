namespace CoffeyUtils
{
	[System.Serializable]
	public class Optional<T>
	{
		public T Value;
		public bool Enabled = true;
		
		public Optional(T value, bool enabled = true)
		{
			Value = value;
			Enabled = enabled;
		}
	}
}