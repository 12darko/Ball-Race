using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.Obstacles.PlatformColorsChanger
{
    public class NetworkPlatformPartColorTarget : MonoBehaviour
    {
        [Header("Target Renderer")] [SerializeField]
        private Renderer targetRenderer;

        [Tooltip("Renklenecek material slot index'i (0,1,2...)")] [SerializeField]
        private int materialIndex = 0;

        private MaterialPropertyBlock _mpb;

        private void Awake()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponent<Renderer>();

            _mpb = new MaterialPropertyBlock();
        }

        public void ApplyColor(Color color, string colorProperty)
        {
            if (targetRenderer == null) return;

            int matCount = targetRenderer.sharedMaterials != null ? targetRenderer.sharedMaterials.Length : 0;
            if (matCount == 0) return;

            int idx = Mathf.Clamp(materialIndex, 0, matCount - 1);

            // slot bazlı MPB (kritik nokta)
            targetRenderer.GetPropertyBlock(_mpb, idx);
            _mpb.SetColor(colorProperty, color);
            targetRenderer.SetPropertyBlock(_mpb, idx);
        }
    }
}