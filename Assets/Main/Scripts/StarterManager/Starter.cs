using System;
using _Main.Scripts.Multiplayer.Runners;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.FusionCore.StartManager
{
    public class Starter : MonoBehaviour
    {
    //    [SerializeField] private NetworkRunnerHandlerDeveloper networkRunnerHandler;
        
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;

        private void Start()
        {
             hostButton.onClick.AddListener(HostStart);
             clientButton.onClick.AddListener(ClientStart);
        }
 
        private void HostStart()
        {
          //  networkRunnerHandler.StartGame(GameMode.Host,"TestRoom", true, true,4);
        }

        private void ClientStart()
        {
           // networkRunnerHandler.StartGame(GameMode.Client,"TestRoom", true, true,4);
        }
    }
}