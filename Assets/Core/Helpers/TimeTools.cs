using System;
using System.Collections.Generic;
using System.Diagnostics;
using RedDev.Helpers.Common;
using RedDev.Helpers.Extensions;
using RedDev.Helpers.Rx;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;

namespace RedDev.Helpers {
    using TimeEvents;

    public class TimeTools {
        private const double INITIAL_DOUBLE_TIME_POINT = 5000000000d;

        private class UpdateCall { };

        private class FixedUpdateCall { };

        private static long _pausedTimeMs;

        public static long InitialTimeMs;
        public static long PrevTimeMs;
        public static long LongTimeMs;
        public static long LongTimeS;
        public static double DoubleTimeS;
        public static long DeltaTimeMs;

        public static long LongTimeMs_Scaled;
        public static long LongTimeS_Scaled;
        public static double DoubleTimeS_Scaled;
        public static long TimeTicks;

        public static long FixedTimeMs;
        public static long FixedTimeS;
        public static double FixedDoubleTimeS;
        public static long FixedTicks;

        public long TimeMs => LongTimeMs_Scaled;
        public long TimeS => LongTimeS_Scaled;
        public double DoubleTime => DoubleTimeS_Scaled;

        public long RealTimeMs => GetUptimeInMS() - InitialTimeMs;
        public long RealTimeS => (GetUptimeInMS() - InitialTimeMs) / 1000;
        public double RealTime => INITIAL_DOUBLE_TIME_POINT + ((GetUptimeInMS() - InitialTimeMs) / 1000d);
        public static double StaticRealTime => INITIAL_DOUBLE_TIME_POINT + ((GetUptimeInMS() - InitialTimeMs) / 1000d);

        public long UnscaledTimeMs => LongTimeMs;
        public long UnscaledTimeS => LongTimeS;
        public double UnscaledTime => DoubleTimeS;
        public long Frames => TimeTicks;

        public long ModelTimeMs => FixedTimeMs;
        public long ModelTimeS => FixedTimeS;
        public double ModelTime => FixedDoubleTimeS;
        public long ModelFrames => FixedTicks;

        public float GetAngledTime(float speed) {
            return (float) System.Math.Sin(DoubleTimeS * speed);
        }

        public void ResetModel() {
            FixedTicks = 0;
        }

        private static void TimerFixedUpdate() {
            ++FixedTicks;
            FixedTimeMs += Mathf.RoundToInt(UnityEngine.Time.fixedDeltaTime * 1000);
            FixedTimeS = FixedTimeMs / 1000;
            FixedDoubleTimeS = FixedTimeMs / 1000d;

            for (int i = FixedTimeEvent.UpdateEvents.Count - 1; i >= 0; --i) {
                FixedTimeEvent.UpdateEvents[i]();
            }
        }

