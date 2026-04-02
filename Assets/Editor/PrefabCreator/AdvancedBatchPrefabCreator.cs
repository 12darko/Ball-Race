using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class AdvancedBatchPrefabCreator : EditorWindow
{
    List<GameObject> objectList = new List<GameObject>();
    Vector2 scroll;

    // Transform values
    Vector3 customPosition = Vector3.zero;
    Vector3 customRotation = Vector3.zero;
    Vector3 customScale    = Vector3.one;

    bool autoMeshCollider = true;

    [MenuItem("TOOLS/Prefab Creator/🧱 Advanced Batch Prefab Creator")]
    public static void Open()
    {
        GetWindow<AdvancedBatchPrefabCreator>("Advanced Prefab Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("🧱 Batch Prefab Creator (PRO)", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 🔹 Transform Inputs
        GUILayout.Label("⚙️ Transform Değerleri", EditorStyles.boldLabel);
        customPosition = EditorGUILayout.Vector3Field("Position", customPosition);
        customRotation = EditorGUILayout.Vector3Field("Rotation", customRotation);
        customScale    = EditorGUILayout.Vector3Field("Scale", customScale);

        GUILayout.Space(10);

        // 🔹 Collider
        autoMeshCollider = EditorGUILayout.Toggle("Auto Add MeshCollider", autoMeshCollider);

        GUILayout.Space(15);

        // 🔹 Object List
        GUILayout.Label("📦 Prefab Listesi", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("➕ Add Selected"))
        {
            AddSelectedObjects();
        }
        if (GUILayout.Button("🧹 Clear List"))
        {
            objectList.Clear();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(150));
        for (int i = 0; i < objectList.Count; i++)
        {
            GUILayout.BeginHorizontal("box");
            objectList[i] = (GameObject)EditorGUILayout.ObjectField(objectList[i], typeof(GameObject), true);

            if (GUILayout.Button("❌", GUILayout.Width(30)))
            {
                objectList.RemoveAt(i);
                break;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        GUILayout.Space(20);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("🚀 Create Prefabs", GUILayout.Height(40)))
        {
            CreatePrefabs();
        }
        GUI.backgroundColor = Color.white;
    }

    void AddSelectedObjects()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            if (!objectList.Contains(go))
                objectList.Add(go);
        }
    }

    void CreatePrefabs()
    {
        if (objectList.Count == 0)
        {
            EditorUtility.DisplayDialog("Uyarı", "Liste boş reis 😅", "Tamam");
            return;
        }

        string folder = EditorUtility.OpenFolderPanel("Prefab Kayıt Klasörü", "Assets", "");
        if (string.IsNullOrEmpty(folder)) return;

        if (!folder.StartsWith(Application.dataPath))
        {
            EditorUtility.DisplayDialog("Hata", "Assets içinden seçmelisin!", "Tamam");
            return;
        }

        string assetPath = "Assets" + folder.Substring(Application.dataPath.Length);

        int count = 0;

        foreach (GameObject original in objectList)
        {
            if (original == null) continue;

            GameObject temp = Instantiate(original);
            temp.name = original.name;

            // 🎯 Custom Transform
            temp.transform.position   = customPosition;
            temp.transform.rotation   = Quaternion.Euler(customRotation);
            temp.transform.localScale = customScale;

            // 🧱 MeshCollider
            if (autoMeshCollider)
                AddMeshCollidersRecursive(temp);

            string prefabPath = AssetDatabase.GenerateUniqueAssetPath(
                Path.Combine(assetPath, original.name + ".prefab")
            );

            PrefabUtility.SaveAsPrefabAsset(temp, prefabPath);
            DestroyImmediate(temp);

            count++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Bitti kral 👑",
            $"{count} adet prefab oluşturuldu.",
            "Eyvallah"
        );
    }

    void AddMeshCollidersRecursive(GameObject root)
    {
        MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>(true);

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;

            GameObject go = mf.gameObject;

            if (go.GetComponent<Collider>() != null) continue;

            MeshCollider mc = go.AddComponent<MeshCollider>();
            mc.sharedMesh = mf.sharedMesh;
            mc.convex = false;
        }
    }
}
