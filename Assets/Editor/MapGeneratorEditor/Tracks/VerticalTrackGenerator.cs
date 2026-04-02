using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class VerticalTrackGenerator : EditorWindow
{
    public MapSettings settings;
    private GameObject mapRoot;

    float currentCumulativePitch = 0f;

    [MenuItem("TOOLS/Road Trackers/11. 🎯 LEGO SYSTEM (START FIXED)")]
    public static void Open()
    {
        GetWindow<VerticalTrackGenerator>("Lego Modu");
    }

    void OnGUI()
    {
        GUI.backgroundColor = Color.yellow;
        GUILayout.Label("🎯 LEGO SİSTEMİ – START FIXED", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.white;

        settings = (MapSettings)EditorGUILayout.ObjectField(
            "Ayarlar Dosyası",
            settings,
            typeof(MapSettings),
            false
        );

        GUILayout.Space(10);

        if (GUILayout.Button("HARİTAYI SIFIRLA VE OLUŞTUR", GUILayout.Height(45)))
        {
            GenerateTrack();
        }
    }

    void GenerateTrack()
    {
        if (settings == null)
        {
            Debug.LogError("Ayarlar dosyası eksik!");
            return;
        }

        if (settings.trackLength < 2)
            settings.trackLength = 2;

        GameObject oldMap = GameObject.Find("Generated_Vertical_Map");
        if (oldMap != null)
            DestroyImmediate(oldMap);

        mapRoot = new GameObject("Generated_Vertical_Map");

        currentCumulativePitch = 0f;

        // ======================
        // START (ÖZEL – TRACKPIECE DEĞİL)
        // ======================
        Vector3 nextPoint = SpawnStart(settings.startPrefab);

        // ======================
        // BODY
        // ======================
        int bodyCount = Mathf.Max(0, settings.trackLength - 2);

        for (int i = 0; i < bodyCount; i++)
        {
            GameObject prefab = SelectRandomPrefab(nextPoint.y);
            if (prefab == null) continue;

            bool isDown = settings.rampDownPrefabs.Contains(prefab);

            nextPoint = SpawnAndConnect(
                prefab,
                nextPoint,
                settings.vertPathRot,
                $"Piece_{i}",
                isDown
            );
        }

        // ======================
        // FINISH (NORMAL TRACKPIECE)
        // ======================
        GameObject finishPrefab = GetRandom(settings.finishPrefabs);
        if (finishPrefab != null)
        {
            SpawnAndConnect(
                finishPrefab,
                nextPoint,
                settings.vertFinishRot,
                "Finish_Piece",
                false
            );
        }
        else
        {
            Debug.LogError("Finish prefab yok!");
        }

        Selection.activeGameObject = mapRoot;
        SceneView.lastActiveSceneView.FrameSelected();
    }

    // ======================
    // START SPAWN (TRACKPIECE YOK)
    // ======================
   // ======================
// START SPAWN (TRACKPIECE UYUMLU)
// ======================
Vector3 SpawnStart(GameObject prefab)
{
    if (prefab == null) {
        Debug.LogError("Start prefab atanmamış!");
        return Vector3.zero;
    }

    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
    obj.name = "Start_Piece";
    obj.transform.SetParent(mapRoot.transform);
    
    // Dünyanın merkezine koy
    obj.transform.position = Vector3.zero;
    // Eğer start parçası hala tersse, 180f olan yeri 0 yap veya tam tersi
    obj.transform.rotation = Quaternion.Euler(0, settings.vertStartRot, 0);
    obj.transform.localScale = Vector3.one * settings.scaleMultiplier;

    TrackPiece piece = obj.GetComponent<TrackPiece>();
    if (piece == null) {
        Debug.LogError("Start prefab'ında TrackPiece scripti yok!");
        return Vector3.zero;
    }

    // Çıkış noktasını (EndOffset) hesapla ve bir sonraki parça için döndür
    Vector3 worldEnd = obj.transform.position + (obj.transform.rotation * Vector3.Scale(piece.endOffset, obj.transform.localScale));
    return worldEnd;
}

// ======================
// BODY / FINISH (PÜRÜZSÜZ BİRLEŞİM)
// ======================
Vector3 SpawnAndConnect(GameObject prefab, Vector3 connectionPoint, float baseRotY, string name, bool isDown)
{
    if (prefab == null) return connectionPoint;

    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
    obj.name = name;
    obj.transform.SetParent(mapRoot.transform);
    obj.transform.localScale = Vector3.one * settings.scaleMultiplier;

    // 1. ROTASYON: Eğim ve Yön
    float finalRotY = isDown ? baseRotY + 180f : baseRotY;
    obj.transform.rotation = Quaternion.Euler(currentCumulativePitch, finalRotY, 0);

    TrackPiece piece = obj.GetComponent<TrackPiece>();
    
    // 2. POZİSYON VE MIKNATIS ETKİSİ
    Vector3 worldStart = obj.transform.rotation * Vector3.Scale(piece.startOffset, obj.transform.localScale);
    
    // GAP FIX: 0.01f ileri iterek parçaları birbirine kenetliyoruz (Görsel pürüzü bu çözer)
    float gapOverlap = 0.01f;
    obj.transform.position = connectionPoint - worldStart + (obj.transform.forward * gapOverlap);

    // 3. PITCH GÜNCELLEME
    float pitchDelta = isDown ? -piece.exitPitch : piece.exitPitch;
    currentCumulativePitch += pitchDelta;

    // 4. SONRAKİ BAĞLANTI NOKTASI
    Vector3 worldEnd = obj.transform.rotation * Vector3.Scale(piece.endOffset, obj.transform.localScale);
    return obj.transform.position + worldEnd;
}

    // ======================
    // PREFAB SEÇİMİ
    // ======================
    GameObject SelectRandomPrefab(float currentY)
    {
        float rnd = Random.value;

        if (currentY < 1f)
        {
            List<GameObject> pool =
                rnd < 0.5f
                    ? settings.rampUpPrefabs
                    : settings.verticalStraightPrefabs;

            return GetRandom(pool);
        }
        else
        {
            if (rnd < 0.3f) return GetRandom(settings.rampUpPrefabs);
            if (rnd < 0.6f) return GetRandom(settings.rampDownPrefabs);
            return GetRandom(settings.verticalStraightPrefabs);
        }
    }

    GameObject GetRandom(List<GameObject> list)
    {
        if (list == null || list.Count == 0) return null;

        List<GameObject> valid = list.FindAll(x => x != null);
        if (valid.Count == 0) return null;

        return valid[Random.Range(0, valid.Count)];
    }
}
