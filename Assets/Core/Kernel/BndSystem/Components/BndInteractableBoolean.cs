using RedDev.Kernel.Bindings.Properties;
using RedDev.Kernel.Bindings.Components.Base;

namespace RedDev.Kernel.Bindings.Components
{
	public class BndInteractableBoolean : BaseBndInteractable
	{
		private BoolProperty _agentBoolean;

		protected override void InitializeProperties()
		{
			_agentBoolean = InitProperty<BoolProperty>(PropName);
		}

		protected override void OnChangedProperties()
		{
			SetInteractable(_agentBoolean.Value);
		}
	}
}