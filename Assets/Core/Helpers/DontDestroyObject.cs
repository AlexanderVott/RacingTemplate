using UnityEngine;

namespace RedDev.Helpers
{
	public class DontDestroyObject : MonoBehaviour
	{
		void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}