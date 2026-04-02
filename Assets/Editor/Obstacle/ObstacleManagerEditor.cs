#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObstacleManager))]
public class ObstacleManagerEditor : UnityEditor.Editor
{
    private SerializedProperty _entries;
    private SerializedProperty _maxActive;

    private void OnEnable()
    {
        _entries   = serializedObject.FindProperty("obstacleEntries");
        _maxActive = serializedObject.FindProperty("maxActiveObstacles");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ObstacleManager manager = (ObstacleManager)target;

        // ── Başlık ──────────────────────────────────────────────
        GUIStyle header = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 13,
            normal   = { textColor = new Color(0.4f, 0.9f, 0.6f) }
        };
        EditorGUILayout.LabelField("OBSTACLE MANAGER", header);
        EditorGUILayout.Space(4);

        // ── Özet ────────────────────────────────────────────────
        DrawSummary();
        EditorGUILayout.Space(6);

        // ── Obstacle Listesi (kompakt) ───────────────────────────
        EditorGUILayout.LabelField("Obstacle Listesi", EditorStyles.boldLabel);
        EditorGUILayout.Space(2);

        if (_entries.arraySize == 0)
        {
            EditorGUILayout.HelpBox("Henüz obstacle yok. 'Refresh' butonuna bas.", MessageType.Info);
        }
        else
        {
            DrawCompactList();
        }

        EditorGUILayout.Space(6);

        // ── Genel Limit ──────────────────────────────────────────
        EditorGUILayout.LabelField("Genel Limit", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_maxActive, new GUIContent("Max Aktif Obstacle", "0 = sınırsız"));

        EditorGUILayout.Space(8);

        // ── Refresh Butonu ───────────────────────────────────────
        GUI.backgroundColor = new Color(0.3f, 0.7f, 1f);
        if (GUILayout.Button("⟳  Refresh Obstacles From Children", GUILayout.Height(30)))
        {
            var method = typeof(ObstacleManager).GetMethod(
                "RefreshObstacles",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(target, null);
            serializedObject.Update();
        }
        GUI.backgroundColor = Color.white;

        serializedObject.ApplyModifiedProperties();
    }

    // ─────────────────────────────────────────────────────────────
    // Özet kutusu
    // ─────────────────────────────────────────────────────────────

    private void DrawSummary()
    {
        // Türe göre say
        Dictionary<string, int> counts  = new Dictionary<string, int>();
        int enabledCount  = 0;
        int disabledCount = 0;

        for (int i = 0; i < _entries.arraySize; i++)
        {
            SerializedProperty entry    = _entries.GetArrayElementAtIndex(i);
            SerializedProperty obsProp  = entry.FindPropertyRelative("obstacle");
            SerializedProperty enabledP = entry.FindPropertyRelative("enabled");

            if (obsProp.objectReferenceValue == null) continue;

            string typeName = obsProp.objectReferenceValue.GetType().Name;
            if (!counts.ContainsKey(typeName)) counts[typeName] = 0;
            counts[typeName]++;

            if (enabledP.boolValue) enabledCount++;
            else                    disabledCount++;
        }

        // Kutu arka planı
        Rect boxRect = EditorGUILayout.BeginVertical();
        EditorGUI.DrawRect(new Rect(boxRect.x - 2, boxRect.y - 2,
                                   boxRect.width + 4, boxRect.height + 8),
                           new Color(0.15f, 0.15f, 0.15f, 0.4f));

        EditorGUILayout.Space(2);

        // Tür bazlı sayılar
        foreach (var kv in counts)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(8);
            EditorGUILayout.LabelField($"• {kv.Key}", GUILayout.Width(150));
            EditorGUILayout.LabelField($"x{kv.Value}", EditorStyles.boldLabel, GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(2);

        // Aktif / Pasif
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(8);

        GUIStyle activeStyle = new GUIStyle(EditorStyles.miniLabel)
            { normal = { textColor = new Color(0.4f, 1f, 0.4f) } };
        GUIStyle disableStyle = new GUIStyle(EditorStyles.miniLabel)
            { normal = { textColor = new Color(1f, 0.4f, 0.4f) } };

        EditorGUILayout.LabelField($"✓ Aktif: {enabledCount}",  activeStyle,  GUILayout.Width(90));
        EditorGUILayout.LabelField($"✗ Pasif: {disabledCount}", disableStyle, GUILayout.Width(90));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(2);
        EditorGUILayout.EndVertical();
    }

    // ─────────────────────────────────────────────────────────────
    // Kompakt liste: her satır = 1 obstacle
    // ─────────────────────────────────────────────────────────────

    private void DrawCompactList()
    {
        // Kolon başlıkları
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(8);
        EditorGUILayout.LabelField("Obstacle",    EditorStyles.miniLabel, GUILayout.Width(160));
        EditorGUILayout.LabelField("Açık",        EditorStyles.miniLabel, GUILayout.Width(36));
        EditorGUILayout.LabelField("Şans (%)",    EditorStyles.miniLabel, GUILayout.Width(130));
        EditorGUILayout.EndHorizontal();

        DrawLine();

        for (int i = 0; i < _entries.arraySize; i++)
        {
            SerializedProperty entry       = _entries.GetArrayElementAtIndex(i);
            SerializedProperty obsProp     = entry.FindPropertyRelative("obstacle");
            SerializedProperty enabledProp = entry.FindPropertyRelative("enabled");
            SerializedProperty chanceProp  = entry.FindPropertyRelative("spawnChance");

            if (obsProp.objectReferenceValue == null) continue;

            bool isEnabled = enabledProp.boolValue;

            // Satır arka planı — pasifse soluk
            Color rowColor = isEnabled
                ? new Color(0.2f, 0.2f, 0.2f, 0.3f)
                : new Color(0.4f, 0.1f, 0.1f, 0.2f);

            Rect rowRect = EditorGUILayout.BeginHorizontal();
            EditorGUI.DrawRect(rowRect, rowColor);

            GUILayout.Space(8);

            // İsim (tür + obje adı)
            string typeName = obsProp.objectReferenceValue.GetType().Name;
            string objName  = obsProp.objectReferenceValue.name;
            string label    = typeName == objName ? typeName : $"{typeName} ({objName})";

            GUIStyle nameStyle = new GUIStyle(EditorStyles.label);
            nameStyle.normal.textColor = isEnabled ? Color.white : new Color(0.6f, 0.6f, 0.6f);
            EditorGUILayout.LabelField(label, nameStyle, GUILayout.Width(160));

            // Toggle
            enabledProp.boolValue = EditorGUILayout.Toggle(enabledProp.boolValue, GUILayout.Width(20));

            // Şans slider (sadece enabled ise aktif)
            EditorGUI.BeginDisabledGroup(!isEnabled);
            chanceProp.intValue = EditorGUILayout.IntSlider(chanceProp.intValue, 0, 100, GUILayout.Width(160));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
        }

        DrawLine();
    }

    private void DrawLine()
    {
        Rect r = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(r, new Color(0.5f, 0.5f, 0.5f, 0.5f));
    }
}
#endif