using System;
using UnityEngine;

namespace EMA.Shaders.Scripts
{
    public class WobbleEffect : MonoBehaviour
    {
        private Material woobleMaterial;

        private Renderer rend;
        private Vector3 lastPos;
        private Vector3 velocity;
        private Vector3 lastRot;
        private Vector3 angularVelocity;
        private float maxWooble = .03f;
        private float wobbleSpeed = 1f;
        private float recovery = 1f;
        private float wobbleAmountX;
        private float wobbleAmountZ;
        private float wobbleAmountToAddX;
        private float wobbleAmountToAddZ;
        private float pulse;
        private float time = .5f;

        private void Start()
        {
            rend = GetComponent<Renderer>();
        }

        private void Update()
        {
            if (woobleMaterial == null)
                return;

            time += Time.deltaTime;
            wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * recovery);
            wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * recovery);

            pulse = 2 * Mathf.PI * wobbleSpeed;
            wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
            wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

            woobleMaterial.SetFloat("_WoobleX", 0f);
            woobleMaterial.SetFloat("_WoobleZ", 0f);

            velocity = (lastPos - transform.position) / Time.deltaTime;
            angularVelocity = transform.rotation.eulerAngles - lastRot;

            wobbleAmountToAddX += Mathf.Clamp(velocity.x + (angularVelocity.z * .2f) * maxWooble, -maxWooble, maxWooble);
            wobbleAmountToAddZ += Mathf.Clamp(velocity.z + (angularVelocity.x * .2f) * maxWooble, -maxWooble, maxWooble);

            lastPos = transform.position;
            lastRot = transform.rotation.eulerAngles;
        }

        public void AssignWoobleMaterial(Material woobleMat)
        {
            if (woobleMaterial != null) {
                woobleMaterial.SetFloat("_Fill", 1f);
                woobleMaterial.SetFloat("_Fill", 1f);
            }
            woobleMaterial = woobleMat;
        }
    }
}
