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

    
    private void TryUseAbility(AbilityType abilityType)
    {

    }
}
