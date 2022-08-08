using Game.Contexts;
using Game.DB;
using Game.Managers;
using RedDev.Kernel.Events;
using RedDev.Kernel.FMS.Globals;
using RedDev.Kernel.Managers;
using UnityEngine;

namespace Game.States {
    public class GameLoadingState : MonoBehaviour{// GlobalBaseState, IReceiverEvent<KernelStartEvent> {
        [SerializeField]
        private GlobalBaseState nextState = null;

        /*public override void OnInitialize() {
            base.OnInitialize();

            contextManager?.Preload<UIGameLoadingContext>();
        }

        public void HandleEvent(KernelStartEvent arg) {
            Dev.Log("yahoo");

            LoadingDB();
            LoadingPrefs();
            InitializeSubSystems();

            NextState();
        }

        private void InitializeSubSystems() {
            Core.Add<VehicleVisualManager>();
            Core.Add<VehiclesManager>();
            Core.Add<CamerasManager>();
            //TODO: TrackConfigManager
            Core.AttachManagers();
        }

        private void LoadingPrefs() {
            var prefs = Core.Get<PrefsManager>();
            if (prefs == null) {
                Dev.LogError("[GameLoadingState] Fail to get prefs manager");
                return;
            }
            //prefs.Load<MissionsPrefsModel>();
        }

        private void LoadingDB() {
            var dbMan = Core.Get<DBManager>();

            dbMan.Load<VehiclesDBMeta>();
            dbMan.Load<CustomizeDBMeta>();
            dbMan.Load<EnginesDBMeta>();
            dbMan.Load<SteeringDBMeta>();
            dbMan.Load<TransmissionsDBMeta>();
            dbMan.Load<BrakesDBMeta>();
            dbMan.Load<AxlesDBMeta>();
            dbMan.Load<WheelsDBMeta>();
            dbMan.Load<VehicleSurfaceDB>();
        }

        public override void OnEnter() {
            base.OnEnter();
            contextManager?.Push<UIGameLoadingContext>();
            var events = Core.Get<SystemEventsManager>();
            events.AddAutoCallArg(new KernelStartEvent());
            events.Subscribe(this, SubscribeType.AutoOneCall);
        }

        public override void OnExit() {
            contextManager?.Pop<UIGameLoadingContext>();
            base.OnExit();
        }

        public override void OnPause() {
            contextManager?.Pop<UIGameLoadingContext>();
            base.OnPause();
        }

        public override void OnResume() {
            base.OnResume();
            contextManager?.Push<UIGameLoadingContext>();
        }

        public void NextState() {
            if (nextState != null)
                globalStates?.ChangeState(nextState.GetType());
            else
                Prod.LogError("[GameLoadingState] Next state after loading is null!");
        }*/
    }
}