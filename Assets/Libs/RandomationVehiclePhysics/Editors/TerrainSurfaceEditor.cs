#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RVP {
    [CustomEditor(typeof(TerrainSurface))]
    public class TerrainSurfaceEditor : Editor {
        private TerrainData terDat;
        private TerrainSurface targetScript;
        private string[] surfaceNames;

        public override void OnInspectorGUI() {
            var surfaceMaster = FindObjectOfType<GroundSurfaceMaster>();
            targetScript = (TerrainSurface) target;
            Undo.RecordObject(targetScript, "Terrain Surface Change");

            if (targetScript.GetComponent<Terrain>().terrainData)
                terDat = targetScript.GetComponent<Terrain>().terrainData;

            EditorGUILayout.LabelField("Textures and Surface Types:", EditorStyles.boldLabel);

            surfaceNames = new string[surfaceMaster.surfaceTypes.Length];

            for (var i = 0; i < surfaceNames.Length; i++)
                surfaceNames[i] = surfaceMaster.surfaceTypes[i].name;

            if (targetScript.surfaceTypes.Length > 0) {
                for (var j = 0; j < targetScript.surfaceTypes.Length; j++)
                    DrawTerrainInfo(terDat, j);
            }
            else {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("<No terrain textures found>");
            }

            if (GUI.changed)
                EditorUtility.SetDirty(targetScript);
        }

        private void DrawTerrainInfo(TerrainData ter, int index) {
            EditorGUI.indentLevel = 1;
            targetScript.surfaceTypes[index] = EditorGUILayout.Popup(terDat.terrainLayers[index].diffuseTexture.name,
                targetScript.surfaceTypes[index], surfaceNames);
            EditorGUI.indentLevel++;
        }
    }
}
#endif