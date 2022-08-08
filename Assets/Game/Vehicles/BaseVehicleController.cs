using UnityEngine;

namespace Game.Vehicles {
    public class BaseVehicleController : MonoBehaviour {
        public virtual void SetActive(bool active) {
            gameObject.SetActive(active);
        }
    }
}