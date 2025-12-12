using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    public int paperFigurines = 0;
    public GameObject paperFigures;
    public void AddPaperFigurine(int amount)
    {
        paperFigurines += amount;
        Debug.Log("Picked up +1 paper figurine! Total: " + paperFigurines);
    }

    public void Use(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            return;

        if (paperFigurines >= 1)
        {
            Instantiate(paperFigures, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            paperFigurines = paperFigurines - 1;

        }
    }

}//might usse this script for storing items later on. 
