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

}
