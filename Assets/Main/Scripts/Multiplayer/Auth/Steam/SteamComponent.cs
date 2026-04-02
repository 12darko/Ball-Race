using Multiplayer.Auth.Anonymous;
using TMPro;
using UnityEngine;

namespace Multiplayer.Auth
{
    public class SteamComponent: AuthComponents
    {
        [SerializeField] private TMP_Text logTxt;

        public TMP_Text LogTxt => logTxt;
        
        // AuthComponents'ten miras alınan soyut metodu uyguluyoruz.
        // Bu metot, AuthManager tarafından çağrılır.
        public override void TogglePanel(bool isActive)
        {
            LoginPanel.SetActive(isActive);
        }

        // Eski ve gereksiz olan LoginButtonController metodunu kaldırdık.
        // Bu metodun mantığı artık AuthManager'da yönetiliyor.
        // public override void LoginButtonController(Login loginType)
        // {
        //     loginType.SignIn(null, null);
        // }
    }
}