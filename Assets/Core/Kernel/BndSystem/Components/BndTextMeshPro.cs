using RedDev.Kernel.Bindings.Properties.Base;
using RedDev.Kernel.Bindings.Components.Base;
using TMPro;
using UnityEngine;

namespace RedDev.Kernel.Bindings.Components
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class BndTextMeshPro : BaseBndTextMeshPro
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