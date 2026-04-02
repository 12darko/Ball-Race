using _Main.Scripts.Multiplayer.Multiplayer.InGame.Spectator;
using _Main.Scripts.Multiplayer.Multiplayer.InGame.Team;
using Fusion;

namespace Player
{
    public class NetworkPlayerStatus : NetworkBehaviour
    {
        
            [Networked] public NetworkPlayerState  State          { get; set; }
            [Networked] public PlayerRef           KilledBy       { get; set; }
            [Networked] public int                 FinishPosition { get; set; }
            [Networked] public int                 Score          { get; set; }
            [Networked] public NetworkString<_32>  DisplayName    { get; set; }
            [Networked] public TeamType            Team           { get; set; } // ← eklendi

            private ChangeDetector _cd;

            public override void Spawned()
            {
                _cd   = GetChangeDetector(ChangeDetector.Source.SimulationState);
                State = NetworkPlayerState.Alive;
            }

            public override void Render()
            {
                foreach (var change in _cd.DetectChanges(this, out _, out _))
                {
                    if (change == nameof(State))
                        OnStateChanged();
                }
            }

            private void OnStateChanged()
            {
                if (!Object.HasInputAuthority) return;
                if (State == NetworkPlayerState.Spectating)
                    SpectatorManager.Instance?.EnterSpectatorMode(this);
            }
        }

  
}