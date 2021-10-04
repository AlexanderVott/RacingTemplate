using RedDev.Kernel.Events;

namespace RedDev.Kernel.Managers {

    #region System events structs
    public class KernelStartEvent : BaseEventParam { }
    public class KernelStopEvent : BaseEventParam { }

    public class GameStartEvent : BaseEventParam { }
    public class GameStopEvent : BaseEventParam { }

    public class SettingsInputUpdateEvent : BaseEventParam { }

    public class GameSessionStartEvent : BaseEventParam { }
    public class GameSessionEndEvent : BaseEventParam { }
    #endregion

    /// <inheritdoc />
    public class SystemEventsManager : BaseGameEvents {
        private void Awake() {
            events.AddReceiverType<KernelStartEvent>()
                .AddReceiverType<KernelStopEvent>()
                .AddReceiverType<SettingsInputUpdateEvent>()
                .AddReceiverType<GameSessionStartEvent>()
                .AddReceiverType<GameSessionEndEvent>();

            //TODO: events.AttachReceivers();
        }

        private void OnDestroy() {
            KernelStop();
        }

        #region Events calls
        public void KernelStart() => Call(new KernelStartEvent());
        public void KernelStop() => Call(new KernelStopEvent());

        public void GameStart() => Call(new GameStartEvent());
        public void GameStop() => Call(new GameStopEvent());

        public void SettingsInputUpdate() => Call(new SettingsInputUpdateEvent());

        public void GameSessionStart() => Call(new GameSessionStartEvent());
        public void GameSessionEnd() {
            Call(new GameSessionEndEvent());
            Clear<GameSessionEndEvent>();
        }
        #endregion
    }
}