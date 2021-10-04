using MicroRace.States;
using RedDev.Kernel.Contexts;
using RedDev.Kernel.Managers;
using UnityEngine;

namespace MicroRace.Contexts {
    public class UIGameLoadingContext : BaseContext {
        private GlobalStatesManager _globalStates;
        private Animator animator;

        void Awake() {
            _globalStates = Core.Get<GlobalStatesManager>();
            animator = GetComponent<Animator>();
        }

        void Start() {
            
        }

        public void FadeOff() {

        }

        public void NextState() => (_globalStates.currentState as GameLoadingState)?.NextState();
    }
}