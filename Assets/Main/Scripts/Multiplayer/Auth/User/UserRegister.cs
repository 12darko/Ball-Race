using System.Threading.Tasks;
using _Main.Scripts.Multiplayer.UnityCore.Auth;
using _Main.Scripts.Multiplayer.UnityCore.Auth.Platforms;
using UnityEngine;

namespace Multiplayer.Auth
{
    public class UserRegister : Register
    {
        public override async void SignUp(string userName, string password, string email)
        {
            await SignUpType(userName, password, email);
        }

        public override async Task SignUpType(string userName, string password, string email)
        {
            switch (PlatformTypesController.Instance.PlatformType)
            {
                case LoginPlatformType.Unity:
                    await PlatformUnity.SignUpWithUnityUsernamePasswordAsync(userName, password, saveData);
                    break;
                case LoginPlatformType.PlayFab:
                
                    break;
                case LoginPlatformType.PlayerPrefs:
                    break;
                default:
                    break;
            }
        }
    }
}