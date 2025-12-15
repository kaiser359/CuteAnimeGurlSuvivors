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
    }

    private void Update()
    {
        aimPosition += aimDelta;
        UpdateReticlePosition();
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
        // removed inversion so vertical movement matches cursor movement
        aimDelta = delta;
    }
    
    private void OnDestroy()
    {
        if (_reticleInstance != null)
            Destroy(_reticleInstance);
    }
}