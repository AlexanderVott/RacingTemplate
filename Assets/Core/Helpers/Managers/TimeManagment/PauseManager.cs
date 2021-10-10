using System.Collections.Generic;
using UnityEngine;

namespace RedDev.Helpers.TimeManagement {
    public class PauseManager {
        private class PauseDescriptor {
            public int Generation = 0;
            public int RefCounter = 0;
        }

        public bool IsPaused => _pauseCaller.Count > 0;

        private int _lastGeneration;

        public int GetGeneration() {
            return _lastGeneration++;
        }

        private GameObject _defaultPauseHolder;

        public GameObject DefaultPauseHolder {
            get {
                if (_defaultPauseHolder == null) {
                    _defaultPauseHolder = UtilsGameObject.GetPermanent("DefaultPauseHolder");
                }

                return _defaultPauseHolder;
            }
        }

        private float _lastScale = 1f;
        private Dictionary<Object, PauseDescriptor> _pauseCaller = new Dictionary<Object, PauseDescriptor>();

        public void Pause(Object holder = null, int callGeneration = 0) {
            holder = holder == null ? DefaultPauseHolder : holder;
            if (!_pauseCaller.TryGetValue(holder, out var currentDescriptor)) {
                currentDescriptor = new PauseDescriptor();
                currentDescriptor.Generation = callGeneration;
                _pauseCaller[holder] = currentDescriptor;
            }

            if (currentDescriptor.Generation == callGeneration) {
                ++currentDescriptor.RefCounter;
            }

            if (currentDescriptor.Generation < callGeneration) {
                currentDescriptor.RefCounter = 1;
            }

            if (!IsPaused) {
                _lastScale = Time.timeScale;
            }

            Time.timeScale = 0f;
        }

        public void Reset() {
            _lastScale = 1f;
            _pauseCaller.Clear();
            Unpause();
        }

        public void Unpause(Object holder = null, int callGeneration = 0) {
            holder = holder == null ? DefaultPauseHolder : holder;
            if (_pauseCaller.TryGetValue(holder, out var currentDescriptor)) {
                if (currentDescriptor.Generation == callGeneration) {
                    --currentDescriptor.RefCounter;
                }

                var needRemove = (currentDescriptor.RefCounter <= 0) || (currentDescriptor.Generation < callGeneration);
                if (needRemove) {
                    _pauseCaller.Remove(holder);
                }
            }

            if (!IsPaused) {
                Time.timeScale = _lastScale;
                _lastScale = 1f;
            }
        }
    }
}