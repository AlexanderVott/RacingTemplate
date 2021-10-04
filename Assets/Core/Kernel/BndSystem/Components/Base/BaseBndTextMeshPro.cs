using TMPro;
using UnityEngine;

namespace RedDev.Kernel.Bindings.Components.Base
{
	public abstract class BaseBndTextMeshPro : BaseBndComponent
	{
		[SerializeField]
		protected string Format = "{0}";

		protected TextMeshProUGUI TextComponent;

		protected override void InitializeComponent()
		{
			TextComponent = GetComponent<TextMeshProUGUI>();
		}
	}
}