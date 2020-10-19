﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Volpi.ObjectyPool;
public class SingleStatus
{
    protected static int globalStatusID = 0;
    private int statusID;
    public enum EffectType
    {
        EffectOverTime,
        LongLasting, // no countdown
    }
    // Status on Player or Enemy
    public GameObject from, target; //From whom this status comes
    protected float startTime = 0;
    public float duration { get; protected set; } // how long it will last
    public float countdown { get; protected set; } // remaining time
    public bool expired { get; protected set; } = false;
    protected bool lostAfterDeath = true;
    // if this status has effect once attached to an entity
    protected bool effectiveAtOnce = true;
    public EffectType statusEffectType = EffectType.EffectOverTime;
    //
    // ─── METAINFO ───────────────────────────────────────────────────────────────────
    //

    public string name, statusDescription;
    private bool _showIcon = true;
    public bool showIcon
    {
        get
        {
            // dont show icon if expired
            return _showIcon;
        }
        set { _showIcon = value; }
    } // should we show icon on ui   
    public Sprite icon; // prefab
    protected BattleManager bm;


    public SingleStatus(GameObject from, GameObject target, float dur)
    {
        this.from = from;
        this.target = target;
        duration = countdown = dur;
        bm = GameObject.FindGameObjectWithTag(Constants.BM.Tag).GetComponent<BattleManager>();
        statusID = globalStatusID;
        globalStatusID++;
    }

    public static Sprite LoadStatusSprite(string str){
        return Resources.Load<Sprite>($"{Constants.UI.StatusPrefabDir}/{str}");
    }

    public void Update()
    {
        switch (statusEffectType)
        {
            case EffectType.LongLasting:
                break; // dont deal with expiration
            default:
                if (countdown >= 0)
                {
                    countdown -= Time.deltaTime;
                    // Debug.Log($"SingleStatus ({this}) Countdown: {countdown}");
                }
                if (countdown < 0 && !expired)
                {
                    expired = true;
                    Debug.Log($"SingleStatus ({this.name}) Expired. Registering.");
                    RegisterEffect();
                }
                break;
        }
    }

    protected virtual void NormalEffect()
    {
        Debug.Log($"SingleStatus ({this.name}) Normal: From {from.name} to {target.name}", this.target);
    }

    protected virtual void ExpireEffect()
    {
        Debug.Log($"SingleStatus ({this.name}) Expire: From {from.name} to {target.name}", this.target);
        // TODO: show remove status on target
        if (showIcon)
            showEffect();
    }


    public void Apply()
    {
        if (!expired)
        {
            NormalEffect();
        }
        else
        {
            ExpireEffect();
        }
    }

    // called when the status is first attached to an entity
    public virtual void OnAttachedToEntity()
    {
        // TODO: Show status added  on target
        if (showIcon)
            showEffect();
        if (effectiveAtOnce)
        {
            bm.AddEvent(this);
        }
    }

    void showEffect()
    {
        Debug.Log($"SingleStatus showEffect: On {target.name}");
        GameObject damageInfoGO = ObjectyManager.Instance.ObjectyPools[Constants.UI.DamageInfoPoolName].Spawn(Constants.UI.DamageInfoPoolSpawningName);
        DamageTextFollower damageTextFollower = damageInfoGO.GetComponent<DamageTextFollower>();
        damageTextFollower.isDamageInfo = false;
        damageTextFollower.Init(par: target.GetComponent<Entity>().moveInfoCanvas.transform, status: this);
    }

    // Called per period (3 secs)
    public void RegisterEffect()
    {
        Debug.Log($"SingleStatus ({this.name}) RegisterEffect.", this.target);
        bm.AddEvent(this);
    }

    public override int GetHashCode()
    {
        return (name + statusID).GetHashCode();
    }

    public override string ToString()
    {
        return (name + statusID);
    }
}
