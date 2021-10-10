using System;
using System.Collections.Generic;
using RedDev.Helpers.ColorExtensions;
using RedDev.Kernel.Managers;

public enum AlertLevel : byte {
    Notify,
    Warning,
    Error,
    FatalError,
    Silence
}

public enum CheckLevel : byte {
    Any,
    Success,
    Fail,
    Silence
}

namespace UnityEngine {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System.Text.RegularExpressions;

    public static class Dev {
        public static void Log(object message) => Debug.Log(StripTagsInBuild(message));
        public static void Log(object message, Object context) => Debug.Log(StripTagsInBuild(message), context);
        public static void LogAssertion(object message, Object context) => Debug.LogAssertion(StripTagsInBuild(message), context);
        public static void LogAssertion(object message) => Debug.LogAssertion(StripTagsInBuild(message));
        public static void LogAssertionFormat(Object context, string format, params object[] args) => Debug.LogAssertionFormat(context, format, args);
        public static void LogAssertionFormat(string format, params object[] args) => Debug.LogAssertionFormat(format, args);
        public static void LogError(object message, Object context) => Debug.LogError(StripTagsInBuild(message), context);
        public static void LogError(object message) => Debug.LogError(StripTagsInBuild(message));
        public static void LogErrorFormat(Object context, string format, params object[] args) => Debug.LogErrorFormat(context, format, args);
        public static void LogErrorFormat(string format, params object[] args) => Debug.LogErrorFormat(format, args);
        public static void LogFormat(string format, params object[] args) => Debug.LogFormat(format, args);
        public static void LogFormat(Object context, string format, params object[] args) => Debug.LogFormat(context, format, args);

        public static void LogFormat(LogType logType, LogOption logOptions, Object context, string format, params object[] args) =>
            Debug.LogFormat(logType, logOptions, context, format, args);

        public static void LogWarning(object message, Object context) => Debug.LogWarning(StripTagsInBuild(message), context);
        public static void LogWarning(object message) => Debug.LogWarning(StripTagsInBuild(message));
        public static void LogWarningFormat(Object context, string format, params object[] args) => Debug.LogWarningFormat(context, format, args);
        public static void LogWarningFormat(string format, params object[] args) => Debug.LogWarningFormat(format, args);

        private static readonly Regex _tagsRegex = new Regex("<.*?>", RegexOptions.Compiled);

        private static object StripTagsInBuild(object message) {
            if (message is string str && Application.isEditor == false && str.Contains("<")) {
                return _tagsRegex.Replace(str, string.Empty);
            }

            return message;
        }
    }
#else
    using System.Diagnostics;
    public static class Dev {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void Log(object message) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void Log(object message, Object context) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogAssertion(object message, Object context) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogAssertion(object message) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogAssertionFormat(Object context, string format, params object[] args) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogAssertionFormat(string format, params object[] args) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogError(object message, Object context) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogError(object message) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogErrorFormat(Object context, string format, params object[] args) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogErrorFormat(string format, params object[] args) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogFormat(string format, params object[] args) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogFormat(Object context, string format, params object[] args) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogFormat(LogType logType, LogOption logOptions, Object context, string format, params object[] args) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogWarning(object message, Object context) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogWarning(object message) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogWarningFormat(Object context, string format, params object[] args) { }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")] public static void LogWarningFormat(string format, params object[] args) { }
    }
#endif

    public interface __ICustomLog {
        void Log(object msg, object parent = null);

        void LogWarning(object msg, object parent = null);

        void LogError(object msg, object parent = null);

        void LogException(Exception e);

        void SetupGroup(string name, AlertLevel alertLevel, CheckLevel checkLevel, Color? color = null);

        void Print(AlertLevel alertLevel, string groupName, string text, UnityEngine.Object obj = null);

        void Check(AlertLevel alertLevel, string groupName, string text, bool val, UnityEngine.Object obj = null);
    }

    public static class Prod {
        private static __ICustomLog _internalInstance;

        public static __ICustomLog Instance {
            get {
                if (_internalInstance == null) {
                    _internalInstance = new __DefaultLog();
                    (_internalInstance as __DefaultLog).Initialize();
                }

                return _internalInstance;
            }
        }

