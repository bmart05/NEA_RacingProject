using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Networking.Client
{
    public class ClientSingleton : MonoBehaviour
    {
        private static ClientSingleton _instance;

        public static ClientSingleton Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<ClientSingleton>();
                if (_instance == null)
                {
                    return null;
                }

                return _instance;
            }
        }

        private ClientGameManager _gameManager;
        
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async Task CreateClient()
        {
            _gameManager = new ClientGameManager();
            await _gameManager.InitAsync();
        }
    }
}