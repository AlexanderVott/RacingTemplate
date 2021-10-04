using UnityEngine;

namespace MicroRace.Vehicles {
    public class BaseVehicleController : MonoBehaviour {
        public virtual void SetActive(bool active) {
            gameObject.SetActive(active);
        }
    }
}