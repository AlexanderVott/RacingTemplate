using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RedDev.Kernel.Managers
{
	public class BundleInfo
	{
		public AssetBundle assetBundle = null;
		public Hash128 hash;
		public string url;
		public string name;

		public BundleInfo(string url, string name, Hash128 hash)
		{
			this.url = url;
			this.name = name;
			this.hash = hash;
		}
	}

	public class BundlesManager : BaseManager
	{
		public Dictionary<string, BundleInfo> assetBundles { get; } = new Dictionary<string,BundleInfo>();

		public List<string> availableBundles { get; } = new List<string>();

		private AssetBundle m_manifestBundle;

		#region Manifest
		public void GetManifest(string url, Action<AssetBundleManifest> callback)
		{
			StartCoroutine(StartManifestGetter(url, callback));
		}

		private IEnumerator StartManifestGetter(string url, Action<AssetBundleManifest> callback)
		{
			var promise = new Coroutine<AssetBundleManifest>(this, DownloadManifest(url));
			yield return promise.coroutine;

			availableBundles.Clear();
			availableBundles.AddRange(promise.result.GetAllAssetBundles());
			
			callback?.Invoke(promise.result);
		}
		#endregion

		#region Bundle
		public void GetBundle(string url, string nameBundle, Hash128 hash, Action<AssetBundle> callback)
		{
			StartCoroutine(StartBundleGetter(url, nameBundle, hash, callback));
		}

		public void GetBundle(string nameBundle, Action<AssetBundle> callback)
		{
			var path = Application.dataPath +
			           Path.AltDirectorySeparatorChar + "AssetBundles" +
			           Path.AltDirectorySeparatorChar + GetPlatformForAssetBundles(Application.platform);
			var manifeshPath = "file:///" + path + Path.AltDirectorySeparatorChar + GetPlatformForAssetBundles(Application.platform);
			var webPath = "file:///" + path;

			GetManifest(manifeshPath, (manifest) =>
			{
				string urlBundle = webPath;

				if (manifest == null)
				{
                    Prod.LogError("[Bundles] Failed to load manifest");
					return;
				}
				var hash = manifest.GetAssetBundleHash(nameBundle);
				GetBundle(urlBundle, nameBundle, hash, callback);
			});
		}

		private IEnumerator StartBundleGetter(string url, string nameBundle, Hash128 hash, Action<AssetBundle> callback)
		{
			var promise = new Coroutine<AssetBundle>(this,
				DownloadBundle(url, nameBundle, hash));
			yield return promise.coroutine;
			
			if (callback != null)
				callback.Invoke(promise.result);
		}
		#endregion

		#region System methods
		public string GetAssetNameWithoutExt(string assetName)
		{
			string result = Path.GetFileNameWithoutExtension(assetName);
			//result = result.Remove(result.Length - 4);
			return result;
		}

		private IEnumerator DownloadManifest(string url)
		{
			if (m_manifestBundle != null)
			{
				m_manifestBundle.Unload(true);
			}
			using (WWW www = new WWW(url))
			{
				yield return www;
				if (www.error != null)
					throw new Exception("WWW download: " + www.error);
				
				m_manifestBundle = www.assetBundle;
				yield return www.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
			}
		}

		private AssetBundle TryGetBundle(string url, Hash128 hash)
		{
			string keyName = url + hash;
			BundleInfo info;
			return assetBundles.TryGetValue(keyName, out info) ? info.assetBundle : null;
		}

		private AssetBundle TryGetBundle(string keyName)
		{
			BundleInfo info;
			return assetBundles.TryGetValue(keyName, out info) ? info.assetBundle : null;
		}

		private bool ContainBundle(string url, Hash128 hash)
		{
			string keyName = url + hash;

			return assetBundles.ContainsKey(keyName);
		}

		private IEnumerator DownloadBundle(string url, string nameBundle, Hash128 hash)
		{
			string keyName = url + Path.AltDirectorySeparatorChar + nameBundle;

			if (assetBundles.ContainsKey(nameBundle))
			{
				yield return TryGetBundle(nameBundle);
			}
			else
			{
				while (!Caching.ready)
					yield return null;

				using (WWW www = WWW.LoadFromCacheOrDownload(keyName, hash))
				{
					yield return www;
					if (www.error != null)
						throw new Exception("WWW download: " + www.error);
					
					BundleInfo info = new BundleInfo(nameBundle, nameBundle, hash);
					info.assetBundle = www.assetBundle;
					assetBundles.Add(nameBundle, info);
					yield return www.assetBundle;
				}
			}
		}

		public void Unload(string url, Hash128 hash, bool allObjects)
		{
			string keyName = url + hash;
			BundleInfo info;
			if (assetBundles.TryGetValue(keyName, out info))
			{
				info.assetBundle.Unload(allObjects);
				info.assetBundle = null;
				assetBundles.Remove(keyName);
			}
		}
		#endregion

		/// <summary>
		/// Метод конвертирует информацию о платформе запуска в строку общего названия платформы. 
		/// <para>См. так же: <seealso cref="BundlesManager.GetPlatformForAssetBundles"/></para>
		/// </summary>
		/// <param name="platform"></param>
		/// <returns></returns>
		private string GetPlatformForAssetBundles(RuntimePlatform platform)
		{
			switch (platform)
			{
				case RuntimePlatform.Android:
					return "Android";
				case RuntimePlatform.IPhonePlayer:
					return "iOS";
				case RuntimePlatform.WebGLPlayer:
					return "WebGL";
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					return "Windows";
				case RuntimePlatform.LinuxEditor:
				case RuntimePlatform.LinuxPlayer:
					return "Linux";
				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.OSXPlayer:
					return "OSX";
				case RuntimePlatform.WSAPlayerARM:
				case RuntimePlatform.WSAPlayerX64:
				case RuntimePlatform.WSAPlayerX86:
					return "WSA";
				case RuntimePlatform.PS4:
					return "PS4";
				case RuntimePlatform.XboxOne:
					return "XboxOne";
				case RuntimePlatform.Switch:
					return "Switch";
				// Add more build targets for your own.
				// If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
				default:
					return null;
			}
		}
	}
}