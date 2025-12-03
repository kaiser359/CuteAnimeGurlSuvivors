using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int paperFigurines = 0;
    public GameObject paperFigures;
    public void AddPaperFigurine(int amount)
    {
        paperFigurines += amount;
        Debug.Log("Picked up +1 paper figurine! Total: " + paperFigurines);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && paperFigurines >= 1)
        {
            Instantiate(paperFigures, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            paperFigurines = paperFigurines - 1;
            
        }
    }

}//might usse this script for storing items later on. 
