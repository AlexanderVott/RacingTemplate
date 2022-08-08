using RedDev.Kernel.Managers;
using RedDev.Kernel.Scenes;
using UnityEngine;

namespace RedDev.Kernel {
    public class Bootstrap : MonoBehaviour {
        [Header("Managers")] 
        public BaseManager[] managersToAdd;

        [Header("Scenes")] [Tooltip("Сцены, которые не будут выгружаться")]
        public SceneField[] scenesToKeep;

        [Tooltip("Сцены, необходимые на этапе инициализации проекта")]
        public SceneField[] scenesDependsOn;

        private void Start() {
            Core.Add<SystemEventsManager>();
            Core.Add<PrefsManager>();
            Core.Add<DBManager>();
            Core.Add<CoreScenesManager>();
            Core.Add<ContextManager>();

            foreach (var man in managersToAdd)
                Core.Add(man);

            Core.AttachManagers();
        }

        public void AfterInitialized() {
            //TODO:
        }
    }
}