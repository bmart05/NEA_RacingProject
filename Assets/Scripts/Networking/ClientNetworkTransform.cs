using Unity.Netcode;
using Unity.Netcode.Components;

namespace Networking
{
    
    //credit to dapper dino udemy tutorial + unity docs
    public class ClientNetworkTransform : NetworkTransform
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            CanCommitToTransform = IsOwner;
        }

        protected override void Update()
        {
            CanCommitToTransform = IsOwner;
            base.Update();
            if (NetworkManager != null)
            {
                if (NetworkManager.IsConnectedClient || NetworkManager.IsListening)
                {
                    if (CanCommitToTransform)
                    {
                        TryCommitTransformToServer(transform,NetworkManager.LocalTime.Time);
                    }
                }
            }
        }

        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}