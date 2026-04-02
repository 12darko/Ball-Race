using UnityEngine;

namespace EMA.Scripts.MyShortcuts
{
    public class InitialSettings : MonoBehaviour
    {
        public Camera cam;
        public float resolution = 720f;
        
        private void Start()
        {
            Application.targetFrameRate = 60;
            var _x = resolution * cam.aspect;
            Screen.SetResolution((int)_x, (int)resolution, true);
        }
    }
}

