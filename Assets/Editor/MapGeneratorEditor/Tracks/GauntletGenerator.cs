using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GauntletGenerator : EditorWindow
{
    public MapSettings settings;
    private GameObject mapRoot;
    private Vector3 currentPos; // İmleç (Cursor) yerine direkt pozisyon takibi

    [MenuItem("TOOLS/Road Trackers/15. 🎪 GAUNTLET (FALL GUYS)")]
    public static void Open() { GetWindow<GauntletGenerator>("Gauntlet"); }

    void OnGUI()
    {
        GUI.backgroundColor = new Color(1f, 0.4f, 1f); // Pembe/Mor (Fall Guys tonları)
        GUILayout.Label("🎪 GAUNTLET / CHAOS GENERATOR", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);
        settings = (MapSettings)EditorGUILayout.ObjectField("Ayarlar Dosyası", settings, typeof(MapSettings), false);

        if (settings != null)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("📏 Parkur Ayarları", EditorStyles.boldLabel);
            settings.gauntletLength = EditorGUILayout.IntField("Engel Sayısı:", settings.gauntletLength);
            settings.gauntletTileLength = EditorGUILayout.FloatField("Parça Uzunluğu (Tile Size):", settings.gauntletTileLength);
            settings.gauntletGap = EditorGUILayout.FloatField("Ekstra Boşluk:", settings.gauntletGap);
            settings.gauntletScale = EditorGUILayout.FloatField("Scale:", settings.gauntletScale);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox("Sistem: Start -> [Engel + Güvenli Alan] x N -> Finish", MessageType.Info);

            if (GUILayout.Button("KAOS PARKURU OLUŞTUR", GUILayout.Height(50)))
            {
                GenerateGauntlet();
            }
        }
    }

    void GenerateGauntlet()
    {
        // Güvenlik
        if (settings.gauntletModules == null || settings.gauntletModules.Count == 0)
        {
            Debug.LogError("HATA: MapSettings > Gauntlet Modules listesi boş!");
            return;
        }

        if (GameObject.Find("Generated_Gauntlet")) DestroyImmediate(GameObject.Find("Generated_Gauntlet"));
        mapRoot = new GameObject("Generated_Gauntlet");
        Undo.RegisterCreatedObjectUndo(mapRoot, "Create Gauntlet");

        // Sıfırlama
        currentPos = Vector3.zero;

        // 1. BAŞLANGIÇ (START)
        if (settings.gauntletStart != null)
        {
            SpawnPiece(settings.gauntletStart, "Start_Platform");
            // Başlangıçtan sonra bir adım ileri git
            MoveCursor();
        }

        // 2. ENGELLER VE GÜVENLİ BÖLGELER (LOOP)
        for (int i = 0; i < settings.gauntletLength; i++)
        {
            // A. ENGEL (CHAOS MODULE)
            GameObject obstaclePrefab = GetRandom(settings.gauntletModules);
            SpawnPiece(obstaclePrefab, $"Obstacle_{i+1}");
            
            MoveCursor(); // Engel bitti, ilerle

            // B. GÜVENLİ BÖLGE (SAFE ZONE)
            // Son engel değilse araya dinlenme yeri koy
            if (i < settings.gauntletLength - 1)
            {
                if (settings.gauntletSafeZones != null && settings.gauntletSafeZones.Count > 0)
                {
                    GameObject safePrefab = GetRandom(settings.gauntletSafeZones);
                    SpawnPiece(safePrefab, $"SafeZone_{i+1}");
                    
                    MoveCursor(); // Güvenli bölge bitti, ilerle
                }
            }
        }

        // 3. BİTİŞ (FINISH)
        if (settings.gauntletFinish != null && settings.gauntletFinish.Count > 0)
        {
            GameObject finishPrefab = GetRandom(settings.gauntletFinish);
            SpawnPiece(finishPrefab, "Finish_Line");
        }

        Selection.activeGameObject = mapRoot;
        SceneView.lastActiveSceneView.FrameSelected();
    }

    // --- YARDIMCILAR ---

    void SpawnPiece(GameObject prefab, string name)
    {
        if (prefab == null) return;

        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        obj.name = name;
        obj.transform.SetParent(mapRoot.transform);
        obj.transform.position = currentPos;
        obj.transform.rotation = Quaternion.identity; // Hep düz bakar (Linear)
        obj.transform.localScale = Vector3.one * settings.gauntletScale;
    }

    void MoveCursor()
    {
        // Bir sonraki parçanın merkezi nerede olacak?
        // Şu anki merkez + (TileLength * Scale) + Gap
        float step = (settings.gauntletTileLength * settings.gauntletScale) + settings.gauntletGap;
        currentPos += Vector3.forward * step;
    }

    GameObject GetRandom(List<GameObject> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}