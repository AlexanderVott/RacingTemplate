using System;
using RedDev.Kernel.Bindings.Properties;
using RedDev.Kernel.Bindings.Components.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RedDev.Kernel.Bindings.Components
{
	[RequireComponent(typeof(Toggle))]
	public class BndToggleGroup : BaseBndComponent
	{
		private IntProperty _agent;
		[SerializeField] private int _matchValue;
		[SerializeField] private IntEvent _onEnable;

		[SerializeField] private string _propInt;
		private Toggle _toggle;

		protected override void InitializeProperties()
		{
			_agent = InitProperty<IntProperty>(_propInt);
		}

		protected override void InitializeComponent()
		{
			_toggle = GetComponent<Toggle>();
			_toggle.onValueChanged.AddListener(ToggleValueChanged);
		}

		private void ToggleValueChanged(bool isOn)
		{
			if (isOn)
			{
				_agent.Value = _matchValue;
				_onEnable.Invoke(_matchValue);
			}
			else
				OnChangedProperties();
		}

		protected override void OnChangedProperties()
		{
			_toggle.isOn = _agent.Value == _matchValue;
		}

		[Serializable]
		public class IntEvent : UnityEvent<int>
		{
		}
	}
}