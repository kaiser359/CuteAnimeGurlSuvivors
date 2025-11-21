using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int paperFigurines = 0;

    public void AddPaperFigurine(int amount)
    {
        paperFigurines += amount;
        Debug.Log("Picked up +1 paper figurine! Total: " + paperFigurines);
    }
}//might usse this script for storing items later on. 
