using UnityEngine;
using UnityEngine.InputSystem;

public class CreateWall : MonoBehaviour
{
    public PlayerHealth playersss;
    private bool used;
    [SerializeField] private float timing = 10f; 

    private void Start()
    {
 
   
    }

  
    public void WallSkill(InputAction.CallbackContext ctx)
    {
       
        if (ctx.canceled || !ctx.performed)
            return;
        if (used)
            return;
        playersss.health += 10;
        used = true;


    }
    private void Update()
    {
        if (used)
        {
            timing -= Time.deltaTime;
            if (timing <= 0f)
            {
                used = false;
                timing = 10f;
            }
        }
    }

}