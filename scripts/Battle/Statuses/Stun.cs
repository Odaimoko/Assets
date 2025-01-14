﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : SingleStatus
{
    public Stun(GameObject fr, GameObject target, float dur) : base(fr, target, dur)
    {
        name = "眩晕";
        icon = LoadStatusSprite("status_stun");
    }

    protected override void NormalEffect()
    {
        base.NormalEffect();
        SinglePlayer sp = target.GetComponent<SinglePlayer>();
        sp.controllable = false;
    }

    protected override void ExpireEffect()
    {
        base.ExpireEffect();
        SinglePlayer sp = target.GetComponent<SinglePlayer>();
        sp.controllable = true;
    }

}
