using System;
 using Multiplayer.Auth;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Auth
{
    
    public class AuthMessages : MonoBehaviour
    {

        [SerializeField] private AnonymousLogin login;
        private void OnEnable()
        {
            //login.OnAuthStarted += AnonymousLoginOnOnAuthStarted;
          //  login.OnAuthFailed += AnonymousLoginOnOnAuthFailed;
        }

        private void OnDisable()
        {
            //login.OnAuthStarted -= AnonymousLoginOnOnAuthStarted;
           // login.OnAuthFailed -= AnonymousLoginOnOnAuthFailed;
        }
        private void AnonymousLoginOnOnAuthFailed(object sender, EventArgs e)
        {
        //  ResponseMessageBoxManager.Instance.ShowResponseBox("LOGIN RESPONSE", "Login Failed...",1);
        }

        private void AnonymousLoginOnOnAuthStarted(object sender, EventArgs e)
        {
           // ResponseMessageBoxManager.Instance.ShowResponseBox("LOGIN RESPONSE", "Login Started...",1);
        }

        
    }
}