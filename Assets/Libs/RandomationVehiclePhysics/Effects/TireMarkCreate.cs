using UnityEngine;

namespace RVP {
    [RequireComponent(typeof(Wheel))]
    [DisallowMultipleComponent]
    [AddComponentMenu("RVP/Effects/Tire Mark Creator", 0)]

    // Class for creating tire marks
    public class TireMarkCreate : MonoBehaviour {
        private Transform tr;
        private Wheel w;
        private Mesh mesh;
        private int[] tris;
        private Vector3[] verts;
        private Vector2[] uvs;
        private Color[] colors;

        private Vector3 leftPoint;
        private Vector3 rightPoint;
        private Vector3 leftPointPrev;
        private Vector3 rightPointPrev;

        private bool creatingMark;
        private bool continueMark;  // Continue making mark after current one ends
        private GameObject curMark; // Current mark
        private Transform curMarkTr;
        private int curEdge;
        private float gapDelay; // Gap between segments

        private int curSurface = -1;  // Current surface type
        private int prevSurface = -1; // Previous surface type

        private bool popped = false;
        private bool poppedPrev = false;

        [Tooltip("How much the tire must slip before marks are created")]
        public float slipThreshold;
        private float alwaysScrape;

        public bool calculateTangents = true;

        [Tooltip("Materials in array correspond to indices in surface types in GroundSurfaceMaster")]
        public Material[] tireMarkMaterials;

        [Tooltip("Materials in array correspond to indices in surface types in GroundSurfaceMaster")]
        public Material[] rimMarkMaterials;

        [Tooltip("Particles in array correspond to indices in surface types in GroundSurfaceMaster")]
        public ParticleSystem[] debrisParticles;
        public ParticleSystem sparks;
        private float[] initialEmissionRates;
        private ParticleSystem.MinMaxCurve zeroEmission = new ParticleSystem.MinMaxCurve(0);

        private void Start() {
            tr = transform;
            w = GetComponent<Wheel>();

            initialEmissionRates = new float[debrisParticles.Length + 1];
            for (var i = 0; i < debrisParticles.Length; i++)
                initialEmissionRates[i] = debrisParticles[i].emission.rateOverTime.constantMax;

            if (sparks)
                initialEmissionRates[debrisParticles.Length] = sparks.emission.rateOverTime.constantMax;
        }

        private void Update() {
            // Check for continuous marking
            if (w.grounded
                && GroundSurfaceMaster.surfaceTypesStatic != null
                && GroundSurfaceMaster.surfaceTypesStatic.Length > 0) 
                {
                alwaysScrape = GroundSurfaceMaster.surfaceTypesStatic[w.contactPoint.surfaceType].alwaysScrape
                                   ? slipThreshold + Mathf.Min(0.5f, Mathf.Abs(w.rawRPM * 0.001f))
                                   : 0;
            }
            else
                alwaysScrape = 0;

            // Create mark
            if (w.grounded &&
                (Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip)) > slipThreshold || alwaysScrape > 0) &&
                w.connected) {
                prevSurface = curSurface;
                curSurface = w.grounded ? w.contactPoint.surfaceType : -1;

                poppedPrev = popped;
                popped = w.popped;

                if (!creatingMark) {
                    prevSurface = curSurface;
                    StartMark();
                }
                else if (curSurface != prevSurface || popped != poppedPrev) {
                    EndMark();
                }

                // Calculate segment points
                if (curMark) {
                    var pointDir = Quaternion.AngleAxis(90, w.contactPoint.normal) * tr.right *
                                   (w.popped ? w.rimWidth : w.tireWidth);
                    leftPoint = curMarkTr.InverseTransformPoint(
                        w.contactPoint.point + pointDir * w.suspensionParent.flippedSideFactor * Mathf.Sign(w.rawRPM) +
                        w.contactPoint.normal * GlobalControl.tireMarkHeightStatic);
                    rightPoint = curMarkTr.InverseTransformPoint(
                        w.contactPoint.point - pointDir * w.suspensionParent.flippedSideFactor * Mathf.Sign(w.rawRPM) +
                        w.contactPoint.normal * GlobalControl.tireMarkHeightStatic);
                }
            }
            else if (creatingMark) {
                EndMark();
            }

