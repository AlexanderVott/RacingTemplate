using RedDev.Kernel.Bindings.Properties.Base;
using RedDev.Kernel.Bindings.Components.Base;
using UnityEngine;

namespace RedDev.Kernel.Bindings.Components
{
	public class BndInteractableNumeric : BaseBndInteractable
	{
		public enum TypeBinding
		{
			LessOrEqual = 0,
			Equal = 1,
			GreaterOrEqual = 2
		}

		private INumericProperty _agent;

		[SerializeField]
		private TypeBinding _typeBinding = TypeBinding.Equal;
		[SerializeField]
		private float _point;

		protected override void InitializeProperties()
		{
			_agent = InitProperty<Property>(PropName) as INumericProperty;
		}

		protected override void OnChangedProperties()
		{
			var visibility = false;

			switch (_typeBinding)
			{
				case TypeBinding.LessOrEqual:
					visibility = _agent.GetFloatValue() <= _point;
					break;
				case TypeBinding.Equal:
					visibility = Mathf.Abs(_agent.GetFloatValue() - _point) > 0.0001f;
					break;
				case TypeBinding.GreaterOrEqual:
					visibility = _agent.GetFloatValue() >= _point;
					break;
			}

			SetInteractable(visibility);
		}
	}
}