using UnityEngine;
using UnityEditor;

public class LoopTrackGenerator : EditorWindow
{
    public MapSettings settings;
    private GameObject mapRoot;

    // YOL TAKİP DEĞİŞKENLERİ
    private Vector3 connectPoint;
    private float rotY;

    // --- KÖŞE AYARI (MANUEL) ---
    // Düz yolları otomatik ölçüyoruz ama köşeler için bu sayıyı sen ayarlayacaksın.
    // Çünkü L şeklindeki objelerin "yol uzunluğu"nu bilgisayar tam ölçemez.
    float manualCornerSize = 1f; 

    [MenuItem("TOOLS/Road Trackers/12. 🏁 LAP SYSTEM (FINAL)")]
    public static void Open()
    {
        GetWindow<LoopTrackGenerator>("Lap Final");
    }

    void OnGUI()
    {
        GUI.backgroundColor = Color.green;
        GUILayout.Label("🏁 LAP SİSTEMİ (FİNAL)", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);
        settings = (MapSettings)EditorGUILayout.ObjectField("Ayarlar Dosyası", settings, typeof(MapSettings), false);

        if (settings != null)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("🔧 HASSAS AYARLAR", EditorStyles.boldLabel);
            
            // Düz yollar zaten otomatik, buraya dokunma
            GUILayout.Label($"Scale: {settings.lapScale}", EditorStyles.miniLabel);

            // KÖŞE AYARI BURADA
            GUI.backgroundColor = Color.yellow;
            GUILayout.Label("👇 KÖŞELERİ BURADAN OTURT 👇");
            manualCornerSize = EditorGUILayout.FloatField("Köşe Boyutu (Corner Size):", manualCornerSize);
            GUI.backgroundColor = Color.white;
            
            GUILayout.Label("İpucu: Parçalar üst üste biniyorsa sayıyı büyüt,\nboşluk kalıyorsa küçült.", EditorStyles.miniLabel);
            
            GUILayout.EndVertical();

            GUILayout.Space(10);

            if (GUILayout.Button("PİSTİ OLUŞTUR", GUILayout.Height(50)))
            {
                GenerateLap();
            }
        }
    }

    void GenerateLap()
    {
        if (settings.lapStraightPrefabs.Count == 0 || settings.lapCornerPrefabs.Count == 0) return;

        if (GameObject.Find("Generated_Lap_Map")) DestroyImmediate(GameObject.Find("Generated_Lap_Map"));

        mapRoot = new GameObject("Generated_Lap_Map");
        Undo.RegisterCreatedObjectUndo(mapRoot, "Create Lap Map");

        connectPoint = Vector3.zero;
        rotY = 0f;

        // 4 KENAR DÖNGÜSÜ
        for (int side = 0; side < 4; side++)
        {
            int count = (side % 2 == 0) ? settings.lapWidth : settings.lapDepth;

            // 1. Düz Parçaları Diz (Otomatik Ölçüm)
            for (int i = 0; i < count; i++)
            {
                PlaceStraight();
            }

            // 2. Köşeyi Koy (Manuel Ayarlı)
            PlaceCorner(true); 
        }

        Selection.activeGameObject = mapRoot;
        SceneView.lastActiveSceneView.FrameSelected();
    }

    // --- DÜZ PARÇA (OTOMATİK - DOKUNMA, BU ÇALIŞIYOR) ---
    void PlaceStraight()
    {
        GameObject prefab = settings.lapStraightPrefabs[Random.Range(0, settings.lapStraightPrefabs.Count)];
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        obj.transform.SetParent(mapRoot.transform);
        
        obj.transform.rotation = Quaternion.Euler(0, rotY, 0);
        obj.transform.localScale = Vector3.one * settings.lapScale;

        // Otomatik Sınır Ölçümü (Pivot Fix)
        Bounds b = GetBounds(obj);
        float pivotFix = -b.min.z; 

        // Yerleştir
        Vector3 direction = Quaternion.Euler(0, rotY, 0) * Vector3.forward;
        obj.transform.position = connectPoint + (direction * pivotFix);

        // Bir sonraki noktaya ilerle
        connectPoint += direction * b.size.z;
    }

    // --- KÖŞE PARÇA (MANUEL AYARLI) ---
    void PlaceCorner(bool rightTurn)
    {
        GameObject prefab = settings.lapCornerPrefabs[Random.Range(0, settings.lapCornerPrefabs.Count)];
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        obj.transform.SetParent(mapRoot.transform);
        
        obj.transform.rotation = Quaternion.Euler(0, rotY, 0);
        obj.transform.localScale = Vector3.one * settings.lapScale;

        // 1. GİRİŞ KISMI (Otomatik)
        // Köşenin girişini önceki yola yapıştırıyoruz (Bu kısım otomatik)
        Bounds b = GetBounds(obj);
        float pivotFix = -b.min.z;
        Vector3 forwardDir = Quaternion.Euler(0, rotY, 0) * Vector3.forward;
        obj.transform.position = connectPoint + (forwardDir * pivotFix);

        // 2. ÇIKIŞ KISMI (Manuel)
        // Burası senin girdiğin sayıya göre hesaplanacak!
        // Scale ile çarpmayı unutmuyoruz ki harita büyüyünce bozulmasın.
        float finalSize = manualCornerSize * settings.lapScale;

        Vector3 sideDir = rightTurn
            ? Quaternion.Euler(0, rotY, 0) * Vector3.right
            : Quaternion.Euler(0, rotY, 0) * Vector3.left;

        // Köşe dönüşü: Hem ileri git hem yana dön
        connectPoint += (forwardDir * finalSize) + (sideDir * finalSize);

        // Yönü çevir
        rotY += rightTurn ? 90f : -90f;
    }

    // --- YARDIMCI: BOUNDS HESAPLAMA ---
    Bounds GetBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(Vector3.zero, Vector3.zero);

        Vector3 oldPos = obj.transform.position;
        Quaternion oldRot = obj.transform.rotation;
        
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        
        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) combinedBounds.Encapsulate(renderers[i].bounds);

        obj.transform.position = oldPos;
        obj.transform.rotation = oldRot;

        return combinedBounds;
    }
}