using System;
using RedDev.Helpers.Extensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace RedDev.Helpers.PhaseTools {
    public static class Phase {
        public static float Clamp(double phase, bool forward) {
            return Clamp((float) phase, forward);
        }

        public static float Clamp(float phase, bool forward) {
            var result = Mathf.Clamp01(phase);
            return forward ? result : 1f - result;
        }

        public static float GetSmoothed(float phase) {
            return phase * phase * (3f - 2f * phase);
        }

        public static bool IsFinished(float phase, bool forward) {
            return (phase == 1f && forward) || (phase == 0f && !forward);
        }

        public static T Map<T>(ref float phase) where T : unmanaged, Enum {
            Split(phase, EnumTools<T>.count, out phase, out var intEnum);
            Assert.IsTrue(Enum.IsDefined(typeof(T), intEnum), $"Could not map {intEnum} to {typeof(T).Name}");
            return EnumTools<T>.From(intEnum);
        }

        public static void Split(float phase, int count, out float subPhase, out int subPhaseNum) {
            var step = 1f / count;
            subPhaseNum = (int) Math.Truncate(phase / step);
            if (subPhaseNum >= count) {
                subPhaseNum = count - 1;
                subPhase = 1f;
            }
            else {
                subPhase = (phase % step) / step;
            }
        }

        public static void Split<T>(float phase, int count, out float subPhase, out T subPhaseEnum) where T : unmanaged, Enum {
            Split(phase, count, out subPhase, out var intEnum);
            Assert.IsTrue(Enum.IsDefined(typeof(T), intEnum), $"Could not map {intEnum} to {nameof(T)}");
            subPhaseEnum = EnumTools<T>.From(intEnum);
        }

        public static float SubPhase(float phase, float start, float end) {
            return Mathf.Clamp01((phase - start) / (end - start));
        }
    }
}