        private static string ConstructOutput(object msg, object parent) {
            var parentName = "Unknown";
            if (parent != null) {
                if (parent is string str) {
                    parentName = str;
                }
                else {
                    parentName = parent.GetType().ToString();
                }

                return $"{parentName} -> {msg}";
            }

            return $"{msg}";
        }

        public static void Log(object msg, object parent = null) {
            Instance.Log(ConstructOutput(msg, parent), parent);
        }

        public static void LogWarning(object msg, object parent = null) {
            Instance.LogWarning(ConstructOutput(msg, parent), parent);
        }

        public static void LogError(object msg, object parent = null) {
            Instance.LogError(ConstructOutput(msg, parent), parent);
        }

        public static void LogException(Exception e) {
            Instance.LogException(e);
        }

        public static void SetupGroup(string name, AlertLevel alertLevel, CheckLevel checkLevel, Color? color = null) {
            Instance.SetupGroup(name, alertLevel, checkLevel, color);
        }

        public static void Print(AlertLevel alertLevel, string groupName, string text, UnityEngine.Object obj = null) {
            Instance.Print(alertLevel, groupName, text, obj);
        }

        public static void Check(AlertLevel alertLevel, string groupName, string text, bool val, UnityEngine.Object obj = null) {
            Instance.Check(alertLevel, groupName, text, val, obj);
        }
    }

    #region Default Logger

    public class __DefaultLog : __ICustomLog {
        public class LogGroupSettings {
            public AlertLevel AlertLevel;
            public CheckLevel CheckLevel;
            public string HexColor = "";
        }

        public __DefaultLog() {
            SetupGroup("Singleton", AlertLevel.Warning, CheckLevel.Any, Color.black);
            SetupGroup("System", AlertLevel.Notify, CheckLevel.Any, Color.black);
            SetupGroup("Unity", AlertLevel.Notify, CheckLevel.Any, new Color(0f, 0.4f, 0.6f));
            if (!Application.isPlaying) {
                return;
            }

            try {
                Print(AlertLevel.Notify, "System", "Log started: " + DateTime.Now);
                Print(AlertLevel.Notify, "System", "Launch params: " + Environment.CommandLine);
                AddTextDelimiter();
            }
            catch { }
        }

        public void Initialize() {
            Core.Instance.OnApplicationQuitEvent -= Close;
            Core.Instance.OnApplicationQuitEvent += Close;
        }

        public void ShowGroupStates() {
            SendMessageToUnityLog(AlertLevel.Notify, "Group states:", null);
            foreach (var item in m_groupList) {
                SendMessageToUnityLog(AlertLevel.Notify,
	                $"Group:'{item.Key,20}'       Alert:{item.Value.AlertLevel,10}       Check:{item.Value.CheckLevel,10}",
                    null);
            }
        }

        private void Close() {
            AddTextDelimiter();
            Print(AlertLevel.Notify, "System", "Log ended: " + DateTime.Now);
        }

        public void AddTextDelimiter() {
            var message = "------------------------------------------------------------";
            SendMessageToUnityLog(AlertLevel.Notify, message, null);
        }

        public AlertLevel GetAlertByName(string name) {
            switch (name) {
                case "notify":
                    return AlertLevel.Notify;
                case "warning":
                    return AlertLevel.Warning;
                case "error":
                    return AlertLevel.Error;
                case "fatalerror":
                    return AlertLevel.FatalError;
                default:
                    return AlertLevel.Silence;
            }
        }

        public CheckLevel GetCheckByName(string name) {
            switch (name) {
                case "any":
                    return CheckLevel.Any;
                case "fail":
                    return CheckLevel.Fail;
                case "success":
                    return CheckLevel.Success;
                default:
                    return CheckLevel.Silence;
            }
        }

        public LogGroupSettings Group(string name) {
            name = name.ToLower();
            LogGroupSettings group;
            if (!m_groupList.TryGetValue(name, out group)) {
                group = RegisterGroup(name, true);
            }

            return group;
        }

        private readonly string m_defaultHexColor = Color.black.ToHexString();

        public void SetupGroup(string name, AlertLevel alertLevel, CheckLevel checkLevel, Color? color = null) {
            LogGroupSettings group = Group(name);

            group.AlertLevel = alertLevel;
            group.CheckLevel = checkLevel;
            if (color.HasValue) {
                group.HexColor = color.Value.ToHexString();
            }
            else {
                group.HexColor = m_defaultHexColor;
            }
        }

