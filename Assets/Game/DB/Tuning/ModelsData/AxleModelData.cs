using System;
using UnityEngine;

namespace Game.DB
{
	[Serializable]
	public class AxleModelData
	{
		[Serializable]
		public class SpringData
		{
			public float maxForce;

			[Tooltip("Spring travel")]
			public float maxLength;

			[Tooltip("Spring force curve")]
			public AnimationCurve forceCurve;
		}

		[Serializable]
		public class DamperData
		{
			[Tooltip("Bump N/m/s")]
			public float bumpForce;

			[Tooltip("Rebound N/m/s")]
			public float reboundForce;

			public AnimationCurve dampingCurve;
		}

		/*[SerializeField]
		private Axle.Geometry _geometry = null;
		public Axle.Geometry geometry => _geometry;*/

		[Range(0f, 1f)]
		[SerializeField]
		private float _powerCoefficient = 1f;
		public float powerCoefficient
		{
			get=>_powerCoefficient;
			set => _powerCoefficient = Mathf.Clamp01(value);
		}

		[Range(0f, 1f)]
		[SerializeField]
		private float _brakeCoefficient = 1f;
		public float brakeCoefficient
		{
			get => _brakeCoefficient;
			set => _brakeCoefficient = Mathf.Clamp01(value);
		}

		[Range(0f, 1f)]
		[SerializeField]
		private float _handbrakeCoefficient;
		public float handbrakeCoefficient
		{
			get => _handbrakeCoefficient;
			set => _handbrakeCoefficient = Mathf.Clamp01(value);
		}

		[Range(0f, 1f)]
		[SerializeField]
		private float _differentialStrength;
		public float differentialStrength
		{
			get => _differentialStrength;
			set => _differentialStrength = Mathf.Clamp01(value);
		}

		//public Axle.DifferentialType differentialType = Axle.DifferentialType.LimitedSlip;
		public SpringData spring;
		public DamperData damper;
	}
}