            // Update mark if it's short enough, otherwise end it
            if (curEdge < GlobalControl.tireMarkLengthStatic && creatingMark)
                UpdateMark();
            else if (creatingMark)
                EndMark();

            // Set particle emission rates
            ParticleSystem.EmissionModule em;
            for (var i = 0; i < debrisParticles.Length; i++) {
                if (w.connected) {
                    if (i == w.contactPoint.surfaceType) {
                        if (GroundSurfaceMaster.surfaceTypesStatic[w.contactPoint.surfaceType].leaveSparks &&
                            w.popped) {
                            em = debrisParticles[i].emission;
                            em.rateOverTime = zeroEmission;

                            if (sparks) {
                                em = sparks.emission;
                                em.rateOverTime = new ParticleSystem.MinMaxCurve(
                                    initialEmissionRates[debrisParticles.Length] * Mathf.Clamp01(
                                        Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip, alwaysScrape)) -
                                        slipThreshold));
                            }
                        }
                        else {
                            em = debrisParticles[i].emission;
                            em.rateOverTime = new ParticleSystem.MinMaxCurve(
                                initialEmissionRates[i] *
                                Mathf.Clamp01(Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip, alwaysScrape)) -
                                              slipThreshold));

                            if (sparks) {
                                em = sparks.emission;
                                em.rateOverTime = zeroEmission;
                            }
                        }
                    }
                    else {
                        em = debrisParticles[i].emission;
                        em.rateOverTime = zeroEmission;
                    }
                }
                else {
                    em = debrisParticles[i].emission;
                    em.rateOverTime = zeroEmission;

                    if (sparks) {
                        em = sparks.emission;
                        em.rateOverTime = zeroEmission;
                    }
                }
            }
        }

        // Start creating a mark
        private void StartMark() {
            if (rimMarkMaterials == null 
                || rimMarkMaterials.Length == 0 
                || tireMarkMaterials == null 
                || tireMarkMaterials.Length == 0)
                return;
            creatingMark = true;
            curMark = new GameObject("Tire Mark");
            curMarkTr = curMark.transform;
            curMarkTr.parent = w.contactPoint.col.transform;
            curMark.AddComponent<TireMark>();
            var tempRend = curMark.AddComponent<MeshRenderer>();

            // Set material based on whether the tire is popped
            tempRend.material = w.popped 
                                ? rimMarkMaterials[Mathf.Min(w.contactPoint.surfaceType, rimMarkMaterials.Length - 1)] 
                                : tireMarkMaterials[Mathf.Min(w.contactPoint.surfaceType, tireMarkMaterials.Length - 1)];

            tempRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mesh = curMark.AddComponent<MeshFilter>().mesh;
            verts = new Vector3[GlobalControl.tireMarkLengthStatic * 2];
            tris = new int[GlobalControl.tireMarkLengthStatic * 3];

            if (continueMark) {
                verts[0] = leftPointPrev;
                verts[1] = rightPointPrev;

                tris[0] = 0;
                tris[1] = 3;
                tris[2] = 1;
                tris[3] = 0;
                tris[4] = 2;
                tris[5] = 3;
            }

            uvs = new Vector2[verts.Length];
            uvs[0] = new Vector2(0, 0);
            uvs[1] = new Vector2(1, 0);
            uvs[2] = new Vector2(0, 1);
            uvs[3] = new Vector2(1, 1);

            colors = new Color[verts.Length];
            colors[0].a = 0;
            colors[1].a = 0;

            curEdge = 2;
            gapDelay = GlobalControl.tireMarkGapStatic;
        }

        // Update mark currently being generated
        private void UpdateMark() {
            if (gapDelay == 0) {
                var alpha = (curEdge < GlobalControl.tireMarkLengthStatic - 2 && curEdge > 5 ? 1 : 0) *
                            Random.Range(
                                Mathf.Clamp01(Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip, alwaysScrape)) -
                                              slipThreshold) * 0.9f,
                                Mathf.Clamp01(Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip, alwaysScrape)) -
                                              slipThreshold));
                gapDelay = GlobalControl.tireMarkGapStatic;
                curEdge += 2;

                verts[curEdge] = leftPoint;
                verts[curEdge + 1] = rightPoint;

                for (var i = curEdge + 2; i < verts.Length; i++) {
                    verts[i] = Mathf.Approximately(i * 0.5f, Mathf.Round(i * 0.5f)) ? leftPoint : rightPoint;
                    colors[i].a = 0;
                }

                tris[curEdge * 3 - 3] = curEdge;
                tris[curEdge * 3 - 2] = curEdge + 3;
                tris[curEdge * 3 - 1] = curEdge + 1;
                tris[Mathf.Min(curEdge * 3, tris.Length - 1)] = curEdge;
                tris[Mathf.Min(curEdge * 3 + 1, tris.Length - 1)] = curEdge + 2;
                tris[Mathf.Min(curEdge * 3 + 2, tris.Length - 1)] = curEdge + 3;

                uvs[curEdge] = new Vector2(0, curEdge * 0.5f);
                uvs[curEdge + 1] = new Vector2(1, curEdge * 0.5f);

                colors[curEdge] = new Color(1, 1, 1, alpha);
                colors[curEdge + 1] = colors[curEdge];

                mesh.vertices = verts;
                mesh.triangles = tris;
                mesh.uv = uvs;
                mesh.colors = colors;
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
            }
            else {
                gapDelay = Mathf.Max(0, gapDelay - Time.deltaTime);
                verts[curEdge] = leftPoint;
                verts[curEdge + 1] = rightPoint;

                for (var i = curEdge + 2; i < verts.Length; i++) {
                    verts[i] = Mathf.Approximately(i * 0.5f, Mathf.Round(i * 0.5f)) ? leftPoint : rightPoint;
                    colors[i].a = 0;
                }

                mesh.vertices = verts;
                mesh.RecalculateBounds();
            }

            if (calculateTangents)
                mesh.RecalculateTangents();
        }

        // Stop making mark
        private void EndMark() {
            creatingMark = false;
            leftPointPrev = verts[Mathf.RoundToInt(verts.Length * 0.5f)];
            rightPointPrev = verts[Mathf.RoundToInt(verts.Length * 0.5f + 1)];
            continueMark = w.grounded;

            curMark.GetComponent<TireMark>().fadeTime = GlobalControl.tireFadeTimeStatic;
            curMark.GetComponent<TireMark>().mesh = mesh;
            curMark.GetComponent<TireMark>().colors = colors;
            curMark = null;
            curMarkTr = null;
            mesh = null;
        }

        // Clean up mark if destroyed while creating
        private void OnDestroy() {
            if (creatingMark && curMark)
                EndMark();
            else if (mesh != null)
                Destroy(mesh);
        }
    }

    // Class for tire mark instances
    public class TireMark : MonoBehaviour {
        [System.NonSerialized] public float fadeTime = -1;
        private bool fading;
        private float alpha = 1;
        [System.NonSerialized] public Mesh mesh;
        [System.NonSerialized] public Color[] colors;

        // Fade the tire mark and then destroy it
        private void Update() {
            if (fading) {
                if (alpha <= 0) {
                    Destroy(mesh);
                    Destroy(gameObject);
                }
                else {
                    alpha -= Time.deltaTime;

                    for (var i = 0; i < colors.Length; i++)
                        colors[i].a -= Time.deltaTime;

                    mesh.colors = colors;
                }
            }
            else {
                if (fadeTime > 0)
                    fadeTime = Mathf.Max(0, fadeTime - Time.deltaTime);
                else if (fadeTime == 0)
                    fading = true;
            }
        }
    }
}