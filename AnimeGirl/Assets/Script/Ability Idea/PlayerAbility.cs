using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerAbilities : MonoBehaviour
{
    void Update()
    {
        // input handling
        if (Input.GetMouseButtonDown(1))
        {
            TryUseAbility(AbilityType.Meteor);
        }
    }

    // Added missing method definition
    private void TryUseAbility(AbilityType abilityType)
    {
        // TODO: Implement ability usage logic here
        Debug.Log($"Trying to use ability: {abilityType}");
    }
}
