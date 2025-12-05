using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // existing fields...
    public float baseDamage = 1f;
    public float baseMaxHealth = 100f;
    public float baseMoveSpeed = 5f;
    public float baseFireRate = 1f;
    public float baseCritChance = 0f;
    public float baseLootChance = 0.05f;
    public float dashCooldown = 1f;
    private float shield = 0f;
    public float baseCritDamage = 2f;
    public float baseMikuBean;
    public float baselifesteal = 0.1f;
    public float necromancyAmount = 5f;

    // Meteor specific (tuned by LevelUpSystem when Meteor is chosen)
    [Header("Meteor")]
    public float meteorDamage = 50f;
    public float meteorCooldown = 20f;
    public float meteorRadius = 2.5f; // default radius, can also be changed on prefab

    // Standard setters (used by LevelUpSystem)
    public void SetDamage(float value) => baseDamage = value;
    public void SetMaxHealth(float value) => baseMaxHealth = value;
    public void SetMoveSpeed(float value) => baseMoveSpeed = value;
    public void SetFireRate(float value) => baseFireRate = value;
    public void SetCritChance(float value) => baseCritChance = value;
    public void SetLootChance(float value) => baseLootChance = value;
    public void SetDashCooldown(float value) => dashCooldown = value;
    public void AddShield(float amount) { shield += amount; }
    public void SetCritDamage(float value) => baseCritDamage = value;

    public void SetMikuBean(float value) => baseMikuBean = value;

    public void setlifesteal(float value) => baselifesteal = value;

    public void SetNecromancyAmount(float value) => necromancyAmount = value;



    // Meteor setters
    public void SetMeteorDamage(float damage)
    {
        meteorDamage = damage;
        Debug.Log($"MeteorDamage set -> {meteorDamage}");
    }

    public void SetMeteorCooldown(float cd)
    {
        meteorCooldown = cd;
        Debug.Log($"MeteorCooldown set -> {meteorCooldown}");
    }

    public void SetMeteorRadius(float r)
    {
        meteorRadius = r;
        Debug.Log($"MeteorRadius set -> {meteorRadius}");
    }
}
