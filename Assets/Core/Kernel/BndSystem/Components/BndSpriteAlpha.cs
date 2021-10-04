using RedDev.Kernel.Bindings.Components.Base;
using RedDev.Kernel.Bindings.Properties.Base;
using UnityEngine;
using UnityEngine.UI;

namespace RedDev.Kernel.Bindings.Components
{
	[RequireComponent(typeof(Image))]
	public class BndSpriteAlpha : BaseBndComponent
	{
		private INumericProperty _agent;
		private Image _image;

		[SerializeField] private string _propName;

		protected override void InitializeProperties()
		{
			_agent = InitProperty<Property>(_propName) as INumericProperty;
		}

		protected override void InitializeComponent()
		{
			_image = GetComponent<Image>();
		}

		protected override void OnChangedProperties()
		{
            if (_agent != null)
            {
                var color = _image.color;
                color.a = _agent.GetFloatValue();
                _image.color = color;
            }
        }
	}
}