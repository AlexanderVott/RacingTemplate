using System;
using System.Dynamic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace RedDev.Helpers {
    public interface IKeepAliveMonoBehaviourSingleton { }

    public interface IAlwaysAccessibleOnQuit { }

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        public static bool isQuitting = false;
        public static bool isInitialized = false;

        private static T _instance;

        public static T Instance {
            get {
                if (isQuitting) {
                    if (_instance is IAlwaysAccessibleOnQuit) {
                        return _instance;
                    }
                    else {
                        return null;
                    }
                }

                return _instance ?? CreateInstance();
            }
        }

        protected virtual void Awake() {
            if (_instance == null || _instance == this) {
                _instance = this as T;

                try {
                    if (!isInitialized) {
                        Initialization();
                    }
                }
                catch (Exception e) {
                    Prod.LogException(e);
                }
                finally {
                    isInitialized = true;
                }
            } else 
                Destroy(gameObject);
        }

        private void OnDestroy() {
            try {
                if (this == _instance) {
                    try {
                        Finalization();
                    }
                    catch (Exception e) {
                        Prod.LogException(e);
                    }
                    finally {
                        isInitialized = false;
                    }
                }
            }
            catch (Exception e) {
                Prod.LogException(e);
            }

            if (this == _instance)
                _instance = null;
        }

        protected virtual void OnApplicationQuit() {
            isQuitting = true;
        }

        protected virtual void Initialization() {}

        protected virtual void Finalization() {}

        private static T CreateInstance() {
            if (_instance == null) {
                try {
                    _instance = FindObjectOfType<T>() as T;
                }
                catch (Exception e) {
                    Prod.LogException(e);
                }
                finally {
                    if (Application.isPlaying) {
                        if (_instance == null) {
                            Prod.Print(AlertLevel.Notify, "Singleton", $"Creating {typeof(T).Name}");
                        }
                        else {
                            // кейс, когда мы запрашиваем GameObject, который реально уже существует на сцене, но для которого еще не был вызван Awake:
                            // нам придется его удалить, и создать новый с насильным запуском Awake, чтобы все процедуры отработали штатно прямо сейчас
                            _instance.gameObject.SetActive(false);
                            Destroy(_instance.gameObject);
                            _instance = null;

                            Prod.Print(AlertLevel.Warning, "Singleton", $"Force creating {typeof(T).Name}");
                        }

                        var gObj = (typeof(T).GetInterface(nameof(IKeepAliveMonoBehaviourSingleton)) != null)
                                       ? UtilsGameObject.GetPermanent(typeof(T).Name)
                                       : UtilsGameObject.GetTemp(typeof(T).Name);

                        // хукаемся так, чтобы m_instance заполнился ДО вызова Awake,
                        // тем самым делая неважным порядок вызова Awake в наследовании
                        gObj.SetActive(false);
                        _instance = gObj.AddComponent<T>();
                        gObj.SetActive(true);
                    }
                }
            }

            return _instance;
        }

        public static bool HasInstance => (!isQuitting || (_instance is IAlwaysAccessibleOnQuit)) && _instance != null;
    }
}