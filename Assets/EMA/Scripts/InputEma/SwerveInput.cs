using System;
using EMA.Scripts.PatternClasses;
using UnityEngine;
using UnityEngine.Events;

namespace EMA.Scripts.InputEma
{
    public class SwerveInput : MonoSingleton<SwerveInput>
    {
        [Header("Properties")]
        [SerializeField] private float deadZone;
        
        [Header("Touch Delta")]
        public UnityEvent<Vector2> Swerve;

        [Header("Check Input")] public UnityEvent<bool> CheckInput;

        public bool inputCheckerBool;
        private Vector2 startTouch;
        private Vector2 swipeDelta;
        
        private void Update()
        {
            if(UnityEngine.Input.touchCount > 0)
            {
                Touch touch = UnityEngine.Input.GetTouch(0);
                if(touch.phase == TouchPhase.Began)
                {
                    startTouch = touch.position;
                }
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) 
                {
                    swipeDelta = touch.position - startTouch;
                    startTouch = touch.position;

                    swipeDelta /= 100f;
                }
                if(touch.phase == TouchPhase.Ended)
                {
                    swipeDelta = Vector2.zero;
                }

                if (Mathf.Abs(swipeDelta.x) < deadZone)
                    swipeDelta.x = 0f;
                if (Mathf.Abs(swipeDelta.y) < deadZone)
                    swipeDelta.y = 0f;
                
                inputCheckerBool = true;
            }

            else {
                swipeDelta = Vector2.zero;
                inputCheckerBool = false;
            }
        }

        private void FixedUpdate()
        {
            CheckInput?.Invoke(inputCheckerBool);
            Swerve?.Invoke(swipeDelta);
        }
    }
}
