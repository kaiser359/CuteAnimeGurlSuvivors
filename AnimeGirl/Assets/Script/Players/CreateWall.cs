using UnityEngine;
using UnityEngine.InputSystem;

public class CreateWall : MonoBehaviour
{
    public PlayerHealth playersss;
    private bool used = false;
    [SerializeField] private float timing = 5f;
    private float initialTiming;

    private void Start()
    {
        
        initialTiming = timing;
    }

    public void WallSkill(InputAction.CallbackContext ctx)
    {
     
        if (!ctx.performed)
            return;
        if (used)
            return;
        if (playersss == null)
        {
           
            return;
        }

       
        playersss.health = Mathf.Min(playersss.health + 15f, playersss.maxHealth);
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
                timing = initialTiming;
            }
        }
    }
}