using _Main.Scripts.Multiplayer.Player.NetworkInput;
using Fusion;
using UnityEngine;

namespace Player
{
    public class NetworkPlayerJump : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private NetworkPlayers networkPlayer;
        [SerializeField] private NetworkPlayersGroundChecker groundChecker;

        [Networked] public NetworkButtons ButtonsPrevious { get; set; }
        
        // Zıplama haklarını tutan değişken
        [SerializeField, Networked] private int JumpsRemaining { get; set; }

        // Zıpladıktan sonra kısa süre yer kontrolünü kapatmak için zamanlayıcı
        [SerializeField, Networked] private TickTimer GroundCheckBlockTimer { get; set; }

        private int _maxJumpCount;

        public override void Spawned()
        {
            _maxJumpCount = networkPlayer.networkPlayersManager.NetworkPlayerStats.NetworkPlayerMaxJumpCount;
            JumpsRemaining = _maxJumpCount;
        }

        public override void FixedUpdateNetwork()
        {
            // 1. YER KONTROLÜ VE HAK YENİLEME
            // Eğer "GroundCheckBlockTimer" süresi dolmuşsa yer kontrolü yap.
            // (Yani zıpladıktan hemen sonra buraya girmesin)
            if (GroundCheckBlockTimer.ExpiredOrNotRunning(Runner))
            {
                if (groundChecker.IsGrounded())
                {
                    JumpsRemaining = _maxJumpCount;
                }
            }

            // 2. INPUT VE ZIPLAMA
            if (GetInput<NetworkPlayerInput>(out NetworkPlayerInput playerInput))
            {
                var pressed = playerInput.NetworkAllButtons.GetPressed(ButtonsPrevious);
                ButtonsPrevious = playerInput.NetworkAllButtons;

                if (pressed.IsSet(NetworkPlayerInputEnums.Jump))
                {
                    TryJump();
                }
            }
        }

        private void TryJump()
        {
            // Havada hakkımız kalmadıysa iptal
            if (JumpsRemaining <= 0) return;

            // --- FİZİK DÜZELTMESİ ---
            // Önceki dikey hızı sıfırla (Double jump yaparken düşüyorsan, düşüşü iptal et)
            Vector3 currentVel = rb.linearVelocity;
            currentVel.y = 0; 
            rb.linearVelocity = currentVel;
            
            float jumpSpeed = networkPlayer.networkPlayersManager.NetworkPlayerStats.NetworkPlayerJumpSpeed;

            // --- KRİTİK DEĞİŞİKLİK: VelocityChange ---
            // Impulse yerine VelocityChange kullanıyoruz. Mass'i (Ağırlığı) yok sayar.
            // Artık Inspector'a 50-60 yazmana gerek kalmaz, 8-12 arası uçar.
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);

            // Hakkı azalt
            JumpsRemaining--;

            // --- BUG FİX: COOLDOWN ---
            // Zıpladık, 0.15 saniye boyunca yer kontrolü yapma.
            // Bu sayede "hala yerdeyim" sanıp hakkı hemen fullemez.
            GroundCheckBlockTimer = TickTimer.CreateFromSeconds(Runner, 0.15f);
        }
    }
}