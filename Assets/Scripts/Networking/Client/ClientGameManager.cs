using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine.SceneManagement;

namespace Networking.Client
{
    public class ClientGameManager
    {
        private const string MenuSceneName = "MainMenu";
        
        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();
            AuthState authState = await AuthenticationWrapper.DoAuth();
            if (authState == AuthState.Authenticated)
            {
                return true;
            }

            return false; //failed to authenticate
        }
        
        public void GoToMenu()
        {
            SceneManager.LoadScene(MenuSceneName);
        }
    }
}