using UnityEngine;

namespace RedDev.Helpers.Extensions
{
	public static class MonobehaviorExtensions
	{
		public static TP InstantiatePrefab<TP>(this MonoBehaviour self, GameObject prefab, Transform parent) 
            => MonoBehaviour.Instantiate(prefab, parent).GetComponent<TP>();

        public static TP GetRootParentComponent<TP>(this MonoBehaviour self) 
            => self.transform.root.GetComponent<TP>();
    }
}