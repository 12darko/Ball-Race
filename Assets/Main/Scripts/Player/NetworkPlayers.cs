using _Main.Scripts.Multiplayer.Multiplayer;
using _Main.Scripts.Multiplayer.Multiplayer.InGame.CountDown;
using _Main.Scripts.Multiplayer.Player;
using Fusion;
using Main.Scripts.Player;
using UnityEngine;

namespace Player
{
    public class NetworkPlayers : NetworkBehaviour, IPlayerLeft
    {
        [SerializeField] private NetworkPlayerInputState playerState = NetworkPlayerInputState.Idle;

        [Networked] public NetworkBool PlayerIsAlive { get; set; }
        [Networked] public NetworkBool PlayerInFinish { get; set; }

        public bool AcceptAnyInput => PlayerIsAlive && !GameManager.MatchIsOver;

        public NetworkPlayerInputState PlayerState
        {
            get => playerState;
            set => playerState = value;
        }

        public static NetworkPlayers LocalPlayers { get; set; }

        [field: SerializeField] public NetworkPlayersManager networkPlayersManager { get; private set; }
        [field: SerializeField] public NetworkPlayerInGameData networkPlayerInGameData { get; private set; }

 


        public override void Spawned()
        {
            // Spawn Edilen Playerların kamera kapatma kodları burada
            base.Spawned();

            if (!Object.IsValid) return;

            PlayerIsAlive = true;
            PlayerInFinish = false;


            gameObject.name = "Player = " + Object.InputAuthority;
            networkPlayersManager.LocalCamera.name = "Player Camera : " + Object.InputAuthority;
            networkPlayersManager.LocalCinemachineCamera.name = "Player VR Camera : " + Object.InputAuthority;

            Runner.SetIsSimulated(Object, true);
            if (Object.HasInputAuthority)
            {
                LocalPlayers = this;
                networkPlayersManager.NetworkPlayerHud.GetComponent<CanvasGroup>().alpha = 1;

                // 1. ÖNCE: Sahne kamerasını sustur (Çakışma olmasın)
                if(networkPlayersManager.StageCamera.GetComponent<AudioListener>() != null)
                {
                    networkPlayersManager.StageCamera.GetComponent<AudioListener>().enabled = false;
                }

                // 2. SONRA: Kendi kameramızın kulağını aç
                networkPlayersManager.LocalCamera.GetComponent<AudioListener>().enabled = true;
                 var cam = networkPlayersManager.LocalCamera.transform;
                    var vcam = networkPlayersManager.LocalCinemachineCamera.transform;

                    cam.SetParent(null, true);
                    vcam.SetParent(null, true);
                Debug.Log("Local Player Benim !!!!!!");
            }
            else
            {
                // BURASI DİĞER (REMOTE) OYUNCULAR
                Debug.Log("Diğer Playerlar Spawne Edildi");
                networkPlayersManager.LocalCamera.gameObject.SetActive(false);
                networkPlayersManager.LocalCinemachineCamera.gameObject.SetActive(false);
                // Diğer oyuncu geldi diye bizim sahne kamerasını kapatmaya gerek var mı emin değilim 
                // ama senin mantığına göre bırakıyorum:
                networkPlayersManager.StageCamera.gameObject.SetActive(false);
            }
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (player == Object.InputAuthority)
                Runner.Despawn(transform.parent.GetComponent<NetworkObject>());
        }
    }
}