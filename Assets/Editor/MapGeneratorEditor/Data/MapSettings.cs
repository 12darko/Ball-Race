using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "LevelSettings_Master_v3", menuName = "Track Generator/Map Settings Profile")]
public class MapSettings : ScriptableObject
{
    [Header("--- GENEL AYARLAR ---")]
    public int trackLength = 15;
    public float scaleMultiplier = 100f;
    public GameObject startPrefab;
    public List<GameObject> finishPrefabs = new List<GameObject>();

    [Header("--- 📏 KRİTİK BOYUT AYARLARI (Manuel) ---")]
    [Tooltip("Düz bir parçanın uzunluğu kaç birim? (Örn: 4 veya 2)")]
    public float straightLength = 4f; 
    
    [Tooltip("Köşe parçasının genişliği kaç birim? (Genelde düz parçanın yarısıdır. Örn: 2)")]
    public float cornerSize = 2f;

    // -----------------------------------------------------------
    [Header("--- 1. LINEAR (DÜZ) MOD ---")]
    public List<GameObject> linearSafePaths = new List<GameObject>();
    public List<GameObject> linearObstaclePaths = new List<GameObject>();
    [Range(0f, 1f)] public float obstacleFrequency = 0.4f;
    
    public float linStartRot = 0f;    
    public float linPathRot = 0f;     
    public float linFinishRot = 180f; 
    public float linStartGap = 0f;       
    public float linGlobalGap = 0f;      

    // -----------------------------------------------------------
    [Header("--- 2. SNAKE (KIVRIMLI) MOD ---")]
    public List<GameObject> snakeStraightPrefabs = new List<GameObject>();
    public List<GameObject> snakeLeftCorners = new List<GameObject>(); // Scale X: -1 olanları buraya koy
    public List<GameObject> snakeRightCorners = new List<GameObject>();
    [Range(0f, 1f)] public float snakeTurnChance = 0.3f;

    public float snakeStartRot = 0f;
    public float snakeFinishRot = 180f; 
    public float snakeGlobalGap = 0f; // Bunu 0 yap
 
    // -----------------------------------------------------------
    // Diğer modlar (Vertical/Split) aşağıda aynı kalabilir...
    [Header("--- 3. VERTICAL (RAMPA) MOD ---")]
    public List<GameObject> rampUpPrefabs = new List<GameObject>();
    public List<GameObject> rampDownPrefabs = new List<GameObject>();
    public List<GameObject> verticalStraightPrefabs = new List<GameObject>();
    public float rampElevation = 1f;
    public float vertStartRot = 0f;
    public float vertPathRot = 0f;
    public float vertFinishRot = 180f;
    public float vertStartGap = 0f;
    public float vertGlobalGap = 0f;
// MapSettings.cs dosyasının içine, diğer değişkenlerin yanına ekle:
    [Header("--- 📏 MANUEL BOYUT AYARLARI ---")]
    public float manualRampLength = 10.0f;   // Rampanın Z uzunluğu
    public float manualRampHeight = 2.0f;    // Rampanın Y yüksekliği
    public float manualStraightLength = 10.0f; // Düz yolun uzunluğu
    [Header("--- 🏁 LAP (DÖNGÜ) AYARLARI ---")]
    public List<GameObject> lapStraightPrefabs; 
    public List<GameObject> lapCornerPrefabs;   
    
 

    [Range(1, 20)] public int lapWidth = 5;
    [Range(1, 20)] public int lapDepth = 3;
    public float lapScale = 1f;
    [Header("--- ⚔️ VERSUS ARENA (TAKIM RENKLERİ) ---")]
    
    [Header("🔵 MAVİ TAKIM (SOL)")]
    public List<GameObject> blueBases;   // Mavi Zeminler
    public List<GameObject> blueGoals;   // Mavi Kaleler
    public List<GameObject> blueConnectors; // Mavi Bağlantı Yolları

