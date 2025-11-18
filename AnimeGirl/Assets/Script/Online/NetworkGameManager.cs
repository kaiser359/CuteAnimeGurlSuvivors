using Unity.Netcode;

using UnityEngine;


public class NetworkGameManager : MonoBehaviour

{

    public GameObject playerPrefab;


    void OnGUI()

    {

        // Simple debugging UI

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)

        {

            if (GUI.Button(new Rect(10, 10, 150, 40), "Start Host"))

            {

                NetworkManager.Singleton.StartHost();

            }

            if (GUI.Button(new Rect(10, 60, 150, 40), "Start Client"))

            {

                NetworkManager.Singleton.StartClient();

            }

        }

    }

}