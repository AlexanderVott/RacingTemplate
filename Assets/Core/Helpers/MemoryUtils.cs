using System;
using System.Collections;
using RedDev.Helpers.TimeManagement;
using UnityEngine;

namespace RedDev.Helpers.Common {
    public static class MemoryUtils {
        public static void Clear(bool waitForNextFrame, bool aggressiveMode = false) {
            if (waitForNextFrame) {
                TimeController.Instance.RunCoroutine(ClearCoroutine(aggressiveMode));
            }
            else {
                ClearAll(aggressiveMode);
            }
        }

        private static IEnumerator ClearCoroutine(bool aggressiveMode) {
            yield return null;

            ClearAll(aggressiveMode);
        }

        private static void ClearAll(bool aggressiveMode) {
            if (aggressiveMode) {
                Resources.UnloadUnusedAssets();
            }
            GC.Collect();

            if (aggressiveMode) {
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}