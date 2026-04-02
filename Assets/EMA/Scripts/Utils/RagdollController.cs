using System;
using UnityEngine;

namespace EMA.Scripts.Utils
{
    public class RagdollController : MonoBehaviour
    {
        [SerializeField] private Rigidbody mainRb;
         [SerializeField] Transform parentBoneTr;
         [SerializeField] private GameObject otherCollidersParent;
         [SerializeField] private Animator animator;
         [SerializeField] private bool ragdollIsActiveAtFirst;
         
         private Vector3 parentBoneDefaultPos;
         
         private Rigidbody[] rigidbodies;
         public Rigidbody[] Rigidbodies => rigidbodies;


         private void Start()
         {
             rigidbodies = parentBoneTr.GetComponentsInChildren<Rigidbody>();
             parentBoneDefaultPos = parentBoneTr.localPosition;
             SetActiveOfRagdoll(ragdollIsActiveAtFirst);
         }

         public void SetActiveOfRagdoll(bool value)
         {
             animator.enabled = !value;
             foreach (var _rigidbody in rigidbodies) {
                 _rigidbody.isKinematic = !value;
                 _rigidbody.GetComponent<Collider>().isTrigger = !value;
                 mainRb.useGravity = !value;
             }

             if (!value) {
                 parentBoneTr.localPosition = parentBoneDefaultPos;
             }
             
             otherCollidersParent.SetActive(!value);
         }
         
    }
}
