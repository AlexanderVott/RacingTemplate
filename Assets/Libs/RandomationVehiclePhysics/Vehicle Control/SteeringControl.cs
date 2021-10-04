using UnityEngine;

namespace RVP {
    [DisallowMultipleComponent]
    [AddComponentMenu("RVP/Vehicle Controllers/Steering Control", 2)]

    // Class for steering vehicles
    public class SteeringControl : MonoBehaviour {
        private Transform tr;
        private VehicleParent vp;
        public float steerRate = 0.1f;
        private float steerAmount;

        [Tooltip("Curve for limiting steer range based on speed, x-axis = speed, y-axis = multiplier")]
        public AnimationCurve steerCurve = AnimationCurve.Linear(0, 1, 30, 0.1f);
        public bool limitSteer = true;

        [Tooltip("Horizontal stretch of the steer curve")]
        public float steerCurveStretch = 1;
        public bool applyInReverse = true; // Limit steering in reverse?
        public Suspension[] steeredWheels;

        [Header("Visual")] public bool rotate;
        public float maxDegreesRotation;
        public float rotationOffset;
        private float steerRot;

        private void Start() {
            tr = transform;
            vp = tr.GetTopmostParentComponent<VehicleParent>();
            steerRot = rotationOffset;
        }

        private void FixedUpdate() {
            var rbSpeed = vp.localVelocity.z / steerCurveStretch;
            var steerLimit = limitSteer ? steerCurve.Evaluate(applyInReverse ? Mathf.Abs(rbSpeed) : rbSpeed) : 1;
            steerAmount = vp.steerInput * steerLimit;

            // Set steer angles in wheels
            foreach (var curSus in steeredWheels)
                curSus.steerAngle = Mathf.Lerp(curSus.steerAngle,
                    steerAmount * curSus.steerFactor * (curSus.steerEnabled ? 1 : 0) * (curSus.steerInverted ? -1 : 1),
                    steerRate * TimeMaster.inverseFixedTimeFactor * Time.timeScale);
        }

        private void Update() {
            // Visual steering wheel rotation
            if (!rotate)
                return;

            steerRot = Mathf.Lerp(steerRot, steerAmount * maxDegreesRotation + rotationOffset,
                steerRate * Time.timeScale);
            tr.localEulerAngles = new Vector3(tr.localEulerAngles.x, tr.localEulerAngles.y, steerRot);
        }
    }
}