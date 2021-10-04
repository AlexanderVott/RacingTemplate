using RedDev.Kernel.Bindings.Properties.Base;
using RedDev.Kernel.Bindings.Components.Base;
using UnityEngine;
using UnityEngine.UI;

namespace RedDev.Kernel.Bindings.Components
{
	[RequireComponent(typeof(Text))]
	public class BndText : BaseBndText
	{
		private Property _basicAgent;

		[SerializeField] private string _propName = "";

		protected override void InitializeProperties()
		{
			_basicAgent = InitProperty<Property>(_propName);
		}

		protected override void OnChangedProperties()
		{
			TextComponent.text = string.Format(Format, _basicAgent);
		}
	}
}