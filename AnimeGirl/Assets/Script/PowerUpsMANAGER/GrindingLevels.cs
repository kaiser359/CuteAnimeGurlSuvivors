using UnityEngine;

public class GrindingLevels : MonoBehaviour
{
public int baselevelToLevelUp = 5;
public int currentLevel = 1;
public int curenteXp = 0;
public int xpNeeded;

    public void AddXP(int xp)
    {
        curenteXp += xp;
        CheckLevelUp();
    }
    private void CheckLevelUp()
    {
        xpNeeded = baselevelToLevelUp * currentLevel;
        while (curenteXp >= xpNeeded)
        {
            curenteXp -= xpNeeded;
            currentLevel++;
            LevelUpSystem.Instance.OnPlayerLeveledUp();
            Debug.Log("Leveled up to level " + currentLevel);
            xpNeeded = baselevelToLevelUp * currentLevel;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && Input.GetKeyDown(KeyCode.O))
        {
            AddXP(7);
        }
    }
}// GrindingLevels.AddXP(value); to add xp

