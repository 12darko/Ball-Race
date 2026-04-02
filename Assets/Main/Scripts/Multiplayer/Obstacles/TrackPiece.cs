using UnityEngine;

public class TrackPiece : MonoBehaviour
{
    [Header("BAĞLANTI NOKTALARI (Lokal)")]
    [Tooltip("Parçanın giriş noktası (Önceki parçaya değecek yer)")]
    public Vector3 startOffset = Vector3.zero; 
    [Header("Eğim Ayarı (Basamağı bu çözer)")]
    public float exitPitch; // Rampa yukarı ise örn: -15, aşağı ise 15, düz ise 0
    [Tooltip("Parçanın çıkış noktası (Sonraki parçanın başlayacağı yer)")]
    public Vector3 endOffset = new Vector3(0, 0, 2f);

    [ContextMenu("Auto Fix Offsets")]
    public void AutoFix()
    {
        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        if (meshFilter == null) return;

        // Modelin dünya üzerindeki değil, kendi içindeki (local) sınırları
        Bounds bounds = meshFilter.sharedMesh.bounds;

        // Düz ve rampalar için en mantıklı matematiksel uç noktalar:
        // Start: Modelin en arkası (Z min) ve alt merkezi
        startOffset = new Vector3(0, 0, bounds.min.z);
        endOffset   = new Vector3(0, bounds.max.y - bounds.min.y, bounds.max.z);

        // End: Modelin en önü (Z max) ve üst merkezi
        endOffset = new Vector3(0, bounds.max.y, bounds.min.z);

        Debug.Log($"{gameObject.name} için offsetler hesaplandı!");
    }
    [ContextMenu("Lego Smart AutoFix (KESİN ÇÖZÜM)")]
    public void LegoSmartAutoFix()
    {
        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        if (meshFilter == null) return;
        Bounds bounds = meshFilter.sharedMesh.bounds;

        // 1. ADIM: Z Eksenini Düzelt (Arka Giriş, Ön Çıkış)
        // Start (Yeşil) arkada olmalı -> bounds.min.z
        // End (Sarı) önde olmalı -> bounds.max.z
        float startZ = Mathf.Round(bounds.min.z * 20f) / 20f;
        float endZ = Mathf.Round(bounds.max.z * 20f) / 20f;

        // 2. ADIM: Y Eksenini SIFIRLA (Merdiven Sorununu Bu Çözer)
        // Start Y mutlaka 0 olmalı ki parça bir öncekine 'kaynasın'
        float startY = 0; 
        // End Y ise sadece yükseklik farkı olmalı
        float endY = Mathf.Round((bounds.max.y - bounds.min.y) * 20f) / 20f;

        // Eğer düz parçaysa (yükseklik farkı çok azsa) End Y'yi de sıfırla
        if (Mathf.Abs(bounds.max.y - bounds.min.y) < 0.1f) endY = 0;

        startOffset = new Vector3(0, startY, startZ);
        endOffset = new Vector3(0, endY, endZ);

        Debug.Log($"{gameObject.name} Ayarlandı! Start Y artık 0.");
    }
    void OnDrawGizmosSelected()
    {
        // Hassasiyet Ayarı: 0.1f yerine 0.02f yaparak iğne ucu kadar küçülttük
        float pointSize = 0.02f;

        // GİRİŞ NOKTASI (START) - YEŞİL
        Gizmos.color = Color.green;
        Vector3 globalStart = transform.TransformPoint(startOffset);
        Gizmos.DrawSphere(globalStart, pointSize);
        // Yön çizgisi: Girişin nereye baktığını görmek için minik bir hat
        Gizmos.DrawLine(globalStart, globalStart + transform.up * 0.1f);

        // ÇIKIŞ NOKTASI (END) - SARI
        Gizmos.color = Color.yellow;
        Vector3 globalEnd = transform.TransformPoint(endOffset);
        Gizmos.DrawSphere(globalEnd, pointSize);
        // Yön çizgisi: Çıkışın nereye baktığını görmek için minik bir hat
        Gizmos.DrawLine(globalEnd, globalEnd + transform.up * 0.1f);
    
        // BAĞLANTI HATTI (Yolun akış yönü)
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(globalStart, globalEnd);

        // YÜZEY HİZALAMA YARDIMCISI: Yolun genişliğini temsil eden minik bir çizgi
        Gizmos.color = Color.magenta;
        Vector3 sideOffset = transform.right * 0.1f;
        Gizmos.DrawLine(globalStart - sideOffset, globalStart + sideOffset);
        Gizmos.DrawLine(globalEnd - sideOffset, globalEnd + sideOffset);
    }
    
    // TrackPiece.cs dosyasının içine, OnDrawGizmosSelected fonksiyonunun altına ekle:

    [ContextMenu("Otomatik Ölç (Tahmini)")]
    void AutoMeasure()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            // Bu sadece kaba bir tahmindir, elle düzeltmen gerekebilir
            endOffset = new Vector3(0, 0, rend.bounds.size.z);
            Debug.Log($"{name} için tahmini uzunluk atandı: {endOffset}");
        }
    }
}