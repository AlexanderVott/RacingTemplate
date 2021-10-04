using System;
using UnityEngine;

namespace MicroRace.DB
{
	[Serializable]
	public class WheelModelData
	{
		[Serializable]
		public class FrictionData
		{
			public float slipCoefficient = 1f;
			public float forceCoefficient = 1f;
			public float maxForce = 0f;
		}

		[SerializeField]
		private FrictionData _forwardFriction = null;
		public FrictionData forwardFriction => _forwardFriction;

		[SerializeField]
		private FrictionData _sideFriction = null;
		public FrictionData sideFriction => _sideFriction;
	}
}