    [Header("🔴 KIRMIZI TAKIM (SAĞ)")]
    public List<GameObject> redBases;    // Kırmızı Zeminler
    public List<GameObject> redGoals;    // Kırmızı Kaleler
    public List<GameObject> redConnectors; // Kırmızı Bağlantı Yolları

    [Header("⚪ ORTAK ALAN")]
    public List<GameObject> arenaBridges; // Ortadaki köprü (Tarafsız)
    public GameObject centerPlatform;     // Orta Merkez Ada
    
    
    [Header("📏 AYARLAR")]
    public int bridgeLength = 10;
    public float arenaScale = 100f;
    public float bridgeGap = 0f;
    [Range(0, 360)] public float versusGoalRot = 0f;

    public bool emptyMiddle = false;       
    public bool spawnCenter = true;        
    [Range(0, 10f)] public float scatterAmount = 0f;
    public bool fillGoalGap = false;
 
 

    // ... (Diğer kodlar yukarıda) ...

    [Header("=================================================")]
    [Header("🐍⚔️ VERSUS SNAKE MODU (ÖZEL AYARLAR)")]
    
    [Header("🔵 MAVİ TAKIM (VS SNAKE ÖZEL)")]
    public List<GameObject> vsSnakeBlueBases;      
    public List<GameObject> vsSnakeBlueGoals;      
    public List<GameObject> vsSnakeBlueConnectors; 

    [Header("🔴 KIRMIZI TAKIM (VS SNAKE ÖZEL)")]
    public List<GameObject> vsSnakeRedBases;       
    public List<GameObject> vsSnakeRedGoals;       
    public List<GameObject> vsSnakeRedConnectors;  

    [Header("🌉 KIVRIMLI KÖPRÜ PARÇALARI (RENKLİ YOLLAR)")]
    [Tooltip("Versus moduna özel Düz Yollar (Mavi/Kırmızı/Renkli)")]
    public List<GameObject> vsSnakeBridgeStraights; 
    
    [Tooltip("Versus moduna özel Köşe Parçaları")]
    public List<GameObject> vsSnakeBridgeCorners;   

    [Header("⚙️ MOD AYARLARI")]
    // Not: Artık int yerine senin Snake classındaki gibi float veya int kullanabiliriz,
    // ama senin snake mantığında MapSettings'deki float turnChance kullanılıyordu.
    // Karışıklık olmasın diye buraya özel değişken ekledik:
    [Range(0f, 1f)] public float vsSnakeTurnChance = 0.3f; 
    [Range(1, 5)] public int vsSnakeMaxSideSteps = 2; 
    
    // ... (Versus kodlarının bittiği yer) ...

    [Header("=================================================")]
    [Header("🎪 GAUNTLET (FALL GUYS / KAOS) MODU")]

    [Header("🏗️ ZEMİN VE BAŞLANGIÇ")]
    public GameObject gauntletStart;        // Geniş Başlangıç Platformu
    public List<GameObject> gauntletFinish; // Bitiş (Castle, Finish Line)
    public List<GameObject> gauntletSafeZones; // Engeller arası düz, güvenli geniş zeminler

    [Header("⚠️ KAOS MODÜLLERİ (Engeller)")]
    [Tooltip("Obstacle 1-7, Hammer, Fan, Sweeper gibi hareketli/tehlikeli parçalar")]
    public List<GameObject> gauntletModules; 

    [Header("⚙️ GAUNTLET AYARLARI")]
    public int gauntletLength = 5;      // Kaç tane engel dizilsin?
    public float gauntletScale = 1f;    // Büyüklük çarpanı
    [Tooltip("Parçaların merkezleri arası mesafe (Parça boyuna göre ayarla)")]
    public float gauntletTileLength = 20f; // ÖNEMLİ: Senin o geniş parçaların uzunluğu neyse buraya gir.
    public float gauntletGap = 0f;      // Ekstra boşluk
}
 
 
 
 
 