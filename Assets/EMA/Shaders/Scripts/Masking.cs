using UnityEngine;

namespace EMA.Shaders.Scripts
{
    public static class Masking
    {
        public static void MaskAnObject(MeshRenderer mesh, int renderQueue = 3002)
        {
            mesh.material.renderQueue = renderQueue;
        }
        
        public static void MaskAnObject(SkinnedMeshRenderer mesh, int renderQueue = 3002)
        {
            mesh.material.renderQueue = renderQueue;
        }
    }
}
