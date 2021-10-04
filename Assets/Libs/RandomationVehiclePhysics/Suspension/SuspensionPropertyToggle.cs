using UnityEngine;

namespace RVP {
    [RequireComponent(typeof(Suspension))]
    [DisallowMultipleComponent]
    [AddComponentMenu("RVP/Suspension/Suspension Property", 2)]

    // Class for changing the properties of the suspension
    public class SuspensionPropertyToggle : MonoBehaviour {
        public SuspensionToggledProperty[] properties;
        private Suspension sus;

        private void Start() {
            sus = GetComponent<Suspension>();
        }

        // Toggle a property in the properties array at index
        public void ToggleProperty(int index) {
            if (properties.Length - 1 >= index) {
                properties[index].toggled = !properties[index].toggled;

                if (sus)
                    sus.UpdateProperties();
            }
        }

        // Set a property in the properties array at index to the value
        public void SetProperty(int index, bool value) {
            if (properties.Length - 1 >= index) {
                properties[index].toggled = value;

                if (sus)
                    sus.UpdateProperties();
            }
        }
    }

    // Class for a single property
    [System.Serializable]
    public class SuspensionToggledProperty {
        public enum Properties {
            steerEnable, //enable steering
            steerInvert, //invert steering
            driveEnable, //enable driving
            driveInvert, //invert drive
            ebrakeEnable, //can ebrake
            skidSteerBrake, //brake is specially adjusted for skid steering
        } // The type of property
        
        public Properties property; // The property
        public bool toggled;        // Is it enabled?
    }
}