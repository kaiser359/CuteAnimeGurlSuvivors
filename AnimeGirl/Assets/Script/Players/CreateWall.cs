using UnityEngine;
using UnityEngine.InputSystem;

public class CreateWall : MonoBehaviour
{
    public PlayerHealth playersss;
    private bool used;
    [SerializeField] private float timing; 

    private void Start()
    {
 
   
    }

  
    public void WallSkill(InputAction.CallbackContext ctx)
    {
       
        if (ctx.canceled || !ctx.performed)
            return;
        playersss.health += 10;


    }

}