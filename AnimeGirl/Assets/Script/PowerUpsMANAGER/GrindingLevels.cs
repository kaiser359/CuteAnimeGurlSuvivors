using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GrindingLevels : MonoBehaviour
{
public int baselevelToLevelUp = 5;
public int currentLevel = 1;
public int curenteXp = 0;
public int xpNeeded;
public Image XpBar;
    public AudioSource AudioSource;

    public void AddXP(int xp)
    {
        curenteXp += xp;
        CheckLevelUp();
    }

    private void Awake()
    {
        xpNeeded = 5;
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
            AudioSource.Play();
        }
    }
    private void Update()
    {

        XpBar.fillAmount = Mathf.Clamp((float)curenteXp / xpNeeded, 0, 1);
    }

    public void AddXP(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            return;

        AddXP(7);
    }

}// GrindingLevels.AddXP(value); to add xp

