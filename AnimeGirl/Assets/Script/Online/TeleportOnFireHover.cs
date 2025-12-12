using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class TeleportOnFireHover : MonoBehaviour

{

    [Header("Player Camera")]

    public Camera playerCamera;


    [Header("LayerMask for Clickable Objects")]

    public LayerMask fireLayer;


    [Header("Teleport Settings")]

    public float cooldown = 5f;

    private float nextTeleportTime = 0f;

    private Vector2 aimDelta;
    public Vector2 aimPosition;

    public Sprite reticle;

    private void Awake()
    {
        aimPosition = new Vector2(Screen.width, Screen.height) / 2;
    }

    private void Update()
    {
        aimPosition += aimDelta;
    }

    private void OnGUI()
    {
        Graphics.DrawTexture(new Rect(aimPosition, Vector2.one * 200), reticle.texture);
    }

    // TODO: Test
    public void Teleport(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            return;

        // If still in cooldown, nope
        if (Time.time < nextTeleportTime)
            return;

        // Convert mouse to world using the player's camera

        Vector3 mouseWorldPos = playerCamera.ScreenToWorldPoint(aimPosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        // Raycast for objects with Fire layer

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0f, fireLayer);

        if (hit.collider != null && hit.collider.CompareTag("Fire"))
        {
            // Call the Server to teleport online only
            transform.position = hit.collider.transform.position;

            // Set cooldown
            nextTeleportTime = Time.time + cooldown;
        }
    }

    public void Aim(InputAction.CallbackContext ctx)
    {
        Vector2 delta = ctx.ReadValue<Vector2>();
        delta.y *= -1;

        aimDelta = delta;
    }
}