using RedDev.Kernel.Bindings.Properties;
using RedDev.Kernel.Bindings.Components.Base;
using RedDev.Kernel.Bindings.Properties.Base;
using UnityEngine;

namespace RedDev.Kernel.Bindings.Components
{
	[RequireComponent(typeof(RectTransform))]
	public class BndVisibilityBoolean : BaseBndVisibility
	{
		private BoolProperty _agentBoolean;

		protected override void InitializeProperties()
		{
			_agentBoolean = InitProperty<Property>(PropName) as BoolProperty;
			OnChangedProperties();
		}

		protected override void OnChangedProperties()
		{
			SetVisible(_agentBoolean != null && _agentBoolean.Value);
		}
	}
}