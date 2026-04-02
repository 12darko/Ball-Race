using _Main.Scripts.Multiplayer.Player.NetworkInput;
using Fusion;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Main.Scripts.Multiplayer.Player
{
    public class NetworkPlayerInputManager : NetworkBehaviour, IBeforeUpdate
    {
        [SerializeField] private NetworkPlayers networkPlayers;

        public NetworkPlayerInput AccumulatedInput;
        public bool resetInput;

        public void BeforeUpdate()
        {
            if (!Object.HasInputAuthority) return;
            if (!networkPlayers.AcceptAnyInput) return;
            if (networkPlayers.PlayerInFinish) return;

            if (resetInput)
            {
                resetInput = false;
                AccumulatedInput = default;
            }

            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            if (mouse != null)
            {
                var mouseDelta = mouse.delta.ReadValue();
                var lookRotationDelta = new Vector3(-mouseDelta.y, mouseDelta.x, 0f);
                AccumulatedInput.NetworkLookDelta = lookRotationDelta;
                AccumulatedInput.NetworkAimForward =
                    networkPlayers.networkPlayersManager.LocalCamera.transform.forward;
                AccumulatedInput.NetworkCameraPosition =
                    networkPlayers.networkPlayersManager.LocalCamera.transform.position;
            }

            if (keyboard != null)
            {
                // ✅ Cursor toggle
                if (keyboard.enterKey.wasPressedThisFrame ||
                    keyboard.numpadEnterKey.wasPressedThisFrame ||
                    keyboard.escapeKey.wasPressedThisFrame)
                {
                    if (Cursor.lockState == CursorLockMode.Locked)
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                    else
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
            }

            NetworkButtons buttons = default;

            if (keyboard != null && mouse != null)
            {
                // ✅ Tamamen yeni Input System'e geçildi
                var camTransform = networkPlayers.networkPlayersManager.LocalCamera.transform;

                float horizontal = 0f;
                float vertical = 0f;

                if (keyboard.aKey.isPressed) horizontal -= 1f;
                if (keyboard.dKey.isPressed) horizontal += 1f;
                if (keyboard.wKey.isPressed) vertical += 1f;
                if (keyboard.sKey.isPressed) vertical -= 1f;

                var camForward = camTransform.forward;
                var camRight = camTransform.right;
                camForward.y = 0;
                camRight.y = 0;
                camForward.Normalize();
                camRight.Normalize();

                var lookCameraDir = (vertical * camForward) + (horizontal * camRight);

                AccumulatedInput.NetworkCameraLookPosition += lookCameraDir;
                AccumulatedInput.NetworkCameraForwardPosition += camForward;

                buttons.Set(NetworkPlayerInputEnums.Jump, keyboard.spaceKey.isPressed);
                buttons.Set(NetworkPlayerInputEnums.UseSkill, keyboard.eKey.isPressed);
                buttons.Set(NetworkPlayerInputEnums.PickUp, keyboard.fKey.isPressed);
                buttons.Set(NetworkPlayerInputEnums.Slot1, keyboard.digit1Key.isPressed);
            }

            AccumulatedInput.NetworkAllButtons =
                new NetworkButtons(AccumulatedInput.NetworkAllButtons.Bits | buttons.Bits);
        }
    }
}