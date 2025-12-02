using Unity.Netcode;
using UnityEngine;


public class TeleportOnFireHover : MonoBehaviour

{

    [Header("Player Camera")]

    public Camera playerCamera;


    [Header("LayerMask for Clickable Objects")]

    public LayerMask fireLayer;


    [Header("Teleport Settings")]

    public float cooldown = 5f;

    private float nextTeleportTime = 0f;


    void Update()

    {

        // If still in cooldown, nope

        if (Time.time < nextTeleportTime)

            return;


        // Convert mouse to world using the player's camera

        Vector3 mouseWorldPos = playerCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);


        // Raycast for objects with Fire layer

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0f, fireLayer);


        if (hit.collider != null && hit.collider.CompareTag("Fire"))

        {

            if (Input.GetMouseButtonDown(0))

            {

                // Call the Server to teleport online only

                TeleportServerRpc(hit.collider.transform.position);
                //transform.position = hit.collider.transform.position;

                // Set cooldown

                nextTeleportTime = Time.time + cooldown;

            }

        }

    }


    [ServerRpc]

    void TeleportServerRpc(Vector3 pos)

    {

        TeleportClientRpc(pos);

    }


    [ClientRpc]

    void TeleportClientRpc(Vector3 pos)

    {

        transform.position = pos;

    }

}