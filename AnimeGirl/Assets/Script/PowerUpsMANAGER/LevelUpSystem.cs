using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelUpSystem : MonoBehaviour
{

    public static LevelUpSystem Instance;

    [Header("Abilities")]
    public List<AbilityData> abilities = new List<AbilityData>();

   
    [HideInInspector] public List<int> abilityLevels = new List<int>();

    [Header("UI")]
    public CanvasGroup levelUpCanvas;   
    public Transform cardsParent;       
    public GameObject cardPrefab;    

    [Header("Settings")]
    public int cardsToShow = 3;
    public bool allowDuplicates = true; 

    [Header("Game hooks (optional)")]
    public PlayerStats playerStats;   

  
    private List<int> currentSelectionIndices = new List<int>();
    private List<GameObject> spawnedCards = new List<GameObject>();
    private System.Random rng = new System.Random();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

       
        EnsureLevelsSize();

        
        if (levelUpCanvas != null)
        {
            levelUpCanvas.alpha = 0f;
            levelUpCanvas.blocksRaycasts = false;
            levelUpCanvas.interactable = false;
        }
    }

    private void EnsureLevelsSize()
    {
        if (abilityLevels == null) abilityLevels = new List<int>();
        while (abilityLevels.Count < abilities.Count) abilityLevels.Add(0);
        if (abilityLevels.Count > abilities.Count)
            abilityLevels.RemoveRange(abilities.Count, abilityLevels.Count - abilities.Count);
    }

  
    public void OnPlayerLeveledUp()
    {
        ShowLevelUp();
    }

    public void ShowLevelUp()
    {
        if (abilities == null || abilities.Count == 0)
        {
            Debug.LogWarning("LevelUpSystem: No abilities defined.");
            return;
        }

        EnsureLevelsSize();
        ClearSpawned();

        // pick indices
        currentSelectionIndices.Clear();
        for (int i = 0; i < cardsToShow; i++)
        {
            int idx = rng.Next(0, abilities.Count);
            currentSelectionIndices.Add(idx);
            if (!allowDuplicates)
            {
                
                int attempts = 0;
                while (currentSelectionIndices.Count(x => x == idx) > 1 && attempts < 20)
                {
                    idx = rng.Next(0, abilities.Count);
                    currentSelectionIndices[i] = idx;
                    attempts++;
                }
            }
        }
        
      
        for (int i = 0; i < currentSelectionIndices.Count; i++)
        {
            int abilityIndex = currentSelectionIndices[i];
            var data = abilities[abilityIndex];

            GameObject go = Instantiate(cardPrefab, cardsParent, false);
            spawnedCards.Add(go);

            LevelUpCard card = go.GetComponent<LevelUpCard>();
            if (card == null)
            {
                Debug.LogError("LevelUpSystem: cardPrefab missing LevelUpCard component.");
                continue;
            }

       
            int occurrences = currentSelectionIndices.Count(x => x == abilityIndex);

            
            string title = data.abilityName;
            string desc = $"{data.description}\n(+{occurrences} level{(occurrences > 1 ? "s" : "")})";

           
            card.Setup(
                title,
                desc,
                data.icon,
                occurrences,
                () => OnCardPicked(abilityIndex, occurrences)
            );
        }

        transform.parent.GetComponentInChildren<EventSystem>().SetSelectedGameObject(spawnedCards.FirstOrDefault().transform.GetChild(0).gameObject);

        // show UI
        if (levelUpCanvas != null)
        {
            levelUpCanvas.alpha = 1f;
            levelUpCanvas.blocksRaycasts = true;
            levelUpCanvas.interactable = true;
        }
    }

    private void OnCardPicked(int abilityIndex, int occurrences)
    {
        if (abilityIndex < 0 || abilityIndex >= abilities.Count)
        {
            Debug.LogWarning("LevelUpSystem: invalid ability picked.");
            CloseLevelUp();
            return;
        }

        EnsureLevelsSize();

      
        int current = abilityLevels[abilityIndex];
        int newLevel = current + occurrences;
        int max = abilities[abilityIndex].maxLevel;
        if (max > 0 && newLevel > max) newLevel = max;
        abilityLevels[abilityIndex] = newLevel;

       
        ApplyAbilityEffect(abilityIndex, newLevel, occurrences);

       
        CloseLevelUp();
    }

    private void ApplyAbilityEffect(int abilityIndex, int newLevel, int addedLevels)
    {
        var data = abilities[abilityIndex];
       
        float computedValue = data.baseValue + (newLevel * data.valuePerLevel);

        
        switch (data.abilityType)
        {
            case AbilityType.Damage:
                if (playerStats != null) playerStats.SetDamage(computedValue);
              
                break;

            case AbilityType.MaxHealth:
                if (playerStats != null) playerStats.SetMaxHealth(computedValue);
                
                break;

            case AbilityType.MoveSpeed:
                if (playerStats != null) playerStats.SetMoveSpeed(computedValue);
                
                break;

            case AbilityType.FireRate:
                if (playerStats != null) playerStats.SetFireRate(computedValue);
           
                break;

            case AbilityType.CritChance:
                if (playerStats != null) playerStats.SetCritChance(computedValue);
              
                break;

            case AbilityType.LootChance:
                if (playerStats != null) playerStats.SetLootChance(computedValue);

                break;

            case AbilityType.DashCooldown:
                if (playerStats != null) playerStats.SetDashCooldown(computedValue);
              
                break;

            case AbilityType.Shield:
                if (playerStats != null) playerStats.AddShield(computedValue * addedLevels);
                
                break;
            case AbilityType.CritDamage:
                if (playerStats != null) playerStats.SetCritDamage(computedValue * addedLevels);
              
                break;
            case AbilityType.MikuBean:
                if (playerStats != null) playerStats.SetMikuBean(computedValue);
               
                break;
            case AbilityType.Lifesteal:
                if (playerStats != null) playerStats.SetMikuBean(computedValue);
             
                break;
            case AbilityType.Necromancy:
                if (playerStats != null) playerStats.SetNecromancyAmount(computedValue);
               
                break;

            
            case AbilityType.Meteor:
               
                if (playerStats != null)
                {
                    float damage = data.GetValue(newLevel);        
                    float cooldown = data.GetCooldown(newLevel);    
                    playerStats.SetMeteorDamage(damage);
                    playerStats.SetMeteorCooldown(cooldown);

                   
                   
                    // playerStats.SetMeteorRadius(data.someRadiusField);

                    Debug.Log($"[Ability] Meteor: damage={damage}, cooldown={cooldown}, level={newLevel}");
                }
                else
                {
                    Debug.Log($"[Ability] Meteor would be applied: level {newLevel}, damage {data.GetValue(newLevel)}, cd {data.GetCooldown(newLevel)}");
                }
                break;


            default:
                Debug.Log($"[Ability] Unhandled ability type {data.abilityType}");
                break;
        }
    }

    private void ClearSpawned()
    {
        foreach (var go in spawnedCards)
            if (go != null) Destroy(go);
        spawnedCards.Clear();
        currentSelectionIndices.Clear();
    }

    public void CloseLevelUp()
    {
        ClearSpawned();
        if (levelUpCanvas != null)
        {
            levelUpCanvas.alpha = 0f;
            levelUpCanvas.blocksRaycasts = false;
            levelUpCanvas.interactable = false;
        }
    }


}


// LevelUpSystem.Instance.OnPlayerLeveledUp(); use it whne player levels up
