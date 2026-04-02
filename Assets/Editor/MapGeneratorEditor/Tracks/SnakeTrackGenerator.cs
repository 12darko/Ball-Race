using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SnakeTrackGenerator : EditorWindow
{
    public MapSettings settings; 
    private GameObject mapRoot;
    
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

    private int attemptCount = 0;
    private const int MAX_ATTEMPTS = 100; // Deneme limitini biraz artırdım

    // YENİ AYAR: Bitiş noktası Başlangıçtan en az ne kadar uzakta olmalı?
    // Bu değer Prefab boyutuna göre değişir. Senin scale 10 ise, buraya 30-40 yazabilirsin.
    public float minFinishDistance = 20f; 

    [MenuItem("TOOLS/Road Trackers/19. 🐍 Snake Generator (DISTANCE CHECK)")]
    public static void Open() { GetWindow<SnakeTrackGenerator>("Snake Final"); }

    void OnGUI()
    {
        GUILayout.Label("🐍 Snake (Uzak Bitiş Garantili)", EditorStyles.boldLabel);
        settings = (MapSettings)EditorGUILayout.ObjectField("Ayarlar", settings, typeof(MapSettings), false);
        
        // Editörden ayarlanabilir mesafe
        minFinishDistance = EditorGUILayout.FloatField("Min Bitiş Uzaklığı", minFinishDistance);

        if (settings != null)
        {
             EditorGUILayout.HelpBox($"ÖNEMLİ: Eğer 'Min Bitiş Uzaklığı'nı çok yüksek yaparsan (örn: {settings.trackLength * settings.scaleMultiplier} gibi), sistem uygun harita bulamaz.", MessageType.Info);
        }
        
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("KUSURSUZ OLUŞTUR", GUILayout.Height(40))) 
        {
            GenerateUntilSuccess();
        }
    }

    void GenerateUntilSuccess()
    {
        if(settings == null) return;

        attemptCount = 0;
        bool success = false;

        // Başarılı olana kadar döngü
        while (!success && attemptCount < MAX_ATTEMPTS)
        {
            attemptCount++;
            ClearMap();
            success = TryGenerateSnakeTrack();
        }

        if (success)
        {
            Debug.Log($"✅ Harita {attemptCount}. denemede başarıyla oluşturuldu! (Mesafe Kriteri Sağlandı)");
            Selection.activeGameObject = mapRoot;
            SceneView.lastActiveSceneView.FrameSelected();
        }
        else
        {
            Debug.LogError("❌ Uygun harita bulunamadı! 'Min Bitiş Uzaklığı'nı düşürmeyi veya Parça Sayısını artırmayı dene.");
        }
    }

    bool TryGenerateSnakeTrack()
    {
        mapRoot = new GameObject("Generated_Snake_Map");
        occupiedPositions.Clear();

        // 1. CURSOR
        GameObject cursor = new GameObject("Cursor_Logic");
        cursor.transform.position = Vector3.zero;
        cursor.transform.rotation = Quaternion.identity; 

        // Başlangıç pozisyonunu sakla (Mesafe ölçümü için)
        Vector3 startPosition = Vector3.zero;

        // Ölçüler
        float straightHalf = (settings.straightLength * settings.scaleMultiplier) / 2f;
        float cornerHalf = (settings.cornerSize * settings.scaleMultiplier) / 2f;
        float previousHalfLength = straightHalf; 

        // 2. START PARÇASI
        // Sadece Start parçasını döndür, Cursor DÜZ kalsın (Start Fix)
        Quaternion customStartRot = Quaternion.Euler(0, settings.snakeStartRot, 0);
        SpawnPiece(settings.startPrefab, cursor.transform.position, customStartRot, false, "Start");
        occupiedPositions.Add(SnapVector(cursor.transform.position));

        // 3. GÖVDE
        for (int i = 0; i < settings.trackLength - 2; i++)
        {
            float currentHalfLength = straightHalf;
            GameObject prefab = null;
            string type = "Straight";
            
            bool isCorner = false;
            bool isLeft = false;

            // --- A. KARAR ---
            bool wantsToTurn = Random.value < settings.snakeTurnChance;
            
            if (wantsToTurn)
            {
                isLeft = (Random.value < 0.5f);
                
                // --- B. LOOK-AHEAD (Sıkışma Kontrolü) ---
                float moveDist = previousHalfLength + settings.snakeGlobalGap + cornerHalf;
                
                Transform ghostCursor = Instantiate(cursor).transform; 
                ghostCursor.Translate(Vector3.forward * moveDist);
                float angle = isLeft ? -90f : 90f;
                ghostCursor.Rotate(0, angle, 0);
                
                if (!IsPathClear(ghostCursor, straightHalf, settings.snakeGlobalGap, 2))
                {
                    wantsToTurn = false;
                }
                DestroyImmediate(ghostCursor.gameObject);
            }

            // --- C. SON KARAR ---
            if (wantsToTurn)
            {
                isCorner = true;
                currentHalfLength = cornerHalf;
                prefab = GetRandom(settings.snakeRightCorners);
                type = isLeft ? "Left (Mirrored)" : "Right";
            }
            else
            {
                isCorner = false;
                currentHalfLength = straightHalf;
                prefab = GetRandom(settings.snakeStraightPrefabs);
                type = "Straight";

                float dist = previousHalfLength + settings.snakeGlobalGap + straightHalf;
                Vector3 checkPos = SimulateMove(cursor.transform, dist);
                if (IsOccupied(checkPos))
                {
                    DestroyImmediate(cursor);
                    return false; // Sıkıştık, YENİDEN DENE
                }
            }

            // --- D. HAREKET ---
            float finalMoveDist = previousHalfLength + settings.snakeGlobalGap + currentHalfLength;
            
            cursor.transform.Translate(Vector3.forward * finalMoveDist);
            SnapCursorToGrid(cursor);

            SpawnPiece(prefab, cursor.transform.position, cursor.transform.rotation, (isCorner && isLeft), $"Part_{i}_{type}");
            occupiedPositions.Add(SnapVector(cursor.transform.position));

            if (isCorner)
            {
                float angle = isLeft ? -90f : 90f;
                cursor.transform.Rotate(0, angle, 0);
                SnapCursorRotation(cursor);
            }

            previousHalfLength = currentHalfLength;
        }

        // 4. FINISH ÖNCESİ MESAFE KONTROLÜ (YENİ ÖZELLİK)
        // -----------------------------------------------------------
        float finishMoveDist = previousHalfLength + straightHalf;
        
        // Finish noktasının olacağı yer
        Vector3 potentialFinishPos = SimulateMove(cursor.transform, finishMoveDist);

        // 1. Kontrol: Finish yeri dolu mu?
        if (IsOccupied(potentialFinishPos))
        {
             DestroyImmediate(cursor);
             return false; 
        }

        // 2. Kontrol: BAŞLANGIÇTAN YETERİNCE UZAK MI?
        float distanceFromStart = Vector3.Distance(startPosition, potentialFinishPos);
        
        if (distanceFromStart < minFinishDistance)
        {
            // Mesafe çok kısa! Yılan başladığı yere geri dönmüş.
            // Bu haritayı çöpe at ve yenisini dene.
            DestroyImmediate(cursor);
            return false; 
        }
        // -----------------------------------------------------------

        // Finish Uygun, Yerleştir
        cursor.transform.Translate(Vector3.forward * finishMoveDist);
        SnapCursorToGrid(cursor);
        
        SpawnPiece(GetRandom(settings.finishPrefabs), cursor.transform.position, cursor.transform.rotation, false, "Finish");

        DestroyImmediate(cursor);
        return true; 
    }

    // --- YARDIMCILAR ---

    bool IsPathClear(Transform ghost, float halfLen, float gap, int steps)
    {
        if (IsOccupied(ghost.position)) return false;

        Vector3 futurePos = ghost.position;
        Vector3 direction = ghost.forward;
        float stepSize = (halfLen * 2) + gap; 

        for (int i = 1; i <= steps; i++)
        {
            futurePos += direction * stepSize;
            if (IsOccupied(futurePos)) return false;
        }
        return true; 
    }

    Vector3 SimulateMove(Transform t, float dist)
    {
        Vector3 pos = t.position + (t.forward * dist);
        return SnapVector(pos);
    }

    bool IsOccupied(Vector3 pos) => occupiedPositions.Contains(SnapVector(pos));

    void SpawnPiece(GameObject prefab, Vector3 pos, Quaternion rot, bool mirrorX, string name)
    {
        if (prefab == null) return;
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        obj.name = name;
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        obj.transform.SetParent(mapRoot.transform, true);
        float s = settings.scaleMultiplier;
        obj.transform.localScale = mirrorX ? new Vector3(-s, s, s) : new Vector3(s, s, s);
    }

    void SnapCursorToGrid(GameObject cursor)
    {
        Vector3 p = cursor.transform.position;
        cursor.transform.position = new Vector3(Mathf.Round(p.x * 100f)/100f, p.y, Mathf.Round(p.z * 100f)/100f);
    }

    void SnapCursorRotation(GameObject cursor)
    {
        Vector3 e = cursor.transform.rotation.eulerAngles;
        cursor.transform.rotation = Quaternion.Euler(0, Mathf.Round(e.y/90f)*90f, 0);
    }

    Vector3 SnapVector(Vector3 pos) => new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
    GameObject GetRandom(List<GameObject> list) => (list!=null && list.Count>0) ? list[Random.Range(0, list.Count)] : null;
    void ClearMap() { DestroyImmediate(GameObject.Find("Generated_Snake_Map")); }
}