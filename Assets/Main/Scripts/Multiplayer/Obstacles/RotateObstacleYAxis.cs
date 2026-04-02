using System;
using DG.Tweening;
using Fusion;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.Obstacles
{
    public class RotateObstacleYAxis : Obstacle
    {
 
        [SerializeField] private float rotateYAxis;
        [SerializeField] private Transform rotatedObject;

        private void Start()
        {
             rotatedObject.DOLocalRotate(rotatedObject.transform.localRotation.eulerAngles +(Vector3.up * rotateYAxis), obstacleRotateSpeed, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        }

        /*public override void FixedUpdateNetwork()
        {
         //   rotatedObject.Rotate(new Vector3(rotatedObject.rotation.x, rotateYAxis * Runner.DeltaTime* obstacleRotateSpeed, 0f));
        }
 */
    }
}