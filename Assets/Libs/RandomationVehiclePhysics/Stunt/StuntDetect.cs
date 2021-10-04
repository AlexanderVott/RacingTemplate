using UnityEngine;
using System.Collections.Generic;

namespace RVP {
    [RequireComponent(typeof(VehicleParent))]
    [DisallowMultipleComponent]
    [AddComponentMenu("RVP/Stunt/Stunt Detector", 1)]

    // Class for detecting stunts
    public class StuntDetect : MonoBehaviour {
        private Transform tr;
        private Rigidbody rb;
        private VehicleParent vp;

        [System.NonSerialized] public float score;
        private List<Stunt> stunts = new List<Stunt>();
        private List<Stunt> doneStunts = new List<Stunt>();
        private bool drifting;
        private float driftDist;
        private float driftScore;
        private float endDriftTime; // Time during which drifting counts even if the vehicle is not actually drifting
        private float jumpDist;
        private float jumpTime;
        private Vector3 jumpStart;

        public bool detectDrift = true;
        public bool detectJump = true;
        public bool detectFlips = true;

        private string driftString;                       // String indicating drift distance
        private string jumpString;                        // String indicating jump distance
        private string flipString;                        // String indicating flips
        [System.NonSerialized] public string stuntString; // String containing all stunts

        public Motor engine;

        private void Start() {
            tr = transform;
            rb = GetComponent<Rigidbody>();
            vp = GetComponent<VehicleParent>();
        }

        private void FixedUpdate() {
            // Detect drifts
            if (detectDrift && !vp.crashing) {
                DetectDrift();
            }
            else {
                drifting = false;
                driftDist = 0;
                driftScore = 0;
                driftString = "";
            }

            // Detect jumps
            if (detectJump && !vp.crashing) {
                DetectJump();
            }
            else {
                jumpTime = 0;
                jumpDist = 0;
                jumpString = "";
            }

            // Detect flips
            if (detectFlips && !vp.crashing) {
                DetectFlips();
            }
            else {
                stunts.Clear();
                flipString = "";
            }

            // Combine strings into final stunt string
            stuntString = vp.crashing
                              ? "Crashed"
                              : driftString + jumpString +
                                (string.IsNullOrEmpty(flipString) || string.IsNullOrEmpty(jumpString) ? "" : " + ") +
                                flipString;
        }

        // Logic for detecting and tracking drift
        private void DetectDrift() {
            endDriftTime = vp.groundedWheels > 0
                               ? Mathf.Abs(vp.localVelocity.x) > 5 ? StuntManager.driftConnectDelayStatic :
                                                                     Mathf.Max(0,
                                                                         endDriftTime - Time.timeScale *
                                                                         TimeMaster.inverseFixedTimeFactor)
                               : 0;
            drifting = endDriftTime > 0;

            if (drifting) {
                driftScore += StuntManager.driftScoreRateStatic * Mathf.Abs(vp.localVelocity.x) * Time.timeScale *
                              TimeMaster.inverseFixedTimeFactor;
                driftDist += vp.velMag * Time.fixedDeltaTime;
                driftString = "Drift: " + driftDist.ToString("n0") + " m";

                if (engine)
                    engine.boost += StuntManager.driftBoostAddStatic * Mathf.Abs(vp.localVelocity.x) * Time.timeScale *
                                    0.0002f * TimeMaster.inverseFixedTimeFactor;
            }
            else {
                score += driftScore;
                driftDist = 0;
                driftScore = 0;
                driftString = "";
            }
        }

        // Logic for detecting and tracking jumps
        private void DetectJump() {
            if (vp.groundedWheels == 0) {
                jumpDist = Vector3.Distance(jumpStart, tr.position);
                jumpTime += Time.fixedDeltaTime;
                jumpString = "Jump: " + jumpDist.ToString("n0") + " m";

                if (engine)
                    engine.boost += StuntManager.jumpBoostAddStatic * Time.timeScale * 0.01f *
                                    TimeMaster.inverseFixedTimeFactor;
            }
            else {
                score += (jumpDist + jumpTime) * StuntManager.jumpScoreRateStatic;

                if (engine)
                    engine.boost += (jumpDist + jumpTime) * StuntManager.jumpBoostAddStatic * Time.timeScale * 0.01f *
                                    TimeMaster.inverseFixedTimeFactor;

                jumpStart = tr.position;
                jumpDist = 0;
                jumpTime = 0;
                jumpString = "";
            }
        }

        // Logic for detecting and tracking flips
        private void DetectFlips() {
            if (vp.groundedWheels == 0) {
                // Check to see if vehicle is performing a stunt and add it to the stunts list
                foreach (var curStunt in StuntManager.stuntsStatic) {
                    if (Vector3.Dot(vp.localAngularVel.normalized, curStunt.rotationAxis) >= curStunt.precision) {
                        var stuntExists = false;

                        foreach (var checkStunt in stunts) {
                            if (curStunt.name == checkStunt.name) {
                                stuntExists = true;
                                break;
                            }
                        }

                        if (!stuntExists)
                            stunts.Add(new Stunt(curStunt));
                    }
                }

                // Check the progress of stunts and compile the flip string listing all stunts
                foreach (var curStunt2 in stunts) {
                    if (Vector3.Dot(vp.localAngularVel.normalized, curStunt2.rotationAxis) >= curStunt2.precision)
                        curStunt2.progress += rb.angularVelocity.magnitude * Time.fixedDeltaTime;

                    if (curStunt2.progress * Mathf.Rad2Deg >= curStunt2.angleThreshold) {
                        var stuntDoneExists = false;

                        foreach (var curDoneStunt in doneStunts) {
                            if (curDoneStunt == curStunt2) {
                                stuntDoneExists = true;
                                break;
                            }
                        }

                        if (!stuntDoneExists)
                            doneStunts.Add(curStunt2);
                    }
                }

                var stuntCount = "";
                flipString = "";

                foreach (var curDoneStunt2 in doneStunts) {
                    stuntCount = curDoneStunt2.progress * Mathf.Rad2Deg >= curDoneStunt2.angleThreshold * 2
                                     ? " x" + Mathf
                                           .FloorToInt(curDoneStunt2.progress * Mathf.Rad2Deg /
                                                       curDoneStunt2.angleThreshold).ToString()
                                     : "";
                    flipString = string.IsNullOrEmpty(flipString)
                                     ? curDoneStunt2.name + stuntCount
                                     : flipString + " + " + curDoneStunt2.name + stuntCount;
                }
            }
            else {
                // Add stunt points to the score
                foreach (var curStunt in stunts) {
                    score += curStunt.progress * Mathf.Rad2Deg * curStunt.scoreRate *
                             Mathf.FloorToInt(curStunt.progress * Mathf.Rad2Deg / curStunt.angleThreshold) *
                             curStunt.multiplier;

                    // Add boost to the engine
                    if (engine)
                        engine.boost += curStunt.progress * Mathf.Rad2Deg * curStunt.boostAdd * curStunt.multiplier *
                                        0.01f;
                }

                stunts.Clear();
                doneStunts.Clear();
                flipString = "";
            }
        }
    }
}