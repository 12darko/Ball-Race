using UnityEditor;
using UnityEngine;

namespace Editor.PrefabCreator.MeshOptimizator
{
    public class ReplaceMeshCollidersWithBox : EditorWindow
    {
        private bool includeChildren = true;
        private int flatVertexThreshold = 20;

        [MenuItem("TOOLS/Mesh Optimizator/Replace MeshColliders With BoxCollider")]
        public static void ShowWindow()
        {
            GetWindow<ReplaceMeshCollidersWithBox>("MeshCol → BoxCol");
        }

        private void OnGUI()
        {
            GUILayout.Label("MeshCollider → Akıllı Collider", EditorStyles.boldLabel);
            GUILayout.Space(5);

            includeChildren = EditorGUILayout.Toggle("Include Children", includeChildren);
            flatVertexThreshold = EditorGUILayout.IntField("Düz Sayılma Vertex Limiti", flatVertexThreshold);

            GUILayout.Space(5);
            EditorGUILayout.HelpBox(
                "Düz/basit meshler (az vertex) → BoxCollider\n" +
                "Köşeli/kompleks meshler (çok vertex) → Convex MeshCollider\n\n" +
                "Vertex limitini değiştirerek ayarlayabilirsin.",
                MessageType.Info);

            GUILayout.Space(10);

            if (GUILayout.Button("Seçili Objelere Uygula", GUILayout.Height(40)))
            {
                ReplaceColliders();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Sadece Convex'e Çevir (Hepsini)", GUILayout.Height(30)))
            {
                SetAllConvex();
            }
        }

        private void ReplaceColliders()
        {
            GameObject[] selected = Selection.gameObjects;

            if (selected.Length == 0)
            {
                EditorUtility.DisplayDialog("Uyarı", "Lütfen Hierarchy'den obje seç.", "Tamam");
                return;
            }

            int boxCount = 0;
            int convexCount = 0;

            foreach (GameObject go in selected)
            {
                MeshCollider[] colliders = includeChildren
                    ? go.GetComponentsInChildren<MeshCollider>()
                    : go.GetComponents<MeshCollider>();

                foreach (MeshCollider mc in colliders)
                {
                    Mesh mesh = mc.sharedMesh;
                    if (mesh == null) continue;

                    bool isSimple = mesh.vertexCount <= flatVertexThreshold;

                    if (isSimple)
                    {
                        // DÜZLER → BOX COLLIDER
                        GameObject obj = mc.gameObject;
                        PhysicsMaterial mat = mc.sharedMaterial;

                        // Mesh'in kendi local space bounds'u — rotation'dan bağımsız, doğru boyut
                        Vector3 localCenter = mesh.bounds.center;
                        Vector3 localSize = mesh.bounds.size;

                        Undo.DestroyObjectImmediate(mc);

                        BoxCollider box = Undo.AddComponent<BoxCollider>(obj);
                        box.center = localCenter;
                        box.size = localSize;

                        if (mat != null)
                            box.sharedMaterial = mat;

                        boxCount++;
                    }
                    else
                    {
                        // KÖŞELİLER / KOMPLEKSLERİ → CONVEX MESH COLLIDER
                        Undo.RecordObject(mc, "Set Convex");
                        mc.convex = true;
                        convexCount++;
                    }
                }
            }

            EditorUtility.DisplayDialog(
                "Tamamlandı",
                $"BoxCollider eklendi: {boxCount} obje\n" +
                $"Convex MeshCollider yapıldı: {convexCount} obje\n\n" +
                "Ctrl+Z ile geri alabilirsin.",
                "Tamam");

            Debug.Log($"[SmartCollider] Box: {boxCount} | Convex: {convexCount}");
        }

        private void SetAllConvex()
        {
            GameObject[] selected = Selection.gameObjects;

            if (selected.Length == 0)
            {
                EditorUtility.DisplayDialog("Uyarı", "Lütfen Hierarchy'den obje seç.", "Tamam");
                return;
            }

            int count = 0;
            foreach (GameObject go in selected)
            {
                MeshCollider[] colliders = includeChildren
                    ? go.GetComponentsInChildren<MeshCollider>()
                    : go.GetComponents<MeshCollider>();

                foreach (MeshCollider mc in colliders)
                {
                    Undo.RecordObject(mc, "Set Convex");
                    mc.convex = true;
                    count++;
                }
            }

            EditorUtility.DisplayDialog("Tamamlandı", $"{count} MeshCollider Convex yapıldı.", "Tamam");
        }
    }
}