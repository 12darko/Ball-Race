using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class VersusSnakeGenerator : EditorWindow
{
    public MapSettings settings;
    private GameObject mapRoot;
    private GameObject cursor;

    // 🔥 YENİ: BOŞLUK (GAP) AYARI
    [Range(0f, 1f)] private float gapChance = 0.1f; // %10 şansla boşluk bırak

    [MenuItem("TOOLS/Road Trackers//Versus Mode/14. 🐍⚔️ VERSUS SNAKE (GAP EDITION)")]
    public static void Open() { GetWindow<VersusSnakeGenerator>("VS Snake Gap"); }

    void OnGUI()
    {
        GUI.backgroundColor = new Color(1f, 1f, 0.4f); // Sarımsı
        GUILayout.Label("🐍⚔️ VERSUS SNAKE (BOŞLUKLU)", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);
        settings = (MapSettings)EditorGUILayout.ObjectField("Ayarlar Dosyası", settings, typeof(MapSettings), false);

        if (settings != null)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("📏 Genel Ayarlar", EditorStyles.boldLabel);
            settings.bridgeLength = EditorGUILayout.IntField("Yol Uzunluğu (Adet):", settings.bridgeLength);
            settings.arenaScale = EditorGUILayout.FloatField("Scale:", settings.arenaScale);
            settings.bridgeGap = EditorGUILayout.FloatField("Parça Aralığı:", settings.bridgeGap);
            
            // 🔥 YENİ: ORTAYI BOŞ BIRAKMA SEÇENEĞİ (TÜMÜNÜ)
            settings.emptyMiddle = EditorGUILayout.Toggle("Ortayı Komple Sil:", settings.emptyMiddle);
            GUILayout.EndVertical();

            GUILayout.Space(5);
            
            if (!settings.emptyMiddle)
            {
                GUILayout.BeginVertical("box");
                GUI.backgroundColor = Color.yellow;
                GUILayout.Label("🐍 Yılan & Boşluk Ayarları", EditorStyles.boldLabel);
                
                settings.vsSnakeTurnChance = EditorGUILayout.Slider("Kıvrılma Şansı:", settings.vsSnakeTurnChance, 0f, 1f);
                settings.vsSnakeMaxSideSteps = EditorGUILayout.IntSlider("Max Yan Yol:", settings.vsSnakeMaxSideSteps, 1, 5);
                
                GUILayout.Space(5);
                // BOŞLUK SLIDER'I
                GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
                gapChance = EditorGUILayout.Slider("🕳️ Boşluk Şansı:", gapChance, 0f, 0.5f); // Max %50 olsun ki yol bitmesin
                EditorGUILayout.HelpBox("%0 = Deliksiz Yol, %20 = Arada 1-2 Parça Siler", MessageType.None);
                GUI.backgroundColor = Color.white;

                GUILayout.EndVertical();
            }

            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.cyan;
            GUILayout.Label("🥅 Kale Ayarları", EditorStyles.boldLabel);
            settings.versusGoalRot = EditorGUILayout.Slider("Kale Açısı:", settings.versusGoalRot, 0, 360);
            settings.fillGoalGap = EditorGUILayout.Toggle("Kale Önünü Doldur:", settings.fillGoalGap);
            GUI.backgroundColor = Color.white;
            GUILayout.EndVertical();

            GUILayout.Space(20);

            if (GUILayout.Button("ARENAYI OLUŞTUR", GUILayout.Height(50)))
            {
                GenerateDynamicArena();
            }
        }
    }

    void GenerateDynamicArena()
    {
        if (settings.vsSnakeBlueBases == null || settings.vsSnakeRedBases == null || settings.vsSnakeBridgeStraights == null) return;

        if (GameObject.Find("Generated_Versus_Snake")) DestroyImmediate(GameObject.Find("Generated_Versus_Snake"));
        mapRoot = new GameObject("Generated_Versus_Snake");
        Undo.RegisterCreatedObjectUndo(mapRoot, "Create Versus Snake");

        // --- 1. MAVİ ÜS ---
        Vector3 startPos = Vector3.zero;
        GenerateBase(startPos, false); 

        // --- 2. KÖPRÜ BAŞLANGIÇ ---
        float startDist = (settings.arenaScale / 2f) + settings.bridgeGap + (settings.arenaScale / 2f);
        Vector3 bridgeStartPos = startPos + (Vector3.forward * startDist);

        // --- 3. YILAN ---
        Transform endCursor;

        if (settings.emptyMiddle)
        {
            // Ortası komple boşsa dummy cursor ile mesafe ölç
            GameObject dummyCursor = new GameObject("Dummy_Cursor");
            dummyCursor.transform.position = bridgeStartPos;
            dummyCursor.transform.rotation = Quaternion.identity;
            float totalStraightDist = (settings.bridgeLength - 1) * (settings.arenaScale + settings.bridgeGap);
            if (settings.bridgeLength > 1) dummyCursor.transform.Translate(Vector3.forward * totalStraightDist);
            endCursor = dummyCursor.transform;
        }
        else
        {
            // Boşluklu Yılanı Oluştur
            endCursor = GenerateSnakeBridge(bridgeStartPos);
        }

        // --- 4. KIRMIZI ÜS ---
        float offsetDistance = settings.arenaScale + settings.bridgeGap + (settings.arenaScale * 0.1f);
        Vector3 redBasePos = endCursor.position + (endCursor.forward * offsetDistance);
        Quaternion redBaseRot = endCursor.rotation * Quaternion.Euler(0, 180, 0);

        GenerateBase(redBasePos, true, redBaseRot);

        if (cursor != null) DestroyImmediate(cursor);
        if (settings.emptyMiddle) DestroyImmediate(endCursor.gameObject);
        
        Selection.activeGameObject = mapRoot;
        SceneView.lastActiveSceneView.FrameSelected();
    }

    Transform GenerateSnakeBridge(Vector3 startPos)
    {
        cursor = new GameObject("Cursor_Logic_Bridge");
        cursor.transform.position = startPos;
        cursor.transform.rotation = Quaternion.identity; 

        float straightHalf = (settings.straightLength * settings.arenaScale) / 2f;
        float cornerHalf = (settings.cornerSize * settings.arenaScale) / 2f;
        
        // İlk parça düzeltmesi
        float firstPieceCorrection = straightHalf + settings.bridgeGap + straightHalf; 
        cursor.transform.Translate(Vector3.back * firstPieceCorrection);

        float previousHalfLength = straightHalf; 

        for (int i = 0; i < settings.bridgeLength; i++)
        {
            // İlk ve Son parçada asla boşluk olmasın
            bool isSafeZone = (i == 0 || i >= settings.bridgeLength - 1);
            
            // Son 2 parça kıvrılmasın
            bool forceStraight = (i >= settings.bridgeLength - 2);

            if (!forceStraight && Random.value < settings.vsSnakeTurnChance)
            {
                // --- KIVRILMA (KÖŞELERDE ASLA BOŞLUK OLMAZ) ---
                bool goRight = (Random.value > 0.5f);
                int sideSteps = Random.Range(1, settings.vsSnakeMaxSideSteps + 1);
                bool needMirror = !goRight; 

                float turnAngle = goRight ? 90 : -90;
                // Köşe: createMesh = true
                PlacePiece(settings.vsSnakeBridgeCorners, cornerHalf, ref previousHalfLength, true, needMirror, true);
                cursor.transform.Rotate(0, turnAngle, 0);
                SnapCursor();

                for(int s=0; s<sideSteps; s++)
                {
                    // Yan yollarda boşluk olabilir mi? İstersen buraya da şans koyabilirsin.
                    // Şimdilik yan yolları sağlam yapıyoruz, sadece ana hatta boşluk olsun.
                    PlacePiece(settings.vsSnakeBridgeStraights, straightHalf, ref previousHalfLength, false, false, true);
                }

                float returnAngle = goRight ? -90 : 90;
                bool returnMirror = goRight; 
                // Köşe: createMesh = true
                PlacePiece(settings.vsSnakeBridgeCorners, cornerHalf, ref previousHalfLength, true, returnMirror, true);
                cursor.transform.Rotate(0, returnAngle, 0);
                SnapCursor();
            }
            else
            {
                // --- DÜZ GİT (BOŞLUK ŞANSI BURADA) ---
                bool createMesh = true;

                // Eğer güvenli bölge değilse ve zar tutarsa boşluk yap
                if (!isSafeZone && Random.value < gapChance)
                {
                    createMesh = false; // Parçayı koyma, sadece ilerle
                }

                PlacePiece(settings.vsSnakeBridgeStraights, straightHalf, ref previousHalfLength, false, false, createMesh);
            }
        }
        return cursor.transform;
    }

    // PlacePiece artık 'createMesh' parametresi alıyor
    void PlacePiece(List<GameObject> prefabs, float currentHalfLen, ref float prevHalfLen, bool isCorner, bool mirrorX, bool createMesh)
    {
        if (prefabs == null || prefabs.Count == 0) return;

        // 1. İlerle (Mesh olsa da olmasa da cursor ilerlemeli)
        float moveDist = prevHalfLen + settings.bridgeGap + currentHalfLen;
        cursor.transform.Translate(Vector3.forward * moveDist);
        SnapCursor();

        // 2. Parçayı Koy (Sadece createMesh true ise)
        if (createMesh)
        {
            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            
            obj.transform.SetParent(mapRoot.transform);
            obj.transform.position = cursor.transform.position;
            obj.transform.rotation = cursor.transform.rotation;
            
            float s = settings.arenaScale;
            obj.transform.localScale = mirrorX ? new Vector3(-s, s, s) : new Vector3(s, s, s);
        }

        // 3. Referansı Güncelle (Boşluk olsa bile uzunluk hesabı tutmalı)
        prevHalfLen = currentHalfLen;
    }

    void GenerateBase(Vector3 pos, bool isRedTeam, Quaternion? customRotation = null)
    {
        List<GameObject> baseList = isRedTeam ? settings.vsSnakeRedBases : settings.vsSnakeBlueBases;
        List<GameObject> goalList = isRedTeam ? settings.vsSnakeRedGoals : settings.vsSnakeBlueGoals;
        List<GameObject> connList = isRedTeam ? settings.vsSnakeRedConnectors : settings.vsSnakeBlueConnectors;

        GameObject basePrefab = GetRandom(baseList);
        if (basePrefab == null) return;

        GameObject baseObj = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
        baseObj.transform.SetParent(mapRoot.transform);
        baseObj.transform.localScale = Vector3.one * settings.arenaScale;
        
        if (customRotation.HasValue) baseObj.transform.rotation = customRotation.Value;
        else
        {
            float baseRotY = isRedTeam ? 180 : 0;
            baseObj.transform.rotation = Quaternion.Euler(0, baseRotY, 0);
        }
        baseObj.transform.position = pos;

        // Kale her zaman "SIRTTA" (-forward)
        Vector3 attachDir = -baseObj.transform.forward; 

        Vector3 currentAttachPoint = pos + (attachDir * (settings.arenaScale * 0.5f));
        currentAttachPoint += attachDir * (settings.arenaScale * 0.01f); 

        if (settings.fillGoalGap && connList != null && connList.Count > 0)
        {
            GameObject connPrefab = GetRandom(connList);
            GameObject connObj = (GameObject)PrefabUtility.InstantiatePrefab(connPrefab);
            connObj.transform.SetParent(mapRoot.transform);
            connObj.transform.localScale = Vector3.one * settings.arenaScale;
            connObj.transform.rotation = baseObj.transform.rotation;
            
            connObj.transform.position = currentAttachPoint + (attachDir * (settings.arenaScale * 0.5f));
            currentAttachPoint += (attachDir * settings.arenaScale); 
        }
        else { currentAttachPoint += (attachDir * (settings.arenaScale * 0.5f)); }

        if (goalList != null && goalList.Count > 0)
        {
            GameObject goalPrefab = GetRandom(goalList);
            GameObject goal = (GameObject)PrefabUtility.InstantiatePrefab(goalPrefab);
            goal.transform.SetParent(mapRoot.transform);
            goal.transform.localScale = Vector3.one * settings.arenaScale;
            
            goal.transform.rotation = baseObj.transform.rotation * Quaternion.Euler(0, settings.versusGoalRot, 0);
            goal.transform.position = currentAttachPoint + (attachDir * (settings.arenaScale * 0.5f));
        }
    }

    void SnapCursor()
    {
        Vector3 p = cursor.transform.position;
        cursor.transform.position = new Vector3(Mathf.Round(p.x * 10f)/10f, p.y, Mathf.Round(p.z * 10f)/10f);
        Vector3 e = cursor.transform.rotation.eulerAngles;
        cursor.transform.rotation = Quaternion.Euler(0, Mathf.Round(e.y/90f)*90f, 0);
    }

    GameObject GetRandom(List<GameObject> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}