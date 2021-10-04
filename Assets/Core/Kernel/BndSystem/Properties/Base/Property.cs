using System;

namespace RedDev.Kernel.Bindings.Properties.Base
{
	public class Property
	{
		public Property()
		{
		}

		protected Action OnChanged;

		public virtual void Subscribe(Action onChanged)
		{
			OnChanged += onChanged;
		}

		public virtual void UnSubscribe(Action onChanged)
		{
			OnChanged -= onChanged;
		}
	}

	public abstract class Property<T> : Property
	{
		public T Value
		{
			get => Get();
			set => Set(value);
		}

		private T _value;

		public Property()
		{
			_value = default(T);
		}

		public Property(T value)
		{
			_value = value;
		}

		public override void Subscribe(Action onChanged)
		{
			base.Subscribe(onChanged);
			//if (_value != null)
			onChanged?.Invoke();
		}

		private T Get()
		{
			return _value;
		}

		private void Set(T value)
		{
			if (IsChanged(value))
			{
				_value = value;
				OnChanged?.Invoke();
			}
		}

		protected virtual bool IsChanged(T newValue)
		{
			var changed = (newValue == null && _value != null)
						|| (newValue != null && _value == null)
						|| ((newValue != null) && !newValue.Equals(_value));
			return changed;
		}

		public override string ToString()
		{
			if (_value == null)
				return "";
			return Value.ToString();
		}
	}

	public interface INumericProperty
	{
		float GetFloatValue();
		void SetFromFloat(float value);
	}
}