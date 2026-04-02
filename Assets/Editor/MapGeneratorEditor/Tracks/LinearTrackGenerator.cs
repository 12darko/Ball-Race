using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LinearTrackGenerator : EditorWindow
{
    public MapSettings settings; 
    private UnityEditor.Editor settingsEditor; 
    private GameObject mapRoot;

    [MenuItem("TOOLS/Road Trackers/1. 🔥 Linear Generator")]
    public static void Open() { GetWindow<LinearTrackGenerator>("Linear Gen"); }

    void OnEnable()
    {
        string lastPath = EditorPrefs.GetString("LastMapSettingsPath", "");
        if (!string.IsNullOrEmpty(lastPath)) settings = AssetDatabase.LoadAssetAtPath<MapSettings>(lastPath);
    }

    void OnGUI()
    {
        GUILayout.Label("🔥 Düz Parkur", EditorStyles.boldLabel);
        settings = (MapSettings)EditorGUILayout.ObjectField("Ayarlar", settings, typeof(MapSettings), false);
        if (settings != null) EditorPrefs.SetString("LastMapSettingsPath", AssetDatabase.GetAssetPath(settings));

        if (GUILayout.Button("OLUŞTUR", GUILayout.Height(40))) GenerateLinearTrack();
    }

    void GenerateLinearTrack()
    {
        if(settings == null) return;
        ClearMap();
        mapRoot = new GameObject("Generated_Linear_Map");
        Vector3 currentPos = Vector3.zero;

        // 1. START
        GameObject startObj = SpawnPiece(settings.startPrefab, currentPos, Quaternion.Euler(0, settings.linStartRot, 0), "Start");
        if (startObj != null)
        {
            float len = CalculateLength(startObj);
            currentPos += Vector3.forward * (len + settings.linStartGap);
        }

        // 2. YOL
        for (int i = 0; i < settings.trackLength - 2; i++)
        {
            GameObject prefab;
            if (Random.value < settings.obstacleFrequency && settings.linearObstaclePaths.Count > 0)
                prefab = GetRandom(settings.linearObstaclePaths);
            else
                prefab = GetRandom(settings.linearSafePaths);

            GameObject pathObj = SpawnPiece(prefab, currentPos, Quaternion.Euler(0, settings.linPathRot, 0), $"Path_{i}");
            if (pathObj != null)
            {
                float len = CalculateLength(pathObj);
                currentPos += Vector3.forward * (len + settings.linGlobalGap);
            }
        }

        // 3. FINISH (Random)
        GameObject finishPrefab = GetRandom(settings.finishPrefabs);
        SpawnPiece(finishPrefab, currentPos, Quaternion.Euler(0, settings.linFinishRot, 0), "Finish");

        Selection.activeGameObject = mapRoot;
        SceneView.lastActiveSceneView.FrameSelected();
    }

    // --- Yardımcılar ---
    GameObject SpawnPiece(GameObject prefab, Vector3 pos, Quaternion rot, string name)
    {
        if (prefab == null) return null;
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        obj.transform.localScale = Vector3.one * settings.scaleMultiplier;
        obj.name = name;
        obj.transform.SetParent(mapRoot.transform);
        return obj;
    }
    float CalculateLength(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 0f;
        Bounds b = renderers[0].bounds;
        foreach (Renderer r in renderers) b.Encapsulate(r.bounds);
        return b.size.z;
    }
    GameObject GetRandom(List<GameObject> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
    void ClearMap() { DestroyImmediate(GameObject.Find("Generated_Linear_Map")); }
}