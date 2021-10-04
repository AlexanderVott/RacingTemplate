using RedDev.Kernel.Managers;
using RedDev.Kernel.States;
using UnityEngine;

namespace RedDev.Kernel.Contexts {
    public class UIStartLoadingContext : BaseContext {
        private GlobalStatesManager _globalStates;

        private Animator _animator;

        void Awake() {
            this.BuildUpDI();
            _globalStates = Core.Get<GlobalStatesManager>();
            _animator = GetComponent<Animator>();
#if UNITY_EDITOR
            if (_animator != null)
                _animator.speed = 100f;
#endif
        }

        public void NextState() 
            => (_globalStates.currentState as StartLoadingState)?.NextState();
    }
}