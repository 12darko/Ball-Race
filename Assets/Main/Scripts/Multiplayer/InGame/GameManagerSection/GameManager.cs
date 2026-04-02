using System;
using _Main.Scripts.Multiplayer.Singleton;
using Fusion;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame.CountDown
{
    public class GameManager : NetworkBehaviour
    {     
        public static GameManager Instance { get; private set; }

        public static bool MatchIsOver { get; set; }
        
        [Networked] private TickTimer GameTimer { get; set; }
        [Networked] private NetworkString<_32> OutPut { get; set; }

        [SerializeField] private float gameTimerDuration;
        
        [SerializeField] private TMP_Text gameTimerText;
        
        private ChangeDetector _changeDetector;

        public override void Spawned()
        {           
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            GameTimer = TickTimer.CreateFromSeconds(Runner, gameTimerDuration);

            MatchIsOver = false;

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                GlobalManager.Instance.NetworkRunnerHandler.Disconnect();
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        public override void Render()
        {
            foreach (var change in _changeDetector.DetectChanges(this, out var previous, out var current))
            {
                switch (change)
                {
                    case nameof(OutPut):

                        //Todo Change Sistemi Değiştiği için bu şekil kullanıyoruz
                        OnChangedCountDownTxt();
                        break;
                }
            }
        }
        public override void FixedUpdateNetwork()
        {      
            if (GameTimer.Expired(Runner) == false && GameTimer.RemainingTime(Runner).HasValue)
            {
                var timeSpan = TimeSpan.FromSeconds(GameTimer.RemainingTime(Runner).Value);
                var outPut = $"{timeSpan.Minutes:D2} : {timeSpan.Seconds:D2}";
                OutPut= outPut;
            }
            else if (GameTimer.Expired(Runner))
            {
                MatchIsOver = true;
                GameTimer = TickTimer.None;
                OutPut = "Match Finished";
            }
        }
        
        private void OnChangedCountDownTxt()
        {
            gameTimerText.text = OutPut.Value;
        }
    }
}