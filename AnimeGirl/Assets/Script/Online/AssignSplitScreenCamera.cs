using Unity.Netcode;

using UnityEngine;


public class AssignSplitScreenCamera : NetworkBehaviour

{

    public Camera cam1;

    public Camera cam2;


    static int playerIndex = 0;


    public override void OnNetworkSpawn()

    {

        if (!IsOwner) return;


        Camera assignedCam;


        if (playerIndex == 0) assignedCam = cam1;

        else assignedCam = cam2;


        playerIndex++;


        assignedCam.enabled = true;


        // Tell teleport script which camera to use

        GetComponent<TeleportOnFireHover>().playerCamera = assignedCam;

    }

}