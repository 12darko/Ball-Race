using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    [SerializeField] private uint appId = 480;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Steam'i başlatmak için dışarıdan çağrılacak metot
    public void InitializeSteam()
    {
        try
        {
            SteamClient.Init(appId); // ⚠️ AppId'yi kendi oyun kimliğinle değiştir B460F35F63C86E12D087C8CE63D5E756
            Debug.Log("SteamClient başarıyla başlatıldı.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Steamworks başlatılamadı: " + e.Message);
        }
    }

    private void Update()
    {
        // Steam başlatılmışsa callback'leri çalıştır
        if (SteamClient.IsValid)
        {
            SteamClient.RunCallbacks();
        }
    }
    
    private void OnDestroy()
    {
        // Steam başlatılmışsa kapat
        if (SteamClient.IsValid)
        {
            SteamClient.Shutdown();
        }
    }
    private void OnApplicationQuit()
    {
        if (SteamClient.IsValid)
        {
            SteamClient.Shutdown();
            Debug.Log("SteamClient kapatıldı.");
        }
    }
}