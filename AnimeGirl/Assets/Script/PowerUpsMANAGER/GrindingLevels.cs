using UnityEngine;
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
        if (Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.O))
        {
            AddXP(7);
        }

        XpBar.fillAmount = Mathf.Clamp((float)curenteXp / xpNeeded, 0, 1);
    }

}// GrindingLevels.AddXP(value); to add xp

