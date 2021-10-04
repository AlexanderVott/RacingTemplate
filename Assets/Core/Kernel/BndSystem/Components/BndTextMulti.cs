using System.Collections.Generic;
using RedDev.Kernel.Bindings.Properties.Base;
using RedDev.Kernel.Bindings.Components.Base;
using UnityEngine;
using UnityEngine.UI;

namespace RedDev.Kernel.Bindings.Components
{
	[RequireComponent(typeof(Text))]
	public class BndTextMulti : BaseBndText
	{
		private readonly List<Property> _propAgents = new List<Property>();

		[SerializeField] private List<string> _propNames;
		private object[] _propValues;

		protected override void InitializeProperties()
		{
			_propValues = new object[_propNames.Count];
			for (var i = 0; i < _propNames.Count; i++)
			{
				var propName = _propNames[i];
// Uncomment this if you use localization system
/*
				if (propName.StartsWith("LangKey:"))
				{
					_propValues[i] = propName.Replace("LangKey:", "").Localized();
					_propAgents.Add(null);
				}
				else if (propName.StartsWith("LangKeyUpper:"))
				{
					_propValues[i] = propName.Replace("LangKeyUpper:", "").Localized().ToUpperInvariant();
					_propAgents.Add(null);
				}
				else
				{
*/
					_propAgents.Add(InitProperty<Property>(propName));
/*
				}
*/
			}
		}

		protected override void OnChangedProperties()
		{
			for (var i = 0; i < _propAgents.Count; i++)
			{
				var agent = _propAgents[i];
				if (agent == null)
					continue;
				_propValues[i] = agent.ToString();
			}
			TextComponent.text = string.Format(Format, _propValues);
		}
	}
}