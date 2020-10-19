﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageVulnerable : StackableStatus
{
    protected float multi;
    public DamageVulnerable(GameObject fr, GameObject target, float dur, float dmgMultiplier, int maxStacks = 1) : base(fr, target, dur, maxStacks)
    {
        // Just Exists, do not take effect until got damage;
        // We defer Damage calculation to the status that cause damage.
        this.maxStacks = maxStacks;
        multi = dmgMultiplier;
    }
    public virtual float GetMultiplier(List<Constants.Battle.DamageType> types)
    {
        // Exposed to other things to get its multiplier.
        return multi;
    }
}
