using Multiplayer.Auth.Anonymous;
using TMPro;
using UnityEngine;

namespace Multiplayer.Auth
{
    public class UserComponent : AuthComponents
    {
        [SerializeField] private TMP_InputField userNameInputField;
        [SerializeField] private TMP_InputField userPasswordInputField;

        public TMP_InputField UserNameInputField => userNameInputField;
        public TMP_InputField UserPasswordInputField => userPasswordInputField;

        // Base sınıfın soyut metodunu uyguluyoruz.
        // Artık bu metot, AuthManager tarafından çağrılacak.
        public override void TogglePanel(bool isActive)
        {
            LoginPanel.SetActive(isActive);
        }

        // Eski ve gereksiz olan bu metodu kaldırdık.
        // Artık bu mantık AuthManager'da yönetiliyor.
        // public override void LoginButtonController(Login loginType)
        // {
        //     loginType.SignIn(userNameInputField.text, userPasswordInputField.text);
        // }
    }
}