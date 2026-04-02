using UnityEngine;
using UnityEditor;
using Main.Scripts.Player.Database;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace _Main.Scripts.Multiplayer.Multiplayer.Customize.Editor
{
    public class NPCSpawnerEditorWindow : EditorWindow
    {
        private CosmeticDatabase cosmeticDB;
        private GameObject lobbyRootPrefab;
        private Transform spawnPoint;
        private Vector3 randomOffset = new Vector3(2f, 0f, 2f);
        
        private Vector3 faceRotation = new Vector3(-90, 0, 0);
        private Vector3 faceLocalPosition = new Vector3(0, 0, 0f);
        private float hatYOffset = -0.15f; // ✅ Yeni ekleme
        private List<GameObject> spawnedNPCs = new List<GameObject>();
        
        private int previewBallIndex = 0;
        private int previewHatIndex = 0;
        private int previewFaceIndex = 0;
        
        private Vector2 scrollPosition;

        [MenuItem("TOOLS/Prefab Creator/NPC Ball Spawner")]
        public static void ShowWindow()
        {
            NPCSpawnerEditorWindow window = GetWindow<NPCSpawnerEditorWindow>("NPC Ball Spawner");
            window.minSize = new Vector2(400, 500);
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            GUILayout.Label("NPC Ball Spawner", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
            cosmeticDB = (CosmeticDatabase)EditorGUILayout.ObjectField("Cosmetic Database", cosmeticDB, typeof(CosmeticDatabase), false);
            lobbyRootPrefab = (GameObject)EditorGUILayout.ObjectField("Lobby Root Prefab", lobbyRootPrefab, typeof(GameObject), false);
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Spawn Settings", EditorStyles.boldLabel);
            spawnPoint = (Transform)EditorGUILayout.ObjectField("Spawn Point", spawnPoint, typeof(Transform), true);
            
            if (spawnPoint == null)
            {
                EditorGUILayout.HelpBox("Spawn Point boş! Scene view'in ortasında spawn olacak.", MessageType.Info);
            }
            
            randomOffset = EditorGUILayout.Vector3Field("Random Offset", randomOffset);
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Face Settings", EditorStyles.boldLabel);
            faceRotation = EditorGUILayout.Vector3Field("Face Local Rotation", faceRotation);
            faceLocalPosition = EditorGUILayout.Vector3Field("Face Local Position", faceLocalPosition);
            hatYOffset = EditorGUILayout.FloatField("Hat Y Offset (Global)", hatYOffset); // ✅ Yeni

            EditorGUILayout.Space(10);
            
            if (cosmeticDB != null)
            {
                EditorGUILayout.LabelField("Preview Specific Cosmetics", EditorStyles.boldLabel);
                
                if (cosmeticDB.allBalls != null && cosmeticDB.allBalls.Count > 0)
                {
                    string[] ballNames = new string[cosmeticDB.allBalls.Count];
                    for (int i = 0; i < cosmeticDB.allBalls.Count; i++)
                        ballNames[i] = $"{i}: {cosmeticDB.allBalls[i].name}";
                    
                    previewBallIndex = EditorGUILayout.Popup("Ball", previewBallIndex, ballNames);
                }
                
                if (cosmeticDB.allHats != null && cosmeticDB.allHats.Count > 0)
                {
                    string[] hatNames = new string[cosmeticDB.allHats.Count];
                    for (int i = 0; i < cosmeticDB.allHats.Count; i++)
                        hatNames[i] = $"{i}: {cosmeticDB.allHats[i].name}";
                    
                    previewHatIndex = EditorGUILayout.Popup("Hat", previewHatIndex, hatNames);
                }
                
                if (cosmeticDB.allFaces != null && cosmeticDB.allFaces.Count > 0)
                {
                    string[] faceNames = new string[cosmeticDB.allFaces.Count];
                    for (int i = 0; i < cosmeticDB.allFaces.Count; i++)
                        faceNames[i] = $"{i}: {cosmeticDB.allFaces[i].name}";
                    
                    previewFaceIndex = EditorGUILayout.Popup("Face", previewFaceIndex, faceNames);
                }
            }
            
            EditorGUILayout.Space(15);
            
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Spawn Random NPC Ball", GUILayout.Height(40)))
            {
                SpawnRandomNPC();
            }
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Spawn Specific NPC Ball", GUILayout.Height(40)))
            {
                SpawnSpecificNPC(previewBallIndex, previewHatIndex, previewFaceIndex);
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField($"Spawned NPCs: {spawnedNPCs.Count}", EditorStyles.helpBox);
            
            EditorGUILayout.Space(5);
            
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Clear All NPCs", GUILayout.Height(30)))
            {
                ClearAllNPCs();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "1. CosmeticDatabase ve Lobby Root Prefab'ı ata\n" +
                "2. Database'deki her item için 'CustomizeItemPrefabLobby' alanını doldur\n" +
                "3. 'Spawn Random' veya 'Spawn Specific' butonuna bas\n" +
                "4. NPC'ler scene'de oluşur", 
                MessageType.Info
            );
            
            EditorGUILayout.EndScrollView();
        }

        private void SpawnRandomNPC()
        {
            if (!ValidateSettings()) return;
            
            int ballIndex = Random.Range(0, cosmeticDB.allBalls.Count);
            int hatIndex = Random.Range(0, cosmeticDB.allHats.Count);
            int faceIndex = Random.Range(0, cosmeticDB.allFaces.Count);
            
            SpawnNPC(ballIndex, hatIndex, faceIndex);
        }

        private void SpawnSpecificNPC(int ballIndex, int hatIndex, int faceIndex)
        {
            if (!ValidateSettings()) return;
            SpawnNPC(ballIndex, hatIndex, faceIndex);
        }

        private void SpawnNPC(int ballIndex, int hatIndex, int faceIndex)
        {
            Debug.Log($"[NPCSpawner] Spawning NPC - Ball:{ballIndex}, Hat:{hatIndex}, Face:{faceIndex}");

            Vector3 spawnPos = GetSpawnPosition();

            // ✅ 1. Lobby Root Prefab spawn
            GameObject rootObj = PrefabUtility.InstantiatePrefab(lobbyRootPrefab) as GameObject;
            rootObj.transform.position = spawnPos;
            rootObj.transform.rotation = Quaternion.identity;
            rootObj.name = $"NPC_Root_{spawnedNPCs.Count}";
            
            Undo.RegisterCreatedObjectUndo(rootObj, "Spawn NPC Root");
            spawnedNPCs.Add(rootObj);

            // ✅ 2. BallRoot bul
            Transform ballRoot = FindDeepChild(rootObj.transform, "BallRoot");
            if (ballRoot == null)
            {
                Debug.LogError("[NPCSpawner] Lobby Root içinde 'BallRoot' bulunamadı!");
                return;
            }

            // ✅ 3. Ball spawn (CustomizeItemPrefabLobby - GameObject)
            if (cosmeticDB.allBalls == null || ballIndex >= cosmeticDB.allBalls.Count)
            {
                Debug.LogError("[NPCSpawner] Ball index geçersiz!");
                return;
            }

            var ballItem = cosmeticDB.allBalls[ballIndex];
            GameObject ballPrefab = ballItem.CustomizeItemPrefabLobby; // ✅ GameObject

            if (ballPrefab == null)
            {
                Debug.LogError($"[NPCSpawner] Ball 'CustomizeItemPrefabLobby' boş! Index: {ballIndex}, Item: {ballItem.name}");
                return;
            }

            GameObject ballObj = PrefabUtility.InstantiatePrefab(ballPrefab) as GameObject;
            ballObj.transform.SetParent(ballRoot);
            ballObj.transform.localPosition = Vector3.zero;
            ballObj.transform.localRotation = Quaternion.identity;
            ballObj.transform.localScale = Vector3.one;
            ballObj.name = $"Ball_{ballItem.name}";
            
            Undo.RegisterCreatedObjectUndo(ballObj, "Spawn Ball");

            // ✅ 4. Hat Parent ve Face Parent bul
            Transform hatParent = FindDeepChild(ballObj.transform, "Hat Parent");
            Transform faceParent = FindDeepChild(ballObj.transform, "Face Parent");

            if (hatParent == null || faceParent == null)
            {
                Debug.LogError("[NPCSpawner] Ball prefab içinde 'Hat Parent' veya 'Face Parent' bulunamadı!");
                return;
            }

            // ✅ 5. Hat spawn (CustomizeItemPrefabLobby - GameObject)
            if (cosmeticDB.allHats != null && hatIndex < cosmeticDB.allHats.Count)
            {
                var hatItem = cosmeticDB.allHats[hatIndex];
                GameObject hatPrefab = hatItem.CustomizeItemPrefabLobby; // ✅ GameObject

                if (hatPrefab != null)
                {
                    GameObject hatObj = PrefabUtility.InstantiatePrefab(hatPrefab) as GameObject;
                    hatObj.transform.SetParent(hatParent);
                    hatObj.transform.localPosition = new Vector3(0, hatItem.CustomizeInGameSpawnYOffset + hatYOffset, 0);
                    hatObj.transform.localRotation = Quaternion.identity;
                    hatObj.name = $"Hat_{hatItem.name}";
                    
                    Undo.RegisterCreatedObjectUndo(hatObj, "Spawn Hat");
                    Debug.Log($"[NPCSpawner] Hat spawned: {hatItem.name}");
                }
                else
                {
                    Debug.LogWarning($"[NPCSpawner] Hat 'CustomizeItemPrefabLobby' boş! Index: {hatIndex}");
                }
            }

            // ✅ 6. Face spawn (CustomizeItemPrefabLobby - GameObject)
            if (cosmeticDB.allFaces != null && faceIndex < cosmeticDB.allFaces.Count)
            {
                var faceItem = cosmeticDB.allFaces[faceIndex];
                GameObject facePrefab = faceItem.CustomizeItemPrefabLobby;

                if (facePrefab != null)
                {
                    GameObject faceObj = PrefabUtility.InstantiatePrefab(facePrefab) as GameObject;
                    faceObj.transform.SetParent(faceParent);
                    faceObj.transform.localPosition = faceLocalPosition;
                    faceObj.transform.localRotation = Quaternion.identity; // ✅ Önce identity yap
                    faceObj.transform.localEulerAngles = faceRotation;      // ✅ Sonra euler angles uygula
                    faceObj.name = $"Face_{faceItem.name}";
        
                    Undo.RegisterCreatedObjectUndo(faceObj, "Spawn Face");
                    Debug.Log($"[NPCSpawner] Face spawned: {faceItem.name}");
                }
            }

            Debug.Log($"[NPCSpawner] NPC spawned successfully!");
            
            EditorUtility.SetDirty(rootObj);
            Selection.activeGameObject = rootObj;
        }

        private Vector3 GetSpawnPosition()
        {
            Vector3 basePos = Vector3.zero;

            if (spawnPoint != null)
            {
                basePos = spawnPoint.position;
            }
            else
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                if (sceneView != null)
                    basePos = sceneView.pivot;
            }

            return basePos + new Vector3(
                Random.Range(-randomOffset.x, randomOffset.x),
                Random.Range(-randomOffset.y, randomOffset.y),
                Random.Range(-randomOffset.z, randomOffset.z)
            );
        }

        private bool ValidateSettings()
        {
            if (cosmeticDB == null)
            {
                EditorUtility.DisplayDialog("Error", "CosmeticDatabase atanmamış!", "OK");
                return false;
            }

            if (lobbyRootPrefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Lobby Root Prefab atanmamış!", "OK");
                return false;
            }

            return true;
        }

        private void ClearAllNPCs()
        {
            foreach (var npc in spawnedNPCs)
            {
                if (npc != null)
                    Undo.DestroyObjectImmediate(npc);
            }

            spawnedNPCs.Clear();
            Debug.Log("[NPCSpawner] All NPCs cleared!");
        }

        private Transform FindDeepChild(Transform parent, string name)
        {
            if (parent == null) return null;

            for (int i = 0; i < parent.childCount; i++)
            {
                var c = parent.GetChild(i);
                if (c.name == name) return c;

                var r = FindDeepChild(c, name);
                if (r != null) return r;
            }

            return null;
        }
    }
}
#endif