        private static void TimerUpdate() {
            ++TimeTicks;
            PrevTimeMs = LongTimeMs;
            LongTimeMs = GetUptimeInMS() - InitialTimeMs - _pausedTimeMs;
            DeltaTimeMs = LongTimeMs - PrevTimeMs;

#if UNITY_EDITOR
            if (EditorApplication.isPaused) {
                var overhead = DeltaTimeMs - 20;
                _pausedTimeMs += overhead;
                DeltaTimeMs = 20;

                PrevTimeMs -= overhead;
                LongTimeMs -= overhead;
            }
#endif

            LongTimeMs_Scaled += (Time.timeScale == 1f) ? DeltaTimeMs : Mathf.RoundToInt(Time.timeScale * DeltaTimeMs);
            LongTimeS_Scaled = LongTimeMs_Scaled / 1000;
            DoubleTimeS_Scaled = INITIAL_DOUBLE_TIME_POINT + (LongTimeMs_Scaled / 1000d);

            LongTimeS = LongTimeMs / 1000;
            DoubleTimeS = INITIAL_DOUBLE_TIME_POINT + (LongTimeMs / 1000d);

            for (int i = TimeEvent.UpdateEvents.Count - 1; i >= 0; --i) {
                TimeEvent.UpdateEvents[i]();
            }

            for (int i = PhasedTimeEvent.UpdateEvents.Count - 1; i >= 0; --i) {
                PhasedTimeEvent.UpdateEvents[i]();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterCoreTimers() {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopUtils.SetBefore<UnityEngine.PlayerLoop.Update.ScriptRunBehaviourUpdate>(ref playerLoop, TimerUpdate, typeof(TimeTools.UpdateCall));
            PlayerLoopUtils.SetBefore<UnityEngine.PlayerLoop.FixedUpdate.ScriptRunBehaviourFixedUpdate>(ref playerLoop, TimerFixedUpdate,
                typeof(TimeTools.FixedUpdateCall));
            PlayerLoop.SetPlayerLoop(playerLoop);

#if UNITY_ANDROID && !UNITY_EDITOR
            m_SystemClock = new AndroidJavaClass("android.os.SystemClock");
#endif

            PrevTimeMs = GetUptimeInMS();
            LongTimeMs = PrevTimeMs;
            InitialTimeMs = PrevTimeMs;
            _pausedTimeMs = 0;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaClass m_SystemClock;

        public static long GetUptimeInMS()
        {
            return m_SystemClock.CallStatic<long>("elapsedRealtime");
        }
#else
        public static long GetUptimeInMS() {
            return Stopwatch.GetTimestamp() / 10000;
        }
#endif

        public TimeEvent GetEventTimeTracker(int seconds) {
            return new TimeEvent(seconds);
        }

        public TimeEvent GetEventTimeTracker(float seconds) {
            return new TimeEvent(seconds);
        }

        public PhasedTimeEvent GetPhasedTimeTracker(int seconds) {
            return new PhasedTimeEvent(seconds);
        }

        public PhasedTimeEvent GetPhasedTimeTracker(float seconds) {
            return new PhasedTimeEvent(seconds);
        }        
    }
}

namespace RedDev.Helpers.TimeEvents {
    public interface ICompletable {
        bool IsCompleted { get; }
    }

    public class TimeEventJob : ICompletable {
        internal IDisposable DoJob(List<Action> list, Action onTick, Action onDone) {
            ICancelableDisposable cancelable = null;
            Action job = null;

            job = () => {
                onTick.SafeCall();
                if (IsCompleted) {
                    cancelable.Cancel();
                    list.Remove(job);
                    onDone.SafeCall();
                }
            };

            list.Add(job);
            cancelable = Disposable.CreateCancelable(() => list.Remove(job));

            return cancelable;
        }

        public virtual bool IsCompleted => true;
    }

    public class TimeEvent : TimeEventJob {
        internal static List<Action> UpdateEvents = new List<Action>();

        private long _finishTimeMs;

        public TimeEvent(int seconds) {
            Reset(seconds);
        }

        public TimeEvent(float seconds) {
            Reset(seconds);
        }

        public void Reset(int seconds) {
            _finishTimeMs = TimeTools.LongTimeMs + seconds * 1000;
        }

        public void Reset(float seconds) {
            _finishTimeMs = TimeTools.LongTimeMs + Mathf.RoundToInt(seconds * 1000f);
        }

        public IDisposable DoJob(Action onTick, Action onDone) => DoJob(UpdateEvents, onTick, onDone);
        public override bool IsCompleted => TimeTools.LongTimeMs > _finishTimeMs;
    }

    public class FixedTimeEvent : TimeEventJob {
        internal static List<Action> UpdateEvents = new List<Action>();

        private long _finishTimeMs;

        public FixedTimeEvent(int seconds) {
            Reset(seconds);
        }

        public FixedTimeEvent(float seconds) {
            Reset(seconds);
        }

        public void Reset(int seconds) {
            _finishTimeMs = TimeTools.FixedTimeMs + seconds * 1000;
        }

        public void Reset(float seconds) {
            _finishTimeMs = TimeTools.FixedTimeMs + Mathf.RoundToInt(seconds * 1000f);
        }
        public IDisposable DoJob(Action onTick, Action onDone) => DoJob(UpdateEvents, onTick, onDone);
        public override bool IsCompleted => TimeTools.FixedTimeMs > _finishTimeMs;
    }

    public class PhasedTimeEvent : TimeEventJob {
        internal static List<Action> UpdateEvents = new List<Action>();

        private long _initialTimeMs;
        private long _finishTimeMs;

        public PhasedTimeEvent(int seconds) {
            Reset(seconds);
        }

        public PhasedTimeEvent(float seconds) {
            Reset(seconds);
        }

        public void Reset(int seconds) {
            _initialTimeMs = TimeTools.LongTimeMs;
            _finishTimeMs = TimeTools.LongTimeMs + seconds * 1000;
        }

        public void Reset(float seconds) {
            _initialTimeMs = TimeTools.LongTimeMs;
            _finishTimeMs = TimeTools.LongTimeMs + Mathf.RoundToInt(seconds * 1000f);
        }

        public IDisposable DoJob(Action<float> onTick, Action onDone) => DoJob(UpdateEvents, () => onTick.SafeCall(Phase), onDone);
        public override bool IsCompleted => TimeTools.LongTimeMs > _finishTimeMs;

        public float Phase {
            get {
                if (IsCompleted) {
                    return 1f;
                }

                if (_finishTimeMs == _initialTimeMs) {
                    return 1f;
                }

                return (float)(TimeTools.LongTimeMs - _initialTimeMs) / (float)(_finishTimeMs - _initialTimeMs);
            }
        }
    }
}