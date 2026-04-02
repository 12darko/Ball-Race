using System;
using System.Threading.Tasks;
using Multiplayer.Player;
 using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer.Auth
{
    public class AnonymousLogin : Login
    {
        public override async Task SignInAsync()
        {
            try
            {
               
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Anonim Giriş Başarılı! ID: " + AuthenticationService.Instance.PlayerId);

                // --- DÜZELTME BURADA ---
                // Yeni sistemdeki yükleme metodunu çağırıyoruz
                await PlayerManager.Instance.PlayerLoadDataForUnity(loadData);                
                Debug.Log("Oyuncu verileri yükleniyor...");

                SceneManager.LoadScene((int)SceneId.MENU);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("Anonim Giriş Başarısız!");
            }
        }
    }
}