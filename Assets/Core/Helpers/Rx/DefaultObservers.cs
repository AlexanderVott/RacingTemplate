using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedDev.Helpers.Rx {
    public class DefaultObservers<K> {
        private List<PropertyObserver<K>> _observers = new List<PropertyObserver<K>>();

        public void Invoke(in K value) {
            var wasErrors = false;
            foreach (var obs in _observers) {
                try {
                    obs.Observer.OnNext(value);
                }
                catch (Exception e) {
                    Debug.LogException(e);
                    obs.Observer.OnError(e);
                    obs.Dead = true;
                    wasErrors = true;
                }
            }

            if (wasErrors) {
                _observers.RemoveAll(e => e.Dead);
            }
        }

        public IDisposable Subscribe(IObserver<K> observer, in K currentValue) {
            try {
                observer.OnNext(currentValue);
            }
            catch (Exception e) {
                Debug.LogException(e);
                observer.OnError(e);
                return Disposable.Empty;
            }

            var obs = new PropertyObserver<K>();
            obs.Observer = observer;
            obs.Disposable = Disposable.CreateCancelable(obs, Unsubscribe);
            _observers.Add(obs);
            return obs.Disposable;
        }

        private void Unsubscribe(PropertyObserver<K> observer) {
            _observers.Remove(observer);
        }

        private class PropertyObserver<Z> {
            internal IObserver<Z> Observer;
            internal ICancelableDisposable Disposable;
            internal bool Dead;
        }
    }
}