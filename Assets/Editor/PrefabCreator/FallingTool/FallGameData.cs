using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FallGameData", menuName = "Fall Guys/Game Data")]
public class FallGameData : ScriptableObject
{
    [Header("--- PARKUR AYARLARI ---")]
    public GameObject startModule;
    public GameObject endModule;
    public List<GameObject> parkourModules = new List<GameObject>(); 
    
    [Range(1, 50)] public int levelLength = 10;
    
    // --- YENİ EKLENENLER: ŞERİT AYARLARI ---
    [Range(1, 5)] public int laneCount = 3;   // Kaç şerit olsun? (Standart 3)
    public float laneSpacing = 4.0f;          // Yan yana boşluk (Senin zeminler 4x4 olduğu için 4 ideal)
    // ----------------------------------------

    [Header("--- FABRİKA: AKTİF ÇALIŞMA ALANI ---")]
    public List<GameObject> factoryFloors = new List<GameObject>(); 
    public bool factoryRandomFloor = true;
    public int factoryFloorID = 0;         
    public List<GameObject> rawObstacles = new List<GameObject>();
    public float factorySpacing = 10f;
    public bool factoryAddPhysics = true;

    [Header("--- 💾 HAZIR PAKET DEPOSU ---")]
    
    // KAOS & SABİT & DÖNEN (Bunlar aynı kalıyor)
    public List<GameObject> presetChaos = new List<GameObject>();      
    public List<GameObject> presetChaosFloors = new List<GameObject>(); 
    
    public List<GameObject> presetStatic = new List<GameObject>();
    public List<GameObject> presetStaticFloors = new List<GameObject>();
    
    public List<GameObject> presetRotating = new List<GameObject>();
    public List<GameObject> presetRotatingFloors = new List<GameObject>();
    
    // --- RENKLİ PAKETLER (YENİ! RENK RENK AYRILDI) ---
    [Header("🔵 MAVİ TAKIM")]
    public List<GameObject> blueObstacles = new List<GameObject>();
    public List<GameObject> blueFloors = new List<GameObject>();

    [Header("🔴 KIRMIZI TAKIM")]
    public List<GameObject> redObstacles = new List<GameObject>();
    public List<GameObject> redFloors = new List<GameObject>();

    [Header("🟢 YEŞİL TAKIM")]
    public List<GameObject> greenObstacles = new List<GameObject>();
    public List<GameObject> greenFloors = new List<GameObject>();

    [Header("🟡 SARI TAKIM")]
    public List<GameObject> yellowObstacles = new List<GameObject>();
    public List<GameObject> yellowFloors = new List<GameObject>();

    [Header("--- HEX MAP AYARLARI ---")]
    public List<GameObject> hexTiles = new List<GameObject>(); 
    public float hexSpacing = 1.1f;
    public FallGameTool.MapShape currentShape = FallGameTool.MapShape.Dikdortgen; 
    public int hexWidth = 15;
    public int hexDepth = 15;
    public int mapRadius = 8; 
    public bool hexHoneycomb = true;
    public bool hexRandomColors = true; 
    public int hexSelectedID = 0;       
}