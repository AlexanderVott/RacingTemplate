using RedDev.Kernel;
using RedDev.Kernel.Managers;
using RedDev.Kernel.States;
using RedDev.Helpers;
using RedDev.Helpers.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public class SceneGenerator
{
	static SceneGenerator()
	{
		Debug.Log("Initialize");
		EditorSceneManager.newSceneCreated += PrepareScene;
	}

	#region Prepare scene
	public static void PrepareScene(Scene scene, NewSceneSetup setup, NewSceneMode mode)
	{
		if (setup != NewSceneSetup.EmptyScene)
			PrepareScene(scene);
	}

	private static void PrepareScene(Scene scene)
	{
		Debug.Log("Prepare scene");
		if (scene.name.Equals("Start"))
		{
			var core = new GameObject("[CORE]", typeof(DontDestroyObject)).transform;
			var toolbox = new GameObject("Core", typeof(Core)).transform;
			toolbox.parent = core;

			var states = new GameObject("States", typeof(GlobalStatesManager)).GetComponent<GlobalStatesManager>();
			states.transform.parent = toolbox;
			var statesRoot = new GameObject("Root").transform;
			statesRoot.parent = states.transform;
			var startup = new GameObject("StartupState", typeof(StartLoadingState)).transform;
			startup.parent = statesRoot;
			states.startingState = startup.GetComponent<StartLoadingState>();

			var bootstrap = new GameObject("[BOOTSTRAP]", typeof(Bootstrap)).GetComponent<Bootstrap>();
			bootstrap.managersToAdd = new BaseManager[]{states};
			bootstrap.transform.SetAsFirstSibling();
		}

		var camera = GameObject.Find("Main Camera");
		if (camera != null)
			camera.Destroy();
		var directionalLight = GameObject.Find("Directional Light")?.transform;

		new GameObject("[SETUP]").transform.SetSiblingIndex(2);

		var world = new GameObject("[WORLD]").transform;
		var staticObjs = new GameObject("Static").transform;
		staticObjs.parent = world;
		new GameObject("Dynamic").transform.parent = world;
		var lights = new GameObject("Lights").transform;
		lights.parent = staticObjs;
		if (directionalLight != null)
			directionalLight.parent = lights;

		if (scene.name.Equals("Start"))
		{
			var ui = (new GameObject("[UI]", typeof(DontDestroyObject))).transform;
			var canvas = (new GameObject("Root", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))).transform;
			canvas.parent = ui;
		}
	}
	#endregion

	#region Create prepared scene
	[MenuItem("Assets/Create/Scenes/Prepared", priority = -100)]
	public static void CreatePreparedScene()
	{
		var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
		EditorSceneManager.SaveScene(scene, "Assets/Resources/Scenes/NewScene.unity");
	}

	[MenuItem("Assets/Create/Scenes/Starter", priority = -100)]
	public static void CreateStarterScene()
	{
		var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
		scene.name = "Start";
		PrepareScene(scene);
		EditorSceneManager.SaveScene(scene, "Assets/Resources/Scenes/Start.unity");
	}
	#endregion

	#region Create separator and folder
	[MenuItem("GameObject/Create separator", priority = -100)]
	static void CreateObjectSeparator()
	{
		var selected = Selection.activeTransform;
		var separator = new GameObject("----------------------").transform;
		separator.parent = selected;
		separator.Translate(selected.position);
		Undo.RegisterCreatedObjectUndo(separator.gameObject, "Created separator");
	}

	[MenuItem("GameObject/Create folder", priority = 0)]
	static void CreateObjectFolder()
	{
		var selected = Selection.activeTransform;
		var folder = new GameObject("[FOLDER]").transform;
		folder.parent = selected;
		folder.Translate(selected.position);
		Undo.RegisterCreatedObjectUndo(folder.gameObject, "Created folder");
	}
	#endregion

	#region Transforms reset
	[MenuItem("GameObject/Transform/Reset transform", validate = false, priority = -100)]
	public static void ResetTransform()
	{
		var selected = Selection.activeTransform;
		if (selected != null && selected.parent != null)
		{
			selected.transform.position = Vector3.zero;
			selected.transform.rotation = Quaternion.identity;
			selected.transform.localScale = Vector3.one;
			selected.Translate(selected.parent.position);
		}
	}

	[MenuItem("GameObject/Transform/Reset positions", validate = false, priority = -100)]
	public static void MoveToCenterParent()
	{
		var selected = Selection.activeTransform;
		if (selected != null && selected.parent != null)
		{
			selected.transform.position = Vector3.zero;
			selected.Translate(selected.parent.position);
		}
	}

	[MenuItem("GameObject/Transform/Reset rotations", validate = false, priority = -100)]
	public static void ResetRotation()
	{
		var selected = Selection.activeTransform;
		if (selected != null && selected.parent != null)
		{
			selected.transform.rotation = Quaternion.identity;
		}
	}

	[MenuItem("GameObject/Transform/Reset scale", validate = false, priority = -100)]
	public static void ResetScale()
	{
		var selected = Selection.activeTransform;
		if (selected != null && selected.parent != null)
		{
			selected.transform.localScale = Vector3.one;
		}
	}
	#endregion
}
