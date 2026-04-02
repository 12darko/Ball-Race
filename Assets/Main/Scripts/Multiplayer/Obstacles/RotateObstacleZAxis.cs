using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Multiplayer.Multiplayer.Obstacles
{
    public class RotateObstacleZAxis : Obstacle
    {
        [SerializeField] private float rotateZMax;
        [SerializeField] private Transform rotatedObject;

        private void Start()
        {
            var random = Random.Range(0f, 1f);
            var rotateValue = random >= .5f ? rotateZMax : -rotateZMax;
            rotatedObject.localRotation = Quaternion.Euler(new Vector3(rotatedObject.localRotation.eulerAngles.x, rotatedObject.localRotation.eulerAngles.y, rotateValue));

            if (rotateValue > 0)
            {
                RotateToRight();
            }
            else
            {
                RotateToLeft();
            }
        }

        private void RotateToRight()
        {
            rotatedObject.DOLocalRotate(new Vector3(rotatedObject.localRotation.eulerAngles.x, rotatedObject.localRotation.eulerAngles.y, rotateZMax), obstacleRotateSpeed).SetEase(Ease.InOutSine)
                .OnComplete(RotateToLeft);
        }

        private void RotateToLeft()
        {
            rotatedObject.DOLocalRotate(new Vector3(rotatedObject.localRotation.eulerAngles.x, rotatedObject.localRotation.eulerAngles.y, -rotateZMax), obstacleRotateSpeed).SetEase(Ease.InOutSine)
                .OnComplete(RotateToRight);
        }
    }
}