using System;
using UnityEngine;

namespace EMA.Scripts.Utils
{
    public class SphereGizmoDrawer : MonoBehaviour
    {
        [SerializeField] private Transform tr;
        [SerializeField] private float radius;
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(tr.position, radius);
        }
    }
}
