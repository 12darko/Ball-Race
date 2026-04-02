using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.SpawnController
{
    public class SpawnerArea: MonoBehaviour
    {
        [SerializeField] private Collider collider;

        public Collider Collider
        {
            get => collider;
            set => collider = value;
        }
    }
}