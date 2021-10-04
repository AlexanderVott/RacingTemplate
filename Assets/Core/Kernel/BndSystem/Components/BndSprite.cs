using RedDev.Kernel.Bindings.Properties;
using RedDev.Kernel.Bindings.Components.Base;
using RedDev.Kernel.Bindings.Properties.Base;
using UnityEngine;
using UnityEngine.UI;

namespace RedDev.Kernel.Bindings.Components
{
	[RequireComponent(typeof(Image))]
	public class BndSprite : BaseBndComponent
	{
		private SpriteProperty _agent;

		private Image _image;

		[SerializeField] private string _propName;

		protected override void InitializeProperties()
		{
			_agent = InitProperty<Property>(_propName) as SpriteProperty;
		}

		protected override void InitializeComponent()
		{
			_image = GetComponent<Image>();
		}

		protected override void OnChangedProperties()
		{
            _image.sprite = _agent?.Value;
		}
	}
}