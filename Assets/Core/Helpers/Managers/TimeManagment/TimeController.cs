using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RedDev.Helpers.TimeManagement {
    
    public class TimeController : Singleton<TimeController>, IKeepAliveMonoBehaviourSingleton {
        public bool IsApplicationQuitting { private set; get; }

        public event Action<bool> OnApplicationPauseEvent;
        public event Action<bool> OnApplicationFocusEvent;
        public Action OnApplicationQuitEvent;

        public Action OnFixedUpdateEvent;
        public Action OnUpdateEvent;
        public Action OnLateUpdateEvent;

        public Action OnPerSecondUpdateEvent;
        private float _perSecondUpdateTime;

        public float InterpolationPhase => GetInterpolationPhase();
        private float _interpolationPhase = 0f;
        private float _lastFixedLiveCall = 0f;
        private int _lastFrame = 0;

        private List<IActionHolder> _callbacks = new List<IActionHolder>();
        private List<IActionHolder> _syncCallbacks = new List<IActionHolder>();
        private List<IEnumerator> _coroutinesToStart = new List<IEnumerator>();
        private List<IEnumerator> _coroutinesToStop = new List<IEnumerator>();
        private int _mainThreadId;

        protected override void Awake() {
            base.Awake();

            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public ActionHolder AddCallback(float delay, Action callback, bool realtime = true) {
            if (callback == null) {
                return null;
            }

            var actionHolder = new ActionHolder((realtime ? Time.realtimeSinceStartup : Time.time) + delay, callback, realtime);
            _callbacks.Add(actionHolder);
            return actionHolder;
        }

        public void RemoveCallback(ActionHolder callback) {
            _callbacks.Remove(callback);
        }

        public void CallFromMainThread(Action action) {
            if (Thread.CurrentThread.ManagedThreadId != _mainThreadId) {
                var newAH = new ActionHolder(0, action, true);
                lock (_syncCallbacks) {
                    _syncCallbacks.Add(newAH);
                }
            }
            else {
                action?.Invoke();
            }
        }

        public void CallFromMainThread<T>(Action<T> action, T param) {
            if (Thread.CurrentThread.ManagedThreadId != _mainThreadId) {
                var newAH = new ActionHolderWParam<T>(0, action, param, true);
                lock (_syncCallbacks) {
                    _syncCallbacks.Add(newAH);
                }
            }
            else {
                action?.Invoke(param);
            }
        }

        public void CallWithTimeout(Action<Action> actionWithCallback, float timeout, Action onDoneCallback, Action timeoutCallback) {
            bool fulfilled = false;
            bool timeouted = false;
            AddCallback(timeout, () => {
                timeouted = true;
                if (!fulfilled && timeoutCallback != null) {
                    timeoutCallback();
                }
            });

            Action onDone = () => {
                fulfilled = true;
                if (!timeouted && onDoneCallback != null) {
                    onDoneCallback();
                }
            };

            actionWithCallback(onDone);
        }

        public IEnumerator RunCoroutine(IEnumerator coroutine) {
            if (coroutine == null) {
                return null;
            }

            _coroutinesToStart.Add(coroutine);
            return coroutine;
        }

        public void RemoveCoroutine(IEnumerator coroutine) {
            if (coroutine == null) {
                return;
            }

            _coroutinesToStop.Add(coroutine);
        }

        public void RunInThreadPool<T>(Func<T> job, Action<T> callback) {
            if (job == null) {
                return;
            }

            WaitCallback threadJob = (obj) => {
                T result = job();
                CallFromMainThread(callback, result);
            };

            if (!ThreadPool.QueueUserWorkItem(threadJob)) {
                Prod.LogError("Could not queue thread pool job", this);
            }
        }

        private void FixedUpdate() {
            _lastFixedLiveCall = Time.timeSinceLevelLoad;
            OnFixedUpdateEvent?.Invoke();
        }

        private float GetInterpolationPhase() {
            if (Time.inFixedTimeStep) {
                return 1f;
            }

            if (Time.renderedFrameCount == _lastFrame) {
                return _interpolationPhase;
            }

            _lastFrame = Time.renderedFrameCount;
            if (_lastFixedLiveCall > Time.timeSinceLevelLoad) {
                _interpolationPhase = 0;
            }
            else {
                var delta = Time.timeSinceLevelLoad - _lastFixedLiveCall;
                _interpolationPhase = delta / Time.fixedDeltaTime;
            }

            return _interpolationPhase;
        }

        private void Update() {
            if (_syncCallbacks.Count > 0) {
                lock (_syncCallbacks) {
                    _callbacks.AddRange(_syncCallbacks);
                    _syncCallbacks.Clear();
                }
            }

            if (_callbacks.Count > 0) {
                int i = 0;
                while (i < _callbacks.Count) {
                    var callback = _callbacks[i];

                    if (!callback.IsReady()) {
                        ++i;
                    }
                    else {
                        _callbacks.Remove(callback);
                        callback.Call();
                    }
                }
            }

            if (_coroutinesToStart.Count > 0) {
                for (int i = 0; i < _coroutinesToStart.Count; i++) {
                    StartCoroutine(_coroutinesToStart[i]);
                }

                _coroutinesToStart.Clear();
            }

            if (_coroutinesToStop.Count > 0) {
                for (int i = 0; i < _coroutinesToStop.Count; i++) {
                    StopCoroutine(_coroutinesToStop[i]);
                }

                _coroutinesToStop.Clear();
            }

            var ut = Time.unscaledTime;
            if (ut > _perSecondUpdateTime) {
                OnPerSecondUpdateEvent?.Invoke();
                _perSecondUpdateTime = ut + 1f;
            }

            OnUpdateEvent?.Invoke();
        }

        private void LateUpdate() {
            OnLateUpdateEvent?.Invoke();
        }

        private void OnApplicationPause(bool pause) {
            OnApplicationPauseEvent?.Invoke(pause);
        }

        private void OnApplicationFocus(bool focus) {
            Prod.Log("Handle OnApplicationFocus: " + focus, this);
            OnApplicationFocusEvent?.Invoke(focus);
        }

        protected override void OnApplicationQuit() {
            Prod.Log("Handle OnApplicationQuit", this);
            IsApplicationQuitting = true;
            OnApplicationQuitEvent?.Invoke();
            base.OnApplicationQuit();
        }

        public static IEnumerator CallNextFrame(Action action) {
            yield return new WaitForEndOfFrame();

            action?.Invoke();
        }
    }

    public interface IActionHolder {
        bool IsReady();

        void Call();
    }

    public abstract class BaseActionHolder : IActionHolder {
        private readonly float _timeToExecute;
        private readonly bool _realtime;

        protected BaseActionHolder(float timeToExecute, bool realtime) {
            _timeToExecute = timeToExecute;
            _realtime = realtime;
        }

        public abstract void Call();

        public bool IsReady() {
            return _timeToExecute <= (_realtime ? Time.realtimeSinceStartup : Time.time);
        }
    }

    public class ActionHolder : BaseActionHolder {
        private readonly Action _action;

        public ActionHolder(float timeToExecute, Action action, bool realtime) : base(timeToExecute, realtime) {
            _action = action;
        }

        public override void Call() {
            if (_action == null) {
                return;
            }

            try {
                _action();
            }
            catch (Exception e) {
                Prod.LogError("Exception in delayed func: " + e.ToString(), this);
            }
        }
    }

    public class ActionHolderWParam<T> : BaseActionHolder {
        private readonly Action<T> _actionWithParam;
        private readonly T _param;

        public ActionHolderWParam(float timeToExecute, Action<T> action, T param, bool realtime) : base(timeToExecute, realtime) {
            _actionWithParam = action;
            _param = param;
        }

        public override void Call() {
            if (_actionWithParam == null) {
                return;
            }

            try {
                _actionWithParam(_param);
            }
            catch (Exception e) {
                Prod.LogError("Exception in delayed callback: " + e.ToString(), this);
            }
        }
    }
}