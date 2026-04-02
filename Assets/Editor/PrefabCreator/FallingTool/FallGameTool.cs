using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FallGameTool : EditorWindow
{
    [MenuItem("TOOLS/Fall Game/Fall Game Creator")]
    public static void ShowWindow()
    {
        GetWindow<FallGameTool>("Fall Game Panel");
    }

    public enum MapShape { Dikdortgen, Daire, Elmas, Altigen }
    public enum FactoryPreset { Ozel_Bos, Kaos_Fizik, Sabit_Parkour, Donen_Olum, Renkli_Paket }
    public enum ColorTheme { Mavi, Kirmizi, Yesil, Sari, Karisik_Random }

    public FallGameData data; 
    
    private int selectedTab = 0;
    private string[] tabs = { "Parkour Generator", "Module Factory", "Hex Map" };
    private Vector2 scrollPos;
    
    public FactoryPreset currentPreset = FactoryPreset.Ozel_Bos;
    public ColorTheme currentColorTheme = ColorTheme.Mavi;

    SerializedObject so;
    SerializedObject soData; 

    void OnEnable()
    {
        so = new SerializedObject(this);
        if (data == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:FallGameData");
            if (guids.Length > 0) data = AssetDatabase.LoadAssetAtPath<FallGameData>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }

    void OnGUI()
    {
        so.Update();
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 14, alignment = TextAnchor.MiddleCenter, fixedHeight = 30 };
        GUILayout.Space(10);
        GUILayout.Label("FALL GUYS CREATOR (ULTIMATE)", titleStyle);

        if (data == null)
        {
            EditorGUILayout.HelpBox("Data dosyası bulunamadı!", MessageType.Warning);
            if (GUILayout.Button("Data Oluştur")) CreateDataFile();
            return;
        }

        soData = new SerializedObject(data);
        soData.Update();

        selectedTab = GUILayout.Toolbar(selectedTab, tabs, GUILayout.Height(30));
        GUILayout.Space(10);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        switch (selectedTab)
        {
            case 0: DrawParkourTab(); break;
            case 1: DrawFactoryTab(); break;
            case 2: DrawHexTab(); break;
        }

        EditorGUILayout.EndScrollView();
        soData.ApplyModifiedProperties();
        so.ApplyModifiedProperties();
    }

    // --- 1. PARKUR SEKMESİ ---
    // --- 1. PARKUR SEKMESİ (GÜNCELLENDİ) ---
    void DrawParkourTab()
    {
        GUILayout.Label("🏁 PARKUR AYARLARI", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(soData.FindProperty("levelLength"), new GUIContent("Parkur Uzunluğu (İleri)"));
        
        // --- YENİ ŞERİT AYARLARI ---
        EditorGUILayout.PropertyField(soData.FindProperty("laneCount"), new GUIContent("Şerit Sayısı (Genişlik)"));
        EditorGUILayout.PropertyField(soData.FindProperty("laneSpacing"), new GUIContent("Şerit Aralığı"));
        
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(soData.FindProperty("startModule"), new GUIContent("Başlangıç (Start)"));
        EditorGUILayout.PropertyField(soData.FindProperty("endModule"), new GUIContent("Bitiş (End)"));
        
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(soData.FindProperty("parkourModules"), new GUIContent("Modül Listesi"), true);
        
        GUILayout.Space(20);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("PARKURU OLUŞTUR", GUILayout.Height(40))) GenerateParkour();
        GUI.backgroundColor = Color.white;
    }

    // --- 2. FABRİKA SEKMESİ ---
    void DrawFactoryTab()
    {
        GUILayout.Label("🏭 MODÜL FABRİKASI", EditorStyles.boldLabel);
        GUILayout.BeginVertical(EditorStyles.helpBox);
        
        currentPreset = (FactoryPreset)EditorGUILayout.EnumPopup("Çalışma Modu:", currentPreset);

        if (currentPreset == FactoryPreset.Renkli_Paket)
        {
            GUILayout.Space(5);
            GUI.backgroundColor = Color.cyan;
            currentColorTheme = (ColorTheme)EditorGUILayout.EnumPopup("🎨 Hangi Renk Olsun?", currentColorTheme);
            GUI.backgroundColor = Color.white;
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(10);

        SerializedProperty currentObstacles = null;
        SerializedProperty currentFloors = null;

        // --- LİSTE SEÇİM EKRANI ---
        switch (currentPreset)
        {
            case FactoryPreset.Ozel_Bos:
                GUILayout.Label("🛠️ ÖZEL MOD", EditorStyles.miniBoldLabel);
                currentObstacles = soData.FindProperty("rawObstacles");
                currentFloors = soData.FindProperty("factoryFloors");
                break;
            case FactoryPreset.Kaos_Fizik:
                GUILayout.Label("🔥 KAOS MODU", EditorStyles.miniBoldLabel);
                currentObstacles = soData.FindProperty("presetChaos");
                currentFloors = soData.FindProperty("presetChaosFloors");
                data.factoryAddPhysics = true; 
                break;
            case FactoryPreset.Sabit_Parkour:
                GUILayout.Label("🧱 SABİT PARKUR", EditorStyles.miniBoldLabel);
                currentObstacles = soData.FindProperty("presetStatic");
                currentFloors = soData.FindProperty("presetStaticFloors");
                data.factoryAddPhysics = false;
                break;
            case FactoryPreset.Donen_Olum:
                GUILayout.Label("⚔️ DÖNEN ÖLÜM", EditorStyles.miniBoldLabel);
                currentObstacles = soData.FindProperty("presetRotating");
                currentFloors = soData.FindProperty("presetRotatingFloors");
                data.factoryAddPhysics = false;
                break;
            case FactoryPreset.Renkli_Paket:
                data.factoryAddPhysics = false;
                switch (currentColorTheme)
                {
                    case ColorTheme.Mavi:
                        GUILayout.Label("🔵 MAVİ LİSTELER", EditorStyles.miniBoldLabel);
                        currentObstacles = soData.FindProperty("blueObstacles");
                        currentFloors = soData.FindProperty("blueFloors");
                        break;
                    case ColorTheme.Kirmizi:
                        GUILayout.Label("🔴 KIRMIZI LİSTELER", EditorStyles.miniBoldLabel);
                        currentObstacles = soData.FindProperty("redObstacles");
                        currentFloors = soData.FindProperty("redFloors");
                        break;
                    case ColorTheme.Yesil:
                        GUILayout.Label("🟢 YEŞİL LİSTELER", EditorStyles.miniBoldLabel);
                        currentObstacles = soData.FindProperty("greenObstacles");
                        currentFloors = soData.FindProperty("greenFloors");
                        break;
                    case ColorTheme.Sari:
                        GUILayout.Label("🟡 SARI LİSTELER", EditorStyles.miniBoldLabel);
                        currentObstacles = soData.FindProperty("yellowObstacles");
                        currentFloors = soData.FindProperty("yellowFloors");
                        break;
                    case ColorTheme.Karisik_Random:
                        GUILayout.Label("🌈 KARIŞIK MOD (Rastgele Renkler)", EditorStyles.miniBoldLabel);
                        break;
                }
                break;
        }

        // Listeleri Göster (Karışık mod hariç)
        if (currentColorTheme != ColorTheme.Karisik_Random || currentPreset != FactoryPreset.Renkli_Paket)
        {
            if (currentFloors != null) EditorGUILayout.PropertyField(currentFloors, new GUIContent("Zemin Listesi"), true);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(soData.FindProperty("factoryRandomFloor"), new GUIContent("Rastgele Zemin Seç"));
            if (!data.factoryRandomFloor && currentFloors != null && currentFloors.arraySize > 0)
            {
                 int maxID = Mathf.Max(0, currentFloors.arraySize - 1);
                 data.factoryFloorID = EditorGUILayout.IntSlider("Zemin ID:", data.factoryFloorID, 0, maxID);
            }
            GUILayout.Space(10);
            if (currentObstacles != null) { GUI.color = Color.cyan; EditorGUILayout.PropertyField(currentObstacles, new GUIContent("Engel Listesi"), true); GUI.color = Color.white; }
        }

        GUILayout.Space(10);
        EditorGUILayout.PropertyField(soData.FindProperty("factorySpacing"), new GUIContent("Boşluk"));
        EditorGUILayout.PropertyField(soData.FindProperty("factoryAddPhysics"), new GUIContent("Fizik Ekle (Auto Convex)"));
        
        GUILayout.Space(20);
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("MODÜLLERİ ÜRET", GUILayout.Height(40))) CreateFactoryModules();
        GUI.backgroundColor = Color.white;
    }

    // --- 3. HEX MAP SEKMESİ ---
    void DrawHexTab()
    {
        GUILayout.Label("🛑 HEX-A-GONE", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(soData.FindProperty("hexTiles"), new GUIContent("Renk Listesi"), true);
        GUILayout.Space(5);
        EditorGUILayout.HelpBox("1x1 Kareler: 1.1\n2x2 Kareler: 2.2", MessageType.Info);
        EditorGUILayout.PropertyField(soData.FindProperty("hexSpacing"), new GUIContent("Kare Aralığı"));
        GUILayout.Space(10);
        data.currentShape = (MapShape)EditorGUILayout.EnumPopup("Şekil:", data.currentShape);
        GUILayout.Space(5);
        if (data.currentShape == MapShape.Dikdortgen)
        {
            EditorGUILayout.PropertyField(soData.FindProperty("hexWidth"), new GUIContent("Genişlik"));
            EditorGUILayout.PropertyField(soData.FindProperty("hexDepth"), new GUIContent("Derinlik"));
        }
        else 
        {
            EditorGUILayout.PropertyField(soData.FindProperty("mapRadius"), new GUIContent("Yarıçap"));
        }
        EditorGUILayout.PropertyField(soData.FindProperty("hexHoneycomb"), new GUIContent("Petek Düzeni"));
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(soData.FindProperty("hexRandomColors"), new GUIContent("Rengarenk Yap"));
        if (!data.hexRandomColors)
        {
            int maxID = Mathf.Max(0, data.hexTiles.Count - 1);
            data.hexSelectedID = EditorGUILayout.IntSlider("Renk ID:", data.hexSelectedID, 0, maxID);
        }
        GUILayout.Space(20);
        GUI.backgroundColor = Color.magenta;
        if (GUILayout.Button("HEX MAP OLUŞTUR", GUILayout.Height(40))) GenerateHexMap();
        GUI.backgroundColor = Color.white;
    }

    // ----------------------------------------------------
    // --- BURASI SİHİRLİ BÖLÜM: ÜRETİM FONKSİYONLARI ---
    // ----------------------------------------------------

    void CreateFactoryModules()
    {
        Transform parent = GetCleanParent("--- MODULE FACTORY ---");
        float currentX = 0;
        List<GameObject> targetObstacles = new List<GameObject>();
        List<GameObject> targetFloors = new List<GameObject>();

        // Karışık mod mu?
        bool isMixedMode = (currentPreset == FactoryPreset.Renkli_Paket && currentColorTheme == ColorTheme.Karisik_Random);

        // --- LİSTE SEÇİM MANTIĞI ---
        if (!isMixedMode)
        {
            switch (currentPreset)
            {
                case FactoryPreset.Ozel_Bos: targetObstacles = data.rawObstacles; targetFloors = data.factoryFloors; break;
                case FactoryPreset.Kaos_Fizik: targetObstacles = data.presetChaos; targetFloors = data.presetChaosFloors; break;
                case FactoryPreset.Sabit_Parkour: targetObstacles = data.presetStatic; targetFloors = data.presetStaticFloors; break;
                case FactoryPreset.Donen_Olum: targetObstacles = data.presetRotating; targetFloors = data.presetRotatingFloors; break;
                case FactoryPreset.Renkli_Paket:
                    switch (currentColorTheme)
                    {
                        case ColorTheme.Mavi: targetObstacles = data.blueObstacles; targetFloors = data.blueFloors; break;
                        case ColorTheme.Kirmizi: targetObstacles = data.redObstacles; targetFloors = data.redFloors; break;
                        case ColorTheme.Yesil: targetObstacles = data.greenObstacles; targetFloors = data.greenFloors; break;
                        case ColorTheme.Sari: targetObstacles = data.yellowObstacles; targetFloors = data.yellowFloors; break;
                    }
                    break;
            }
            if (targetFloors.Count == 0 || targetObstacles.Count == 0) { Debug.LogError("Liste boş reis! Doldurmayı unutma."); return; }
        }
        else
        {
            // Karışık mod için sahte liste
            for(int i=0; i<10; i++) targetObstacles.Add(null); 
        }

        // --- ÜRETİM DÖNGÜSÜ ---
        foreach (GameObject loopItem in targetObstacles)
        {
            GameObject currentObstaclePrefab = loopItem;
            GameObject currentFloorPrefab = null;

            // --- KARIŞIK RENK SEÇİMİ ---
            if (isMixedMode)
            {
                int rndColor = Random.Range(0, 4); // 0:Mavi, 1:Kirmizi, 2:Yesil, 3:Sari
                List<GameObject> tempObs = null; 
                List<GameObject> tempFloor = null;

                switch (rndColor) {
                    case 0: tempObs = data.blueObstacles; tempFloor = data.blueFloors; break;
                    case 1: tempObs = data.redObstacles; tempFloor = data.redFloors; break;
                    case 2: tempObs = data.greenObstacles; tempFloor = data.greenFloors; break;
                    case 3: tempObs = data.yellowObstacles; tempFloor = data.yellowFloors; break;
                }

                if (tempObs != null && tempObs.Count > 0) currentObstaclePrefab = tempObs[Random.Range(0, tempObs.Count)];
                if (tempFloor != null && tempFloor.Count > 0) currentFloorPrefab = tempFloor[0];
            }
            else
            {
                // Normal mod zemin seçimi
                if (targetFloors.Count > 0)
                    currentFloorPrefab = data.factoryRandomFloor ? targetFloors[Random.Range(0, targetFloors.Count)] : targetFloors[Mathf.Clamp(data.factoryFloorID, 0, targetFloors.Count - 1)];
            }

            if (currentObstaclePrefab == null) continue;

            // --- BENZERSİZ İSİM (UNIQUE ID) ---
            string uniqueID = Random.Range(1000, 9999).ToString();
            GameObject grp = new GameObject("Module_" + currentObstaclePrefab.name + "_" + uniqueID);
            
            grp.transform.parent = parent;
            grp.transform.position = new Vector3(currentX, 0, 0);
            Undo.RegisterCreatedObjectUndo(grp, "Create Module");

            // Zemini Oluştur
            if (currentFloorPrefab) 
            { 
                GameObject f = (GameObject)PrefabUtility.InstantiatePrefab(currentFloorPrefab, grp.transform); 
                f.transform.localPosition = Vector3.zero; 
            }

            // Engeli Oluştur
            GameObject o;
            if (PrefabUtility.IsPartOfPrefabAsset(currentObstaclePrefab)) 
                o = (GameObject)PrefabUtility.InstantiatePrefab(currentObstaclePrefab, grp.transform);
            else 
                o = Instantiate(currentObstaclePrefab, grp.transform);
            
            o.transform.localPosition = Vector3.up * 0.5f;

            // --- PERVANE (BEAM) MANTIĞI: + ŞEKLİ ---
            if (o.name.Contains("Beam"))
            {
                o.transform.localPosition = Vector3.up * 1.3f; // Havaya kaldır
                
                // İkinci bıçağı oluştur
                GameObject blade2 = Instantiate(currentObstaclePrefab, o.transform);
                blade2.transform.localPosition = Vector3.zero;
                blade2.transform.localRotation = Quaternion.Euler(0, 90, 0);
                
                // Kopyanın Rigidbody'sini temizle
                if(blade2.GetComponent<Rigidbody>()) DestroyImmediate(blade2.GetComponent<Rigidbody>());

                // Rotator Scripti Varsa Ekle
                if(o.GetComponent<RotatingPlatform>() == null) 
                {
                    // Not: Projende Rotator.cs scripti varsa bu çalışır. Yoksa hata vermez ama dönmez.
                    o.AddComponent(System.Type.GetType("RotatingPlatform")); 
                }
            }

            // --- FİZİK VE CONVEX FIX ---
            if (data.factoryAddPhysics && (o.name.Contains("Barrel") || o.name.Contains("Box") || o.name.Contains("Cone") || o.name.Contains("Can") || o.name.Contains("Sphere")))
            {
                if (!o.GetComponent<Rigidbody>()) o.AddComponent<Rigidbody>();
                // MeshCollider'ları bul ve Convex yap!
                MeshCollider[] colliders = o.GetComponentsInChildren<MeshCollider>();
                foreach (MeshCollider mc in colliders) mc.convex = true;
            }
            
            // Dönen Sütunlar
            if (o.name.Contains("Pillar")) 
            {
                o.transform.localPosition = Vector3.up * 1.5f;
                if(o.GetComponent<RotatingPlatform>() == null) o.AddComponent(System.Type.GetType("RotatingPlatform"));
            }

            GameObject exit = new GameObject("ExitPoint"); 
            exit.transform.parent = grp.transform; 
            exit.transform.localPosition = Vector3.forward * 4.0f; 
            
            currentX += data.factorySpacing;
        }
    }
// --- 🧹 TEMİZLİKÇİ FONKSİYON (Eksik Olan Buydu!) ---
 
    // --- DİĞER GENERATOR FONKSİYONLARI ---
     // --- GENERATOR FONKSİYONU (ARTIK YAN YANA DİZİYOR) ---
     // --- 🛠️ ANA OLUŞTURMA FONKSİYONU ---
    void GenerateParkour()
    {
        // Önce eski parkuru temizle
        Transform parent = GetCleanParent("--- GENERATED PARKUR ---");
        Vector3 currentCenterPos = Vector3.zero;
        
        // Şeritlerin ortalanması için offset hesabı
        float startOffset = -((data.laneCount - 1) * data.laneSpacing) / 2f;

        // 📏 OTOMATİK GENİŞLETME (STRETCH) HESABI
        // KayKit zeminleri standart 4 birimdir. Şerit aralığına göre genişletiyoruz.
        float stretchFactor = data.laneSpacing / 4.0f; 
        if (stretchFactor < 1f) stretchFactor = 1f; // Küçültme yapmasın

        // ----------------------------------------------------
        // 🟢 1. MODÜLER START
        // ----------------------------------------------------
        if (data.startModule) 
        {
            for (int lane = 0; lane < data.laneCount; lane++)
            {
                float xPos = startOffset + (lane * data.laneSpacing);
                Vector3 spawnPos = currentCenterPos + new Vector3(xPos, 0, 0);

                GameObject s = (GameObject)PrefabUtility.InstantiatePrefab(data.startModule, parent);
                s.transform.position = spawnPos;
                
                // Aradaki boşluğu kapatmak için genişlet
                s.transform.localScale = new Vector3(stretchFactor, 1, 1);
            }
            currentCenterPos += Vector3.forward * 4; 
        }

        // ----------------------------------------------------
        // 🧱 2. ENGEL MODÜLLERİ (Rastgele)
        // ----------------------------------------------------
        for (int i = 0; i < data.levelLength; i++)
        {
            // Eğer liste boşsa döngüyü kır
            if (data.parkourModules == null || data.parkourModules.Count == 0) break;

            for (int lane = 0; lane < data.laneCount; lane++)
            {
                // Rastgele modül seç
                GameObject modPrefab = data.parkourModules[Random.Range(0, data.parkourModules.Count)];
                if (modPrefab == null) continue;

                float xPos = startOffset + (lane * data.laneSpacing);
                Vector3 spawnPos = currentCenterPos + new Vector3(xPos, 0, 0);

                GameObject m = (GameObject)PrefabUtility.InstantiatePrefab(modPrefab, parent);
                m.transform.position = spawnPos;
            }
            currentCenterPos += Vector3.forward * 4.5f; // Bir sonraki sıraya geç
        }

        // ----------------------------------------------------
        // 🏁 3. MODÜLER FINISH
        // ----------------------------------------------------
 
        if (data.endModule) 
        {
            for (int lane = 0; lane < data.laneCount; lane++)
            {
                float xPos = startOffset + (lane * data.laneSpacing);
                Vector3 spawnPos = currentCenterPos + new Vector3(xPos, 0, 0);

                GameObject e = (GameObject)PrefabUtility.InstantiatePrefab(data.endModule, parent);
                e.transform.position = spawnPos;

                // Aradaki boşluğu kapatmak için genişlet
                e.transform.localScale = new Vector3(stretchFactor, 1, 1);
            }
        }
    }

    // Ufak bir yardımcı (Kenara süs koymak için)
    void SpawnProp(GameObject parentModule, string name, float xOffset)
    {
        // Buraya istersen otomatik koni/bariyer koyma kodu ekleyebilirsin
        // Şimdilik boş kalsın, kafa karıştırmasın.
    }
    void GenerateHexMap() 
    { 
        Transform parent = GetCleanParent("--- HEX MAP ---"); 
        float spacing = data.hexSpacing; 
        if (data.hexTiles.Count == 0) return; 
        int xMin = 0, xMax = data.hexWidth, zMin = 0, zMax = data.hexDepth; 
        if (data.currentShape != MapShape.Dikdortgen) { xMin = -data.mapRadius; xMax = data.mapRadius; zMin = -data.mapRadius; zMax = data.mapRadius; } 
        for (int z = zMin; z <= zMax; z++) { for (int x = xMin; x <= xMax; x++) { float xPos = x * spacing; float zPos = z * spacing; if (data.hexHoneycomb && z % 2 != 0) xPos += spacing / 2f; Vector3 pos = new Vector3(xPos, 0, zPos); bool shouldSpawn = false; switch (data.currentShape) { case MapShape.Dikdortgen: if (x >= 0 && x < data.hexWidth && z >= 0 && z < data.hexDepth) shouldSpawn = true; break; case MapShape.Daire: if (pos.magnitude <= (data.mapRadius * spacing)) shouldSpawn = true; break; case MapShape.Elmas: if (Mathf.Abs(x) + Mathf.Abs(z) <= data.mapRadius) shouldSpawn = true; break; case MapShape.Altigen: int q = x - (z - (z & 1)) / 2; int r = z; if ((Mathf.Abs(q) + Mathf.Abs(q + r) + Mathf.Abs(r)) / 2 <= data.mapRadius) shouldSpawn = true; break; } if (shouldSpawn) { GameObject selectedTile = data.hexRandomColors ? data.hexTiles[Random.Range(0, data.hexTiles.Count)] : data.hexTiles[Mathf.Clamp(data.hexSelectedID, 0, data.hexTiles.Count - 1)]; if (selectedTile) { GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(selectedTile, parent); tile.transform.position = pos; Undo.RegisterCreatedObjectUndo(tile, "Create Tile"); } } } } }
    
    // --- HELPER FONKSİYONLAR ---
    void CreateDataFile() { FallGameData newData = ScriptableObject.CreateInstance<FallGameData>(); AssetDatabase.CreateAsset(newData, "Assets/MyFallGameData.asset"); AssetDatabase.SaveAssets(); data = newData; }
    Transform GetCleanParent(string name) { GameObject existing = GameObject.Find(name); if (existing) Undo.DestroyObjectImmediate(existing); GameObject newParent = new GameObject(name); Undo.RegisterCreatedObjectUndo(newParent, "Create Parent"); return newParent.transform; }
    Vector3 GetExitPoint(GameObject obj, Vector3 fallback) { LevelModule lm = obj.GetComponent<LevelModule>(); if (lm != null && lm.exitPoint != null) return lm.exitPoint.position; Transform exit = obj.transform.Find("ExitPoint"); if (exit != null) return exit.position; return fallback; }
}