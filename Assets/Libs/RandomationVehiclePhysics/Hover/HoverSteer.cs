using UnityEngine;

namespace RVP {
    [DisallowMultipleComponent]
    [AddComponentMenu("RVP/Hover/Hover Steer", 2)]

    // Class for steering hover vehicles
    public class HoverSteer : MonoBehaviour {
        private Transform tr;
        private VehicleParent vp;
        public float steerRate = 1;
        private float steerAmount;

        [Tooltip("Curve for limiting steer range based on speed, x-axis = speed, y-axis = multiplier")]
        public AnimationCurve steerCurve = AnimationCurve.Linear(0, 1, 30, 0.1f);

        [Tooltip("Horizontal stretch of the steer curve")]
        public float steerCurveStretch = 1;
        public HoverWheel[] steeredWheels;

        [Header("Visual")] public bool rotate;
        public float maxDegreesRotation;
        public float rotationOffset;
        private float steerRot;

        private void Start() {
            tr = transform;
            vp = tr.GetTopmostParentComponent<VehicleParent>();
        }

        private void FixedUpdate() {
            // Set steering of hover wheels
            var rbSpeed = vp.localVelocity.z / steerCurveStretch;
            var steerLimit = steerCurve.Evaluate(Mathf.Abs(rbSpeed));
            steerAmount = vp.steerInput * steerLimit;

            foreach (var curWheel in steeredWheels)
                curWheel.steerRate = steerAmount * steerRate;
        }

        private void Update() {
            // Set visual rotation
            if (rotate) {
                steerRot = Mathf.Lerp(steerRot, steerAmount * maxDegreesRotation + rotationOffset,
                    steerRate * 0.1f * Time.timeScale);
                tr.localEulerAngles = new Vector3(tr.localEulerAngles.x, tr.localEulerAngles.y, steerRot);
            }
        }
    }
}