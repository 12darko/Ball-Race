using System;
using System.Linq;
using System.Threading.Tasks;
using Multiplayer.Player;
using Steamworks;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer.Auth
{
    public class SteamLogin : Login
    {
        public override async Task SignInAsync()
        {
            if (!SteamClient.IsValid) return;

            try
            {
                var ticketResult = await SteamUser.GetAuthSessionTicketAsync(SteamClient.SteamId);
                var authTicketData = ticketResult.Data.ToArray();

                if (authTicketData.Length > 0)
                {
                    string authTicketString = Convert.ToBase64String(authTicketData);

                    await AuthenticationService.Instance.SignInWithSteamAsync(authTicketString);
                    Debug.Log("Steam Girişi Başarılı. Veriler bekleniyor...");
                    
                    // --- DÜZELTME: "await" ekledik ---
                    // Veriler Cloud'dan inmeden sahne asla değişmeyecek.
                    await PlayerManager.Instance.PlayerLoadDataForUnity(loadData);
                    
                    SceneManager.LoadScene((int)SceneId.MENU);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Steam Giriş Hatası: " + e.Message);
            }
        }
    }
}