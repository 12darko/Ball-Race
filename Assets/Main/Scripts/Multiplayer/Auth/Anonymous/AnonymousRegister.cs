using System.Threading.Tasks;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.UnityCore.Auth.Anonymous
{
    public class AnonymousRegister : Register
    {
        public override async void SignUp(string userName, string password, string email)
        {
            await SignUpType(null, null,null);
        }

        public override async Task SignUpType(string userName, string password, string email)
        {
            //Boş
        }
    }
}

 