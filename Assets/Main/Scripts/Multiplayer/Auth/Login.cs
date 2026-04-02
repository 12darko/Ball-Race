using System;
using System.Threading.Tasks;
using Assets._Main.Scripts.UI.MessagePopUp;
 
using UnityEngine;
using Unity.Services.Core;

namespace Multiplayer.Auth
{
    // Login tiplerine daha iyi uyum sağlaması için daha esnek bir yapı
    public abstract class Login : MonoBehaviour
    {
        // Public yerine [SerializeField] kullanarak Unity Editor'da atama yapabiliriz
        [SerializeField] protected Multiplayer.CloudSave.SaveData saveData;
        [SerializeField] protected Multiplayer.CloudSave.LoadData loadData;
        
        // Bu Awake metodu, UnityServices'i her seferinde başlatmaya çalışmaz.
        public virtual async void Awake()
        {
            // Unity Services'i uygulamanın başında tek bir yerde başlatın (örneğin GameManager veya MainLobbyManager)
            // Bu metodu burada bırakıyorum ama çağırmayı siz kontrol edin.
            await UnityServices.InitializeAsync();
        }

       
        // Tek ve esnek bir soyut metot
        public abstract Task SignInAsync();

        // Bu metotları doğrudan kullanmanıza gerek kalmıyor
        // public abstract void SignIn(string userName, string password);
        // public abstract Task SignInType(string userName, string password);
    }
}