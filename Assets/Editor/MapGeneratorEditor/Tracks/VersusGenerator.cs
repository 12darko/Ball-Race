using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class VersusGenerator : EditorWindow
{
    public MapSettings settings;
    private GameObject mapRoot;

    [MenuItem("TOOLS/Road Trackers/Versus Mode/13. ⚔️ VERSUS ARENA (FIXED)")]
    public static void Open() { GetWindow<VersusGenerator>("Versus Fix"); }

    void OnGUI()
    {
        GUI.backgroundColor = new Color(0.2f, 1f, 0.2f); // Yeşil tonlu pencere (Fix)
        GUILayout.Label("⚔️ VERSUS ARENA (MESAFE DÜZELTİLDİ)", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);
        settings = (MapSettings)EditorGUILayout.ObjectField("Ayarlar Dosyası", settings, typeof(MapSettings), false);

        if (settings != null)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("📏 Genel Ayarlar", EditorStyles.boldLabel);
            settings.bridgeLength = EditorGUILayout.IntField("Mesafe:", settings.bridgeLength);
            settings.arenaScale = EditorGUILayout.FloatField("Scale:", settings.arenaScale);
            settings.bridgeGap = EditorGUILayout.FloatField("Parça Aralığı:", settings.bridgeGap);
            GUILayout.EndVertical();
            
            GUILayout.Space(5);
            
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.cyan;
            GUILayout.Label("🎨 TAKIM VE KALE", EditorStyles.boldLabel);
            settings.versusGoalRot = EditorGUILayout.Slider("Kale Açısı:", settings.versusGoalRot, 0, 360);
            settings.fillGoalGap = EditorGUILayout.Toggle("Kale Önünü Doldur:", settings.fillGoalGap);
            GUI.backgroundColor = Color.white;
            GUILayout.EndVertical();
            
            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            GUILayout.Label("🌋 ORTA BÖLGE", EditorStyles.boldLabel);
            settings.spawnCenter = EditorGUILayout.Toggle("Ortaya Ada Koy:", settings.spawnCenter);
            settings.emptyMiddle = EditorGUILayout.Toggle("Köprüyü İptal Et:", settings.emptyMiddle);
            if (!settings.emptyMiddle)
                settings.scatterAmount = EditorGUILayout.Slider("Dağıtma (Scatter):", settings.scatterAmount, 0, 10f);
            GUILayout.EndVertical();

            GUILayout.Space(20);

            if (GUILayout.Button("ARENAYI OLUŞTUR", GUILayout.Height(50)))
            {
                GenerateArena();
            }
        }
    }

    void GenerateArena()
    {
        if (settings.blueBases == null || settings.redBases == null) return;

        if (GameObject.Find("Generated_Arena")) DestroyImmediate(GameObject.Find("Generated_Arena"));
        mapRoot = new GameObject("Generated_Arena");
        Undo.RegisterCreatedObjectUndo(mapRoot, "Create Arena");

        // --- MERKEZ HESABI (DÜZELTİLDİ) ---
        // Eskiden sadece köprü boyunu hesaplıyorduk, bu yüzden üsler köprünün içine gömülüyordu.
        // Şimdi üslerin kendi boyutunu (Scale) da hesaba katıp onları daha uzağa itiyoruz.
        
        float bridgeTotalLen = settings.bridgeLength * (settings.arenaScale + settings.bridgeGap);
        
        // Üslerin Merkezi Nerede Olmalı?
        // Köprü Boyu / 2 + Üs Yarıçapı
        float zOffset = (bridgeTotalLen / 2f) + (settings.arenaScale); 

        // --- 1. MAVİ TAKIM (SOL / -Z) ---
        GenerateBase(new Vector3(0, 0, -zOffset), false);

        // --- 2. KIRMIZI TAKIM (SAĞ / +Z) ---
        GenerateBase(new Vector3(0, 0, zOffset), true);

        // --- 3. ORTA ADA ---
        if (settings.spawnCenter && settings.centerPlatform != null)
        {
            GameObject centerObj = (GameObject)PrefabUtility.InstantiatePrefab(settings.centerPlatform);
            centerObj.transform.SetParent(mapRoot.transform);
            centerObj.transform.localScale = Vector3.one * settings.arenaScale;
            centerObj.transform.position = Vector3.zero; 
            centerObj.transform.rotation = Quaternion.identity;
        }

        // --- 4. KÖPRÜ ---
        if (!settings.emptyMiddle)
        {
            GenerateBridge(-zOffset, zOffset);
        }

        Selection.activeGameObject = mapRoot;
        SceneView.lastActiveSceneView.FrameSelected();
    }

    void GenerateBase(Vector3 pos, bool isRedTeam)
    {
        List<GameObject> baseList = isRedTeam ? settings.redBases : settings.blueBases;
        List<GameObject> goalList = isRedTeam ? settings.redGoals : settings.blueGoals;
        List<GameObject> connList = isRedTeam ? settings.redConnectors : settings.blueConnectors;

        GameObject basePrefab = GetRandom(baseList);
        if (basePrefab == null) return;

        GameObject baseObj = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
        baseObj.transform.SetParent(mapRoot.transform);
        baseObj.transform.localScale = Vector3.one * settings.arenaScale;
        
        float baseRotY = isRedTeam ? 180 : 0;
        baseObj.transform.rotation = Quaternion.Euler(0, baseRotY, 0);
        baseObj.transform.position = pos;

        // Yön Hesaplamaları (Üssün Arkası)
        Vector3 backDirection = isRedTeam ? Vector3.forward : Vector3.back;
        
        // Üssün arkasına geçmek için merkezden yarım scale kadar gitmeliyiz
        Vector3 currentAttachPoint = pos + (backDirection * (settings.arenaScale * 0.5f));
        // Hafif bir "Gap" payı bırakalım ki z-fighting olmasın (0.01f)
        currentAttachPoint += backDirection * (settings.arenaScale * 0.01f); 

        // ARA BAĞLANTI (CONNECTOR)
        if (settings.fillGoalGap && connList != null && connList.Count > 0)
        {
            GameObject connPrefab = GetRandom(connList);
            GameObject connObj = (GameObject)PrefabUtility.InstantiatePrefab(connPrefab);
            connObj.transform.SetParent(mapRoot.transform);
            connObj.transform.localScale = Vector3.one * settings.arenaScale;
            connObj.transform.rotation = Quaternion.Euler(0, baseRotY, 0);
            
            // Konumlandır (Merkezden merkeze mesafe = Scale/2 + Scale/2 = Scale)
            connObj.transform.position = currentAttachPoint + (backDirection * (settings.arenaScale * 0.5f));
            
            // Uca taşı
            currentAttachPoint += (backDirection * settings.arenaScale); 
        }
        else
        {
            // Tik kapalıysa manuel boşluk bırak
            currentAttachPoint += (backDirection * (settings.arenaScale * 0.5f));
        }

        // KALE (GOAL)
        if (goalList != null && goalList.Count > 0)
        {
            GameObject goalPrefab = GetRandom(goalList);
            GameObject goal = (GameObject)PrefabUtility.InstantiatePrefab(goalPrefab);
            goal.transform.SetParent(mapRoot.transform);
            goal.transform.localScale = Vector3.one * settings.arenaScale;
            
            float finalGoalRot = baseRotY + settings.versusGoalRot;
            goal.transform.rotation = Quaternion.Euler(0, finalGoalRot, 0);

            // Kaleyi yerleştir
            goal.transform.position = currentAttachPoint + (backDirection * (settings.arenaScale * 0.5f));
        }
    }

    void GenerateBridge(float startBaseZ, float endBaseZ)
    {
        // KÖPRÜ BAŞLANGIÇ HESABI (DÜZELTİLDİ)
        // startBaseZ: Mavi Üssün Merkezi.
        // Köprünün ilk parçası, üssün merkezine değil, UCUNA gelmeli.
        // Mavi Üs Ucu = startBaseZ + Scale/2.
        // Köprü Parçası Merkezi = Uç + Scale/2.
        // Yani: startBaseZ + Scale.
        
        float currentZ = startBaseZ + settings.arenaScale;
        
        // Bitiş noktası (Kırmızı Üssün Ucu)
        float endZlimit = endBaseZ - settings.arenaScale;
        
        int safety = 0;

        // Aradaki boşluğu doldurana kadar döngü
        // Hata payı (epsilon) ekledik ki son parça tam sınırdaysa koymamazlık yapmasın
        while (currentZ <= endZlimit + 0.1f && safety < 200)
        {
            GameObject prefab = GetRandom(settings.arenaBridges);
            if (prefab == null) break;

            // Ortaya ada koyduysak ve çok yaklaştıysak atla
            if (settings.spawnCenter && Mathf.Abs(currentZ) < (settings.arenaScale * 0.6f))
            {
                float jumpStep = settings.arenaScale + settings.bridgeGap;
                currentZ += jumpStep;
                continue;
            }

            GameObject segment = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            segment.transform.SetParent(mapRoot.transform);
            segment.transform.localScale = Vector3.one * settings.arenaScale;
            
            float randomX = Random.Range(-settings.scatterAmount, settings.scatterAmount) * settings.arenaScale;
            segment.transform.position = new Vector3(randomX, 0, currentZ);
            segment.transform.rotation = Quaternion.identity;

            if (Random.value > 0.5f) segment.transform.rotation = Quaternion.Euler(0, 180, 0);

            // Bir sonraki parçaya geç
            float step = settings.arenaScale + settings.bridgeGap;
            currentZ += step;
            safety++;
        }
    }

    GameObject GetRandom(List<GameObject> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}