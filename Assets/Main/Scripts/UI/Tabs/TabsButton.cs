using UnityEngine;

namespace _Main.Scripts.Multiplayer.UI.Tabs
{
    public class TabsButton : MonoBehaviour
    {
        [SerializeField] private Sprite inactiveTab;
        [SerializeField] private Sprite activeTab;

        public Sprite InactiveTab => inactiveTab;

        public Sprite ActiveTab => activeTab;
    }
}