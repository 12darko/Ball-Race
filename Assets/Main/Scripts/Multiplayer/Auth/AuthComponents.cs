using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Multiplayer.Auth.Anonymous
{
    public abstract class AuthComponents : MonoBehaviour
    {
        // Butona tıklandığında tetiklenecek olay
        // AuthManager bu olayı dinler.
        public UnityEvent OnLoginButtonClicked;

        [SerializeField] private Button loginBtn;
        [SerializeField] private GameObject loginPanel;

        protected virtual void Awake()
        {
            // Olayı butona bağla.
            if (loginBtn != null)
            {
                loginBtn.onClick.AddListener(() =>
                    OnLoginButtonClicked?.Invoke());
            }
        }

        // Panelin görünürlüğünü kontrol etmek için
        public abstract void TogglePanel(bool isActive);
        
        // Dışarıdan erişim için
        public Button LoginBtn => loginBtn;
        public GameObject LoginPanel => loginPanel;
    }
}