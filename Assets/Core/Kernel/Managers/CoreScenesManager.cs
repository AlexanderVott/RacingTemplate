using System;
using System.Collections;
using System.Collections.Generic;
using RedDev.Helpers.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RedDev.Kernel.Managers
{
	public class CoreScenesManager : BaseManager
	{
		#region Fields
		/// <summary>
		/// Сцены, которые не будут выгружаться.
		/// </summary>
		private List<string> _scenesToKeep = new List<string>();

		/// <summary>
		/// Сцены, необходимые для инициализации приложения.
		/// </summary>
		private List<string> _scenesDependsOn = new List<string>();

		public Dictionary<string, Scene> scenes { get; } = new Dictionary<string, Scene>();

        private bool changingScene = false;
        public bool ChangingScene => changingScene;
        #endregion

        public Action<Scene> sceneLoaded;
		public Action sceneClosing;
		public Action<Scene, Scene> activeSceneChanged;

        public Scene this[string sceneName] => scenes.TryGetValue(sceneName, out var result) ? result : new Scene();
		public Scene this[int indexer] => SceneManager.GetSceneAt(indexer);

		public int Count => scenes.Count;
		
		#region Initialization
		protected void Awake()
		{
			_scenesToKeep.Clear();
			_scenesDependsOn.Clear();

			Bootstrap bootstrap = FindObjectOfType<Bootstrap>();
			if (bootstrap != null)
			{
				foreach (var scene in bootstrap.scenesToKeep)
					_scenesToKeep.Add(scene.sceneName);

				foreach (var scene in bootstrap.scenesDependsOn)
					_scenesDependsOn.Add(scene.sceneName);
			}
			else
			{
                Prod.LogError("[System] CoreScenesManager could't find the Bootstrap object");
			}

			SceneManager.activeSceneChanged += ActiveSceneChangedHandler;
			SceneManager.sceneUnloaded += SceneUnloadedHandler;
			SceneManager.sceneLoaded += SceneLoadedHandler;

			Core.Instance.StartCoroutine(Setup(bootstrap));
		}

		private void ActiveSceneChangedHandler(Scene current, Scene next)
		{
			activeSceneChanged.SafeCall(current, next);
		}

		private void SceneUnloadedHandler(Scene scene)
		{
			sceneClosing.SafeCall();
		}

		private void SceneLoadedHandler(Scene scene, LoadSceneMode sceneMode)
		{
			sceneLoaded.SafeCall(scene);
			switch (sceneMode)
			{
				case LoadSceneMode.Single:
					scenes.Clear();
					break;
				case LoadSceneMode.Additive:
					if (!String.IsNullOrEmpty(scene.name) && !scenes.ContainsKey(scene.name))
						scenes.Add(scene.name, scene);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(sceneMode), sceneMode, null);
			}
		}

		private IEnumerator Setup(Bootstrap bootstrap)
		{
			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				scenes.Add(scene.name, scene);
			}

			foreach (string dependence in _scenesDependsOn)
			{
				if (scenes.ContainsKey(dependence))
				{
					yield return new WaitForSeconds(0.1f);
					continue;
				}

				var load = SceneManager.LoadSceneAsync(dependence, LoadSceneMode.Additive);
				while (!load.isDone)
					yield return null;

				scenes.Add(name, SceneManager.GetSceneByName(dependence));
			}

			//sceneLoaded();

			bootstrap.AfterInitialized();
		}
		#endregion
		
		#region Methods
		public void To(int id)
		{
			StartCoroutine(LoadCoroutine(id));
		}

		public void To(string sceneName)
		{
			StartCoroutine(LoadCoroutine(sceneName));
		}

		public void Add(int id)
		{
			StartCoroutine(AddCoroutine(id));
		}

		public void Merge(Scene sceneFrom, Scene sceneTo)
		{
			if (scenes.ContainsKey(sceneFrom.name))
				scenes.Remove(sceneFrom.name);
			SceneManager.MergeScenes(sceneFrom, sceneTo);
		}

		public void SetActiveScene(string sceneName)
		{
			var scene = FindLoadedScene(sceneName);
			if (scene.HasValue)
				SceneManager.SetActiveScene(scene.Value);
		}

		public void SetActiveScene(Scene scene)
		{
			SceneManager.SetActiveScene(scene);
		}

		public AsyncOperation AddWithAsyncResult(int id)
		{
			return SceneManager.LoadSceneAsync(id, LoadSceneMode.Additive);
		}

        public AsyncOperation ToWithAsyncResult(string sceneName)
        {
            return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
		
		public void Remove(int id)
		{
			StartCoroutine(RemoveCoroutine(id));
		}

		public void Remove(Scene scene)
		{
			Remove(scene.buildIndex);
		}

		public void Remove(string sceneName)
		{
			var scene = FindLoadedScene(sceneName);
			if (scene.HasValue)
				Remove(scene.Value);
		}

		public bool Exists(int index)
		{
			foreach (var scene in scenes.Values)
				if (scene.buildIndex == index)
					return true;
			return false;
		}

        public bool Exists(string sceneName)
        {
            foreach (var scene in scenes.Values)
                if (scene.name.Equals(sceneName, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

		public Scene? FindLoadedScene(string sceneName)
		{
			/*for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				if (scene.name.ToLower().Equals(name.ToLower()))
					return scene;
			}
			return null;*/
			return scenes.TryGetValue(sceneName, out var result) ? (Scene?) result : null;
		}
		#endregion
		
		#region Private methods
		private IEnumerator LoadCoroutine(string sceneName)
		{
			//sceneClosing();
			changingScene = true;
			Core.ClearSession();

			var currentActive = SceneManager.GetActiveScene();
			var currentName = currentActive.name;
			scenes.Remove(currentName);

			foreach (var scene in scenes.Keys)
			{
				if (_scenesToKeep.Contains(scene))
					continue;

				var jobUnload = SceneManager.UnloadSceneAsync(scenes[scene]);

				while (!jobUnload.isDone)
					yield return null;
			}
			scenes.Clear();
			var job = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			while (!job.isDone)
				yield return null;

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
			job.allowSceneActivation = true;
			changingScene = false;
		}

		private IEnumerator LoadCoroutine(int id)
		{
			//sceneClosing();
			changingScene = true;
			Core.ClearSession();

			var currentActive = SceneManager.GetActiveScene();
			var currentName = currentActive.name;

			var job = SceneManager.UnloadSceneAsync(currentName);

			while (!job.isDone)
				yield return null;

			scenes.Remove(currentName);
			foreach (var scene in scenes.Keys)
			{
				if (_scenesToKeep.Contains(scene))
					continue;

				job = SceneManager.UnloadSceneAsync(scenes[scene]);

				while (!job.isDone)
					yield return null;
			}

			job = Resources.UnloadUnusedAssets();
			while (!job.isDone)
				yield return null;

			scenes.Clear();

			job = SceneManager.LoadSceneAsync(id, LoadSceneMode.Additive);
			while (!job.isDone)
				yield return null;

			SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(id));
			job.allowSceneActivation = true;
			changingScene = false;
		}

		private IEnumerator AddCoroutine(int id)
		{
			changingScene = true;

			var job = SceneManager.LoadSceneAsync(id, LoadSceneMode.Additive);
			while (!job.isDone)
				yield return null;
			
			changingScene = false;
		}

		private IEnumerator RemoveCoroutine(int id)
		{
			changingScene = true;

			var scene = SceneManager.GetSceneByBuildIndex(id);
			if (scenes.ContainsKey(scene.name))
				scenes.Remove(scene.name);

			var job = SceneManager.UnloadSceneAsync(id);
			while (job.isDone)
				yield return null;

			changingScene = false;
		}
		#endregion
	}
}