        public void Print(AlertLevel alertLevel, string groupName, string text, UnityEngine.Object obj = null) {
            var group = Group(groupName);

            if (group.AlertLevel == AlertLevel.Silence) {
                return;
            }

            if (group.AlertLevel <= alertLevel) {
                var message = Application.isEditor ? $"<color={group.HexColor}>{groupName}> </color>{text}" : $"{groupName}> {text}";

                SendMessageToUnityLog(alertLevel, message, obj);
            }
        }

        public void Check(AlertLevel alertLevel, string groupName, string text, bool val, UnityEngine.Object obj = null) {
            var group = Group(groupName);

            if ((group.AlertLevel == AlertLevel.Silence) || (group.CheckLevel == CheckLevel.Silence)) {
                return;
            }

            var allow = false;
            if (group.AlertLevel <= alertLevel) {
                allow = (group.CheckLevel == CheckLevel.Any) || (val && (group.CheckLevel == CheckLevel.Success)) ||
                        (!val && (group.CheckLevel == CheckLevel.Fail));
            }

            if (allow) {
                var message = Application.isEditor
                                  ? $"<color={group.HexColor}>{groupName}> </color>{text}{(val ? "...OK" : "...FAIL")}"
                                  : $"{groupName}> {text}{(val ? "...OK" : "...FAIL")}";
                SendMessageToUnityLog(alertLevel, message, obj);
            }
        }

        private Dictionary<string, LogGroupSettings> m_groupList = new Dictionary<string, LogGroupSettings>();

        private static void SendMessageToUnityLog(AlertLevel alertLevel, string message, UnityEngine.Object obj) {
            switch (alertLevel) {
                case AlertLevel.Notify:
                    Debug.Log(message, obj);
                    break;

                case AlertLevel.Warning:
                    Debug.LogWarning(message, obj);
                    break;

                case AlertLevel.Error:
                    Debug.LogError(message, obj);
                    break;

                case AlertLevel.FatalError:
                    Debug.LogError(message, obj);
                    break;

                default:
                    Debug.Log(message, obj);
                    break;
            }
        }

        private LogGroupSettings RegisterGroup(string name, bool silent = false) {
            var group = new LogGroupSettings();
            group.AlertLevel = AlertLevel.Notify;
            group.CheckLevel = CheckLevel.Fail;
            m_groupList.Add(name, group);
            if (!silent) {
                Print(AlertLevel.Warning, "System", "New group '" + name + "' registered.");
            }

            return group;
        }

        public void Log(object msg, object parent = null) {
            Print(AlertLevel.Notify, "Unity", msg.ToString(), parent as UnityEngine.Object);
        }

        public void LogWarning(object msg, object parent = null) {
            Print(AlertLevel.Warning, "Unity", msg.ToString(), parent as UnityEngine.Object);
        }

        public void LogError(object msg, object parent = null) {
            Print(AlertLevel.Error, "Unity", msg.ToString(), parent as UnityEngine.Object);
        }

        public void LogException(Exception e) {
            Debug.LogException(e);
        }
    }

    public static class HELPER_GameLog {
        public static AlertLevel ToAlertLevel(this LogType logType) {
            switch (logType) {
                case LogType.Log:
                    return AlertLevel.Notify;
                case LogType.Warning:
                    return AlertLevel.Warning;
                case LogType.Error:
                    return AlertLevel.Error;
                case LogType.Exception:
                    return AlertLevel.FatalError;
                case LogType.Assert:
                    return AlertLevel.Error;
                default:
                    return AlertLevel.Silence;
            }
        }

        public static string GetName(this AlertLevel level) {
            switch (level) {
                case AlertLevel.Notify:
                    return "[Notify]    : ";
                case AlertLevel.Warning:
                    return "[Warning]   : ";
                case AlertLevel.Error:
                    return "[Error]     : ";
                case AlertLevel.FatalError:
                    return "[FatalError]: ";
                default:
                    return "[Silence]   : ";
            }
        }

        public static string GetName(this CheckLevel level) {
            switch (level) {
                case CheckLevel.Any:
                    return "<Any>";
                case CheckLevel.Fail:
                    return "<Fail>";
                case CheckLevel.Success:
                    return "<Success>";
                default:
                    return "<Silence>";
            }
        }
    }

    #endregion Default Logger
}