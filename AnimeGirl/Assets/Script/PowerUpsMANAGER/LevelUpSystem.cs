using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpSystem : MonoBehaviour
{

    public static LevelUpSystem Instance;

    [Header("Abilities")]
    public List<AbilityData> abilities = new List<AbilityData>();

    // tracks current levels for each ability (parallel to abilities list)
    [HideInInspector] public List<int> abilityLevels = new List<int>();

    [Header("UI")]
    public CanvasGroup levelUpCanvas;   // panel that shows/hides the level-up UI
    public Transform cardsParent;       // layout transform to spawn cards under
    public GameObject cardPrefab;       // prefab with LevelUpCard

    [Header("Settings")]
    public int cardsToShow = 3;
    public bool allowDuplicates = true; // duplicates allowed among the 3 shown cards

    [Header("Game hooks (optional)")]
    public PlayerStats playerStats;     // optional reference to player stats to apply effects immediately

    // runtime
    private List<int> currentSelectionIndices = new List<int>();
    private List<GameObject> spawnedCards = new List<GameObject>();
    private System.Random rng = new System.Random();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // ensure abilityLevels matches abilities count
        EnsureLevelsSize();

        // hide UI initially
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

    /// <summary>
    /// Call this from your XP/leveling system when the player levels up.
    /// </summary>
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
                // if duplicates not allowed, ensure unique selection
                int attempts = 0;
                while (currentSelectionIndices.Count(x => x == idx) > 1 && attempts < 20)
                {
                    idx = rng.Next(0, abilities.Count);
                    currentSelectionIndices[i] = idx;
                    attempts++;
                }
            }
        }

        // spawn cards
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

            // count duplicates for this ability in the current choice
            int occurrences = currentSelectionIndices.Count(x => x == abilityIndex);

            // prepare display strings
            string title = data.abilityName;
            string desc = $"{data.description}\n(+{occurrences} level{(occurrences > 1 ? "s" : "")})";

            // setup click callback
            card.Setup(
                title,
                desc,
                data.icon,
                occurrences,
                () => OnCardPicked(abilityIndex, occurrences)
            );
        }

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

        // apply levels, respecting maxLevel if set
        int current = abilityLevels[abilityIndex];
        int newLevel = current + occurrences;
        int max = abilities[abilityIndex].maxLevel;
        if (max > 0 && newLevel > max) newLevel = max;
        abilityLevels[abilityIndex] = newLevel;

        // apply the actual gameplay effect
        ApplyAbilityEffect(abilityIndex, newLevel, occurrences);

        // close
        CloseLevelUp();
    }

    private void ApplyAbilityEffect(int abilityIndex, int newLevel, int addedLevels)
    {
        var data = abilities[abilityIndex];
        // Compute value from base + level * increment
        float computedValue = data.baseValue + (newLevel * data.valuePerLevel);

        // Use a switch/case like your original script
        switch (data.abilityType)
        {
            case AbilityType.Damage:
                if (playerStats != null) playerStats.SetDamage(computedValue);
                else Debug.Log($"[Ability] Damage set to {computedValue} (level {newLevel})");
                break;

            case AbilityType.MaxHealth:
                if (playerStats != null) playerStats.SetMaxHealth(computedValue);
                else Debug.Log($"[Ability] MaxHealth set to {computedValue} (level {newLevel})");
                break;

            case AbilityType.MoveSpeed:
                if (playerStats != null) playerStats.SetMoveSpeed(computedValue);
                else Debug.Log($"[Ability] MoveSpeed set to {computedValue} (level {newLevel})");
                break;

            case AbilityType.FireRate:
                if (playerStats != null) playerStats.SetFireRate(computedValue);
                else Debug.Log($"[Ability] FireRate set to {computedValue} (level {newLevel})");
                break;

            case AbilityType.CritChance:
                if (playerStats != null) playerStats.SetCritChance(computedValue);
                else Debug.Log($"[Ability] CritChance set to {computedValue} (level {newLevel})");
                break;

            case AbilityType.LootChance:
                if (playerStats != null) playerStats.SetLootChance(computedValue);
                else Debug.Log($"[Ability] LootChance set to {computedValue} (level {newLevel})");
                break;

            case AbilityType.DashCooldown:
                if (playerStats != null) playerStats.SetDashCooldown(computedValue);
                else Debug.Log($"[Ability] DashCooldown set to {computedValue} (level {newLevel})");
                break;

            case AbilityType.Shield:
                if (playerStats != null) playerStats.AddShield(computedValue * addedLevels);
                else Debug.Log($"[Ability] Shield added {computedValue * addedLevels} (addedLevels {addedLevels})");
                break;
            case AbilityType.CritDamage:
                if (playerStats != null) playerStats.SetCritDamage(computedValue * addedLevels);
                else Debug.Log($"[Ability] critdamage added {computedValue * addedLevels} (addedLevels {addedLevels})");
                break;
            case AbilityType.MikuBean:
                if (playerStats != null) playerStats.SetMikuBean(computedValue);
                else Debug.Log($"[Ability] MikuBean set to {computedValue} (level {newLevel})");
                break;
            case AbilityType.Lifesteal:
                if (playerStats != null) playerStats.SetMikuBean(computedValue);
                else Debug.Log($"[Ability] MikuBean set to {computedValue} (level {newLevel})");
                break;
            case AbilityType.Necromancy:
                if (playerStats != null) playerStats.SetNecromancyAmount(computedValue);
                else Debug.Log($"[Ability] Necromancy set to {computedValue} (level {newLevel})");
                break;

            // inside LevelUpSystem.ApplyAbilityEffect(...)
            case AbilityType.Meteor:
                // Primary value = meteor damage, cooldown handled by GetCooldown
                if (playerStats != null)
                {
                    float damage = data.GetValue(newLevel);         // uses baseValue & valuePerLevel
                    float cooldown = data.GetCooldown(newLevel);    // uses cooldownBase & cooldownReductionPerLevel
                    playerStats.SetMeteorDamage(damage);
                    playerStats.SetMeteorCooldown(cooldown);

                    // Optionally allow setting radius from ability data (if you add a field)
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

#if UNITY_EDITOR
    // convenience in editor to force show
    [ContextMenu("DEBUG_ShowLevelUp")]
    private void DebugShow() => ShowLevelUp();
#endif
}


// LevelUpSystem.Instance.OnPlayerLeveledUp(); use it whne player levels up
