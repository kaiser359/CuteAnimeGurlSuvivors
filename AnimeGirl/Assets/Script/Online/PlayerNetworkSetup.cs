using Unity.Netcode;

using UnityEngine;


public class PlayerNetworkSetup : NetworkBehaviour

{

    public Camera playerCamera;


    public override void OnNetworkSpawn()

    {

        if (!IsOwner)

        {

            // Disable camera for non-local players

            if (playerCamera != null)

                playerCamera.enabled = false;

        }

    }

}