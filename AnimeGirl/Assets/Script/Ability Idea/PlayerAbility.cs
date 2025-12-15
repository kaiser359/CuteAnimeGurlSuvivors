using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{

    public void UseAbility(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            return;

        TryUseAbility(AbilityType.Meteor);
    }

    // Added missing method definition
    private void TryUseAbility(AbilityType abilityType)
    {
        // TODO: Implement ability usage logic here
        Debug.Log($"Trying to use ability: {abilityType}");
    }
}
