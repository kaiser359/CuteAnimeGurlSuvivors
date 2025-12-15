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

    [Header("Reticle")]
    public Sprite reticle;
    [Tooltip("World Z position where reticle will be placed (usually 0)")]
    public float reticleWorldZ = 0f;
    [Tooltip("Local scale applied to instantiated reticle sprite")]
    public float reticleScale = 1f;
    private GameObject _reticleInstance;
    private SpriteRenderer _reticleSR;

    private void Awake()
    {
        aimPosition = new Vector2(Screen.width, Screen.height) / 2;

        // Create a world-space reticle sprite (if a sprite was provided)
        if (reticle != null)
        {
            _reticleInstance = new GameObject("Reticle");
            _reticleSR = _reticleInstance.AddComponent<SpriteRenderer>();
            _reticleSR.sprite = reticle;
            _reticleSR.sortingOrder = 1000; // render on top of most things
            _reticleInstance.transform.localScale = Vector3.one * reticleScale;
        }

        // clamp initially to camera viewport
        ClampAimToCamera();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        aimPosition += aimDelta;
        ClampAimToCamera();
        UpdateReticlePosition();
    }


    private void ClampAimToCamera()
    {
        // if no camera assigned use full screen bounds
        if (playerCamera == null)
        {
            aimPosition.x = Mathf.Clamp(aimPosition.x, 0f, Screen.width);
            aimPosition.y = Mathf.Clamp(aimPosition.y, 0f, Screen.height);
            return;
        }

        // use camera pixel rect so cursor stays inside the camera viewport (handles split-screen / viewport adjustments)
        Rect pr = playerCamera.pixelRect;
        aimPosition.x = Mathf.Clamp(aimPosition.x, pr.xMin, pr.xMax);
        aimPosition.y = Mathf.Clamp(aimPosition.y, pr.yMin, pr.yMax);
    }

    private void UpdateReticlePosition()
    {
        if (_reticleInstance == null || playerCamera == null)
            return;

        // compute world position at the desired reticleWorldZ plane
        float zDistance = Mathf.Abs(playerCamera.transform.position.z - reticleWorldZ);
        Vector3 screenPoint = new Vector3(aimPosition.x, aimPosition.y, zDistance);
        Vector3 world = playerCamera.ScreenToWorldPoint(screenPoint);
        world.z = reticleWorldZ;
        _reticleInstance.transform.position = world;
    }

   
    public void Teleport(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            return;

        if (Time.time < nextTeleportTime)
            return;

        if (playerCamera == null) return;

        float zDistance = Mathf.Abs(playerCamera.transform.position.z - transform.position.z);
        Vector3 screenPoint = new Vector3(aimPosition.x, aimPosition.y, zDistance);
        Vector3 mouseWorldPos = playerCamera.ScreenToWorldPoint(screenPoint);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        Collider2D hit = Physics2D.OverlapPoint(mousePos2D, fireLayer);

        if (hit != null && hit.CompareTag("Fire"))
        {
            transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z);
            nextTeleportTime = Time.time + cooldown;
        }
    }

    public void Aim(InputAction.CallbackContext ctx)
    {
        Vector2 delta = ctx.ReadValue<Vector2>();
        // vertical no longer inverted; delta applied directly
        aimDelta = delta;
    }
    
    private void OnDestroy()
    {
        if (_reticleInstance != null)
            Destroy(_reticleInstance);
    }
}