using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class TeleportOnFireHover : MonoBehaviour
{
    //this script had becomed bizare.its the now a "screen" which creates a cursor for most abilities.

    public Camera playerCamera;

   
    public LayerMask fireLayer;

   
    public float cooldown = 5f;
    private float nextTeleportTime = 0f;

    private Vector2 aimDelta;
    public Vector2 aimPosition;

   
    public Sprite reticle;
   
    public float reticleWorldZ = 0f;
    
    public float reticleScale = 1f;
    private GameObject _reticleInstance;
    private SpriteRenderer _reticleSR;

    public CinemachineConfiner2D confiner; 

    private void Awake()
    {
        GameObject boundingObject = GameObject.Find("Dark background vr3_0");
        if (boundingObject != null)
        {
            confiner.BoundingShape2D = boundingObject.GetComponent<Collider2D>();
        }
        aimPosition = new Vector2(Screen.width, Screen.height) / 2;

        
        if (reticle != null)
        {
            _reticleInstance = new GameObject("Reticle");
            _reticleSR = _reticleInstance.AddComponent<SpriteRenderer>();
            _reticleSR.sprite = reticle;
            _reticleSR.sortingOrder = 1000; 
            _reticleInstance.transform.localScale = Vector3.one * reticleScale;
        }

     
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
      
        if (playerCamera == null)
        {
            aimPosition.x = Mathf.Clamp(aimPosition.x, 0f, Screen.width);
            aimPosition.y = Mathf.Clamp(aimPosition.y, 0f, Screen.height);
            return;
        }

       
        Rect pr = playerCamera.pixelRect;
        aimPosition.x = Mathf.Clamp(aimPosition.x, pr.xMin, pr.xMax);
        aimPosition.y = Mathf.Clamp(aimPosition.y, pr.yMin, pr.yMax);
    }

    private void UpdateReticlePosition()
    {
        if (_reticleInstance == null || playerCamera == null)
            return;

        
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
       
        aimDelta = delta;
    }
    
    private void OnDestroy()
    {
        if (_reticleInstance != null)
            Destroy(_reticleInstance);
    }
}