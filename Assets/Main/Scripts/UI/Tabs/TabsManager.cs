using _Main.Scripts.Multiplayer.Multiplayer.Customize;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.UI.Tabs
{
    public class TabsManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] tabsPanel;
        [SerializeField] private Button[] tabsButtons;
    

        public void SwitchToTab(int tabId)
        {
            foreach (var go in tabsPanel)
            {
                go.SetActive(false);
            }

            tabsPanel[tabId].SetActive(true);
            foreach (var button in tabsButtons)
            {
                button.image.sprite = button.GetComponent<TabsButton>().InactiveTab;
            }

            tabsButtons[tabId].image.sprite =  tabsButtons[tabId].GetComponent<TabsButton>().ActiveTab;
            

        }
    }
}