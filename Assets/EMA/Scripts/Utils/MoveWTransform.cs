using UnityEngine;

namespace EMA.Scripts.Utils
{
    public class MoveWTransform : MonoBehaviour
    {
        [SerializeField] private Transform otherTr;
        private Transform _tr;

        private void Start()
        {
            _tr = transform;
        }

        private void Update()
        {
            _tr.position = otherTr.position;
        }
    }
}
