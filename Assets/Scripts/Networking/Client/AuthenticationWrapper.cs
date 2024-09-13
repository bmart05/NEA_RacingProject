using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Networking.Client
{
    public static class AuthenticationWrapper
    {
        public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

        public static async Task<AuthState> DoAuth(int maxTries = 5)
        {
            if (AuthState == AuthState.Authenticated)
            {
                return AuthState;
            }

            if (AuthState == AuthState.Authenticating)
            {
                await Authenticating();
                return AuthState;
            }

            AuthState = AuthState.Authenticating;

            await SignInAnonymouslyAsync(maxTries);

            return AuthState;
        }

        private static async Task<AuthState> Authenticating()
        {
            Debug.LogWarning("Already authenticating!");
            while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
            {
                await Task.Delay(200);
            }

            return AuthState;
        }

        private static async Task SignInAnonymouslyAsync(int maxTries)
        {
            int tries = 0;
            while (AuthState == AuthState.Authenticating && tries < maxTries)
            {
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                    if (AuthenticationService.Instance.IsAuthorized && AuthenticationService.Instance.IsSignedIn)
                    {
                        AuthState = AuthState.Authenticated;
                        break;
                    }
                }
                catch (AuthenticationException e)
                {
                    Debug.LogError(e);
                    AuthState = AuthState.Error;
                    throw;
                }
                catch (RequestFailedException exception)
                {
                    Debug.LogError(exception);
                    AuthState = AuthState.Error;
                    throw;
                }
                
                
                tries++;
                await Task.Delay(1000); //prevents from too many tries from being sent at once so you don't get rate limited
            }

            if (AuthState != AuthState.Authenticated)
            {
                Debug.LogWarning($"Player was not signed in successfully after {tries} tries");
                AuthState = AuthState.TimeOut;
            }
        }
    }
    public enum AuthState
    {
        NotAuthenticated,
        Authenticating,
        Authenticated,
        Error,
        TimeOut
    }
}