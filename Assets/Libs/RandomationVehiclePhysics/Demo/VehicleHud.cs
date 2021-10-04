using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RVP {
    [DisallowMultipleComponent]
    [AddComponentMenu("RVP/Demo Scripts/Vehicle HUD", 1)]

    // Class for the HUD in the demo
    public class VehicleHud : MonoBehaviour {
        public GameObject targetVehicle;
        public Text speedText;
        public Text gearText;
        public Slider rpmMeter;
        public Slider boostMeter;
        public Text propertySetterText;
        public Text stuntText;
        public Text scoreText;
        private VehicleParent vp;
        private Motor engine;
        private Transmission trans;
        private GearboxTransmission gearbox;
        private ContinuousTransmission varTrans;
        private StuntDetect stunter;
        public bool stuntMode;
        private float stuntEndTime = -1;
        private PropertyToggleSetter propertySetter;

        private void Start() {
            Initialize(targetVehicle);
        }

        public void Initialize(GameObject newVehicle) {
            if (!newVehicle)
                return;

            targetVehicle = newVehicle;
            vp = targetVehicle.GetComponent<VehicleParent>();

            trans = targetVehicle.GetComponentInChildren<Transmission>();
            if (trans) {
                if (trans is GearboxTransmission transmission)
                    gearbox = transmission;
                else if (trans is ContinuousTransmission continuousTransmission)
                    varTrans = continuousTransmission;
            }

            if (stuntMode)
                stunter = targetVehicle.GetComponent<StuntDetect>();

            engine = targetVehicle.GetComponentInChildren<Motor>();
            propertySetter = targetVehicle.GetComponent<PropertyToggleSetter>();

            stuntText.gameObject.SetActive(stuntMode);
            scoreText.gameObject.SetActive(stuntMode);
        }

        private void Update() {
            if (vp) {
                speedText.text = (vp.velMag * 2.23694f).ToString("0") + " MPH";

                if (trans) {
                    if (gearbox)
                        gearText.text = "Gear: " + (gearbox.currentGear == 0
                                                        ? "R"
                                                        :
                                                        gearbox.currentGear == 1
                                                            ?
                                                            "N"
                                                            : (gearbox.currentGear - 1).ToString());
                    else if (varTrans)
                        gearText.text = "Ratio: " + varTrans.currentRatio.ToString("0.00");
                }

                if (engine) {
                    rpmMeter.value = engine.targetPitch;

                    if (engine.maxBoost > 0)
                        boostMeter.value = engine.boost / engine.maxBoost;
                }

                if (stuntMode && stunter) {
                    stuntEndTime = string.IsNullOrEmpty(stunter.stuntString)
                                       ? Mathf.Max(0, stuntEndTime - Time.deltaTime)
                                       : 2;

                    if (stuntEndTime == 0)
                        stuntText.text = "";
                    else if (!string.IsNullOrEmpty(stunter.stuntString))
                        stuntText.text = stunter.stuntString;

                    scoreText.text = $"Score: {stunter.score:n0}";
                }

                if (propertySetter)
                    propertySetterText.text = propertySetter.currentPreset == 0 ? "Normal Steering" :
                                              propertySetter.currentPreset == 1 ? "Skid Steering" : "Crab Steering";
            }
        }
    }
}