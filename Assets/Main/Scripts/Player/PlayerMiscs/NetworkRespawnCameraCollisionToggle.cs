using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Behaviour = UnityEngine.Behaviour;


namespace Main.Scripts.Player
{
    public class NetworkRespawnCameraCollisionToggle: NetworkBehaviour
    {
        [Header("Drag your camera collision / obstruction component here")]
        [SerializeField] private Behaviour cameraCollisionBehaviour;

        [SerializeField] private float disableSeconds = 0.25f;

        public override void Spawned()
        {
            if (!Object.HasInputAuthority)
            {
                enabled = false;
                return;
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
        public void RPC_DisableCollisionTemporarily()
        {
            if (cameraCollisionBehaviour == null) return;
            StartCoroutine(ToggleRoutine());
        }

        private IEnumerator ToggleRoutine()
        {
            cameraCollisionBehaviour.enabled = false;
            yield return new WaitForSeconds(disableSeconds);
            if (cameraCollisionBehaviour != null)
                cameraCollisionBehaviour.enabled = true;
        }
    }
}