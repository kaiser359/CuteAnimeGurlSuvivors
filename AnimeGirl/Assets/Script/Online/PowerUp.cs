
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    //public static PowerUp instance;

    //public List<PowerData> Powers = new List<PowerData>();

    //private const float baseInterval = 0.1f;






    //private float currentDelay;

    //private void Awake()
    //{
    //    instance = this;
    //}

    //private void Update()
    //{
    //    for (int i = 0; i < Powers.Count; i++)
    //    {
    //        HandlePowers(Powers[i], i);
    //    }

    //        }
    //    }
    //}

    //// --- Called from button events ---
    //public void OnPowerButtonDown(int index)
    //{
    //    // Safety checks before using index
    //    if (index <= 0 || index > Powers.Count)
    //    {
    //        Debug.LogWarning($"Invalid power index: {index}");
    //        return;
    //    }

    //    heldPowerIndex = index - 1;
    //    isHolding = true;
    //    currentDelay = baseRepeatDelay;
    //    holdTimer = baseRepeatDelay; // start countdown, prevents instant 2nd buy
    //    BuyUpgrade(index); // Single instant click
    //}

    //public void OnPowerButtonUp()
    //{
    //    isHolding = false;
    //    heldPowerIndex = -1;
    //}

    //private void HandlePowers(PowerData power, int index)
    //{
    //    if (power.baseInterval != baseInterval)
    //        power.baseInterval = baseInterval;

    //    int level 

    //    BigDouble cost = GetPowerCost(power, level);
    //    BigDouble production = GetProduction(power, level);

    //    if (level > 0)
    //    {
    //        power.currentTime += Time.deltaTime;
    //        if (power.currentTime >= power.baseInterval)
    //        {
    //            switch (index)
    //            {
    //                case 0: // Power 1
    //                    ClickerManager.instance.moneyValue = production.ToDouble();
    //                    break;
    //                case 1: // Power 2
    //                    GoldCoinSpawner.instance.EnableSpawner(true);
    //                    double goldCoinChance = power.baseProduction + (level * power.productionIncreaseRate);
    //                    GoldCoinSpawner.instance.spawnChance = (float)goldCoinChance;
    //                    break;
    //                case 2: // Power 3
    //                    if (GachaManager.instance != null)
    //                        GachaManager.instance.isGacha = true;
    //                    double luckBoost = power.baseProduction + (level * power.productionIncreaseRate);
    //                    GachaManager.instance.luckBonus = (float)luckBoost;
    //                    break;
    //                case 3: // Power 4
    //                    double critChance = power.baseProduction + (level * power.productionIncreaseRate);
    //                    ClickerManager.instance.critChance = (float)critChance;

    //                    break;
    //                case 4: // Power 5
    //                    double newTime = power.baseProduction - (level * power.productionIncreaseRate);
    //                    UpgradeManager.instance.baseInterval = (float)newTime;
    //                    break;
    //                case 5: // Power 6
    //                    double offlineIncome = power.baseProduction + (level * power.productionIncreaseRate);
    //                    SaveDataController.currentData.offlineEarningsMultiplier = (float)offlineIncome;
    //                    break;
    //                case 6: // Power 7
    //                    // Add custom power logic here
    //                    break;
    //            }

    //            power.currentTime = 0f;
    //        }
    //    }

    //    // Update UI
    //    if (power.levelText != null)
    //        power.levelText.text = $"Level {level}";

    //    if (power.costText != null)
    //        power.costText.text = $"${NumberFormatter.Format(cost)}";

    //    if (power.rateText != null)
    //    {
    //        if (level > 0)
    //            power.rateText.text = GetPowerDescription(index, production);
    //        else
    //            power.rateText.text = "";
    //    }

    //    // Handle max level
    //    if (power.levelText != null && power.maxLevel > 0 && level >= power.maxLevel)
    //    {
    //        power.levelText.text = "MAX LEVEL";
    //        if (power.costText != null)
    //            power.costText.text = "";
    //    }
    //}



    //public void BuyUpgrade(int index)
    //{
    //    index -= 1;
    //    if (index < 0 || index >= Powers.Count) return;

    //    var power = Powers[index];
    //    int level = SaveDataController.currentData.powerLevels[index];

    //    if (power.maxLevel > 0 && level >= power.maxLevel)
    //    {
    //        Debug.Log($"{power.upgradeName} is already at max level!");
    //        return;
    //    }



    //    foreach (var (p, i) in Powers.Select((p, i) => (p, i)))
    //        HandlePowers(p, i);
    //}



   
    //}
}

