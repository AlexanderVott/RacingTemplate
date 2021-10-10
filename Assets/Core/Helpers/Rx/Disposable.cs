using System;

namespace RedDev.Helpers.Rx {
    public interface ICancelableDisposable : IDisposable {
        void Cancel();
    }

    public static class Disposable {
        public static readonly IDisposable Empty = EmptyDisposable.Singleton;

        public static IDisposable Create(Action disposeAction) {
            return new AnonymousDisposable(disposeAction);
        }

        public static ICancelableDisposable CreateCancelable(Action disposeAction) {
            return new CancelableDisposable(disposeAction);
        }

        public static IDisposable Create<TState>(TState state, Action<TState> disposeAction) {
            return new AnonymousDisposable<TState>(state, disposeAction);
        }

        public static ICancelableDisposable CreateCancelable<TState>(TState state, Action<TState> disposeAction) {
            return new CancelableDisposable<TState>(state, disposeAction);
        }

        private class EmptyDisposable : IDisposable {
            public static EmptyDisposable Singleton = new EmptyDisposable();

            private EmptyDisposable() { }

            public void Dispose() { }
        }

        private class AnonymousDisposable : IDisposable {
            protected bool IsDisposed = false;
            protected Action DisposeAction;

            public AnonymousDisposable(Action dispose) {
                this.DisposeAction = dispose;
            }

            public void Dispose() {
                if (!IsDisposed) {
                    IsDisposed = true;
                    DisposeAction();
                }
            }
        }

        private class CancelableDisposable : AnonymousDisposable, ICancelableDisposable {
            public CancelableDisposable(Action dispose) : base(dispose) {}

            public void Cancel() {
                IsDisposed = true;
                DisposeAction = null;
            }
        }

        private class AnonymousDisposable<T> : IDisposable {
            protected bool IsDisposed = false;
            protected T State;
            protected Action<T> DisposeAction;

            public AnonymousDisposable(T state, Action<T> dispose) {
                this.State = state;
                this.DisposeAction = dispose;
            }

            public void Dispose() {
                if (!IsDisposed) {
                    IsDisposed = true;
                    DisposeAction(State);
                }
            }
        }

        private class CancelableDisposable<T> : AnonymousDisposable<T>, ICancelableDisposable {
            public CancelableDisposable(T state, Action<T> dispose) : base(state, dispose) { }

            public void Cancel() {
                State = default;
                IsDisposed = true;
                DisposeAction = null;
            }
        }
    }
}