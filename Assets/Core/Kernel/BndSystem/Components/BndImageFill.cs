using RedDev.Kernel.Bindings.Properties.Base;
using RedDev.Kernel.Bindings.Components.Base;
using UnityEngine;
using UnityEngine.UI;

namespace RedDev.Kernel.Bindings.Components
{
	[RequireComponent(typeof(Image))]
	public class BndImageFill : BaseBndComponent
	{
		private INumericProperty _valueAgent;
		private Image _image;

		[SerializeField] private string _propName;
		[SerializeField] private float _max = 100;

		protected override void InitializeProperties()
		{
			_valueAgent = InitProperty<Property>(_propName) as INumericProperty;
		}

		protected override void InitializeComponent()
		{
			_image = GetComponent<Image>();
		}

		protected override void OnChangedProperties()
		{
			_image.fillAmount = _valueAgent.GetFloatValue() / _max;
		}
	}
}