using UnityEngine;
using UnityEngine.InputSystem;

public class CreateWall : MonoBehaviour
{
 
    public TeleportOnFireHover aimProvider;


    public Camera mainCamera;


    public GameObject wallPrefab;

  
    public LayerMask groundLayers;

    public float placementWorldZ = 0f;

 
    public float wallCooldown = 5f;
    private float nextWallTime = 0f;

    private void Start()
    {
 
        if (aimProvider == null)
        {
        
            aimProvider = GetComponent<TeleportOnFireHover>();
        }

    
        if (mainCamera == null && aimProvider != null)
        {
            mainCamera = aimProvider.playerCamera;
        }

        if (aimProvider == null || mainCamera == null)
        {
           
        }
    }

  
    public void WallSkill(InputAction.CallbackContext ctx)
    {
       
        if (ctx.canceled || !ctx.performed)
            return;

        if (wallPrefab == null)
        {
            Debug.LogError("Wall Prefab is not assigned.");
            return;
        }

        // Cooldown check
        if (Time.time < nextWallTime)
        {
            Debug.Log("Wall skill is on cooldown.");
            return;
        }

        if (mainCamera == null || aimProvider == null)
            return;


        

       
        Vector2 screenPosition = aimProvider.aimPosition;

        float zDistance = Mathf.Abs(mainCamera.transform.position.z - placementWorldZ);

       
        Vector3 screenPoint = new Vector3(screenPosition.x, screenPosition.y, zDistance);

    
        Vector3 wallWorldPos = mainCamera.ScreenToWorldPoint(screenPoint);

      
        wallWorldPos.z = placementWorldZ;

        Vector2 targetPos2D = new Vector2(wallWorldPos.x, wallWorldPos.y);

        
        //if (groundLayers != 0)
        //{
        //    Collider2D col = Physics2D.OverlapPoint(targetPos2D, groundLayers);

        //    if (col == null)
        //    {
        //        Debug.Log("Invalid placement: Must be placed on a valid surface (LayerMask check failed).");
        //        return; // Stop if placement is invalid
        //    }
        //}

       
        InstantiateWall(wallWorldPos);
        nextWallTime = Time.time + wallCooldown;
    }

    
    private void InstantiateWall(Vector3 position)
    {
      
        Instantiate(wallPrefab, position, Quaternion.identity);
        Debug.Log("Wall instantiated at: " + position);
    }
}