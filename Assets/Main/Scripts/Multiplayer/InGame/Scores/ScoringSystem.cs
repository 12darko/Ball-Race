using _Main.Scripts.Multiplayer.Multiplayer.Modes;
using Fusion;
using Player;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame.Scores
{
     public class ScoringSystem : NetworkBehaviour
    {
        public static ScoringSystem Instance { get; private set; }

        private InGameMode _mode = InGameMode.Standard_Mode;

        // Bireysel modlar — finish pozisyonu bazlı
        private static readonly int[] SoloFinishPoints = { 10, 6, 3 };

        // Takım modları — finish pozisyonu bazlı
        private static readonly int[] TeamFinishPoints = { 8, 5, 2 };

        // Kill puanları
        private const int FallBallsKillPoints  = 3;  // Fall_Balls: kill başı
        private const int StandardKillBonus    = 1;  // Standard/Lap: kill bonus (az)
        private const int TeamMatchKillBonus   = 1;  // Team_Match: takım kill bonusu

        // Soccer: gol başı puan
        private const int SoccerGoalPoints = 5;

        // Hayatta kalma bonusu (Fall_Balls — son X kişi)
        private const int SurvivalBonus = 5;

        private void Awake() => Instance = this;

        public void SetMode(InGameMode mode) => _mode = mode;

        public void AddFinishScore(PlayerRef player, int position)
        {
            if (!Runner.IsServer) return;
            var ps = FindStatus(player);
            if (ps == null) return;

            switch (_mode)
            {
                case InGameMode.Standard_Mode:
                case InGameMode.Lap_Mode:
                    ps.Score += PositionPoint(SoloFinishPoints, position);
                    break;

                case InGameMode.Team_Match:
                    ps.Score += PositionPoint(TeamFinishPoints, position);
                    // Takım puanı ayrıca eklenebilir — TeamManager hazır olunca
                    break;

                case InGameMode.Fall_Balls:
                case InGameMode.Team_Soccer_Match:
                    // Finish puanı yok, sadece kill/gol sayılır
                    break;
            }
        }

        public void AddKillScore(PlayerRef killer)
        {
            if (!Runner.IsServer) return;
            var ps = FindStatus(killer);
            if (ps == null) return;

            switch (_mode)
            {
                case InGameMode.Standard_Mode:
                case InGameMode.Lap_Mode:
                    ps.Score += StandardKillBonus;
                    break;

                case InGameMode.Fall_Balls:
                    ps.Score += FallBallsKillPoints;
                    break;

                case InGameMode.Team_Match:
                    ps.Score += TeamMatchKillBonus;
                    break;

                case InGameMode.Team_Soccer_Match:
                    // Soccer'da kill puanı yok, sadece gol var
                    break;
            }
        }

        /// Team_Soccer_Match — karşı takımın finish'ine giren oyuncu için çağır
        public void AddGoalScore(PlayerRef scorer)
        {
            if (!Runner.IsServer) return;
            if (_mode != InGameMode.Team_Soccer_Match) return;

            var ps = FindStatus(scorer);
            if (ps == null) return;
            ps.Score += SoccerGoalPoints;
        }

        /// Fall_Balls — hayatta kalan son oyuncular için çağır
        public void AddSurvivalBonus(PlayerRef player)
        {
            if (!Runner.IsServer) return;
            if (_mode != InGameMode.Fall_Balls) return;

            var ps = FindStatus(player);
            if (ps == null) return;
            ps.Score += SurvivalBonus;
        }

        private static int PositionPoint(int[] table, int position)
            => position >= 1 && position <= table.Length ? table[position - 1] : 0;

        private NetworkPlayerStatus FindStatus(PlayerRef p)
        {
            foreach (var ps in FindObjectsOfType<NetworkPlayerStatus>())
                if (ps.Object.InputAuthority == p) return ps;
            return null;
        }
    }
}