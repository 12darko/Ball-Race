using System;
using System.Threading.Tasks;
using Multiplayer.Player; // PlayerManager için gerekli
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer.Auth
{
    public class UserLogin : Login
    {
        public override async Task SignInAsync()
        {
            // Bu metod boş kalabilir veya abstract yapıdan dolayı override edilmesi gerekebilir.
            await Task.CompletedTask;
        }

        // Kullanıcı adı ve şifre ile GİRİŞ
        public async Task SignInWithUsernamePassword(string userName, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(userName, password);
                Debug.Log("Giriş Başarılı! ID: " + AuthenticationService.Instance.PlayerId);
                
                // --- DÜZELTME: Veri Yükleme ---
                await PlayerManager.Instance.PlayerLoadDataForUnity(loadData);                
                SceneManager.LoadScene((int)SceneId.MENU);
            }
            catch (Exception ex)
            {
                Debug.LogError("Giriş Hatası: " + ex.Message);
            }
        }
        
        // Kullanıcı adı ve şifre ile KAYIT
        public async Task SignUpWithUsernamePassword(string username, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                Debug.Log("Kayıt Başarılı! ID: " + AuthenticationService.Instance.PlayerId);

                // --- DÜZELTME: Kayıt Sonrası İşlem ---
                // Kayıt olduktan sonra varsayılan verileri kaydetmek için eski metodu çağırabiliriz
                // Veya direkt menüye yönlendirip LoadData'nın "Para yoksa 5000 ver" mantığını kullanabiliriz.
                // Senin yapını bozmamak için Save çağırıyorum:
                PlayerManager.Instance.PlayerSaveDataForUnity(saveData);
                
                // Not: Genelde Kayıt olduktan sonra otomatik giriş (SignIn) de yapılır veya kullanıcıdan giriş yapması istenir.
                // Direkt menüye atarsan veriler yüklenmemiş olabilir.
                // En sağlıklısı kayıt sonrası SignInWithUsernamePassword çağırmaktır ama şimdilik sahneyi yüklüyoruz.
                SceneManager.LoadScene(1);
            }
            catch (Exception ex)
            {
                Debug.LogError("Kayıt Hatası: " + ex.Message);
            }
        }
    }
}