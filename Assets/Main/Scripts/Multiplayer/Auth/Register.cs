using System.Threading.Tasks;
using Multiplayer.CloudSave;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.UnityCore.Auth
{
    public abstract class Register : MonoBehaviour
    {
        public SaveData saveData;
        public LoadData loadData;
        
        public abstract void SignUp(string userName, string password, string email);
        public abstract Task SignUpType(string userName, string password,string email);
    }
}