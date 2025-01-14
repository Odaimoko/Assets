﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusGroup
{
    // The Buff/Debuff Manager to handle the status expiration and effect
    // Shoul dbe attached to every player or enemy
    public List<SingleStatus> statuses = new List<SingleStatus>(); // Should be overridden, with a fixed size?
    public GameObject from, target;
    public virtual bool expired
    {
        get
        {
            bool isExpire = true;
            foreach (SingleStatus s in statuses)
            {
                if (!s.expired)
                {
                    return false;
                }
            }
            return isExpire;
        }
    }
    public bool showIcon
    {
        get
        {

            bool shouldShow = true;
            foreach (SingleStatus s in statuses)
            {
                if (!s.showIcon)
                {
                    return false;
                }
            }
            return shouldShow;
        }
    }
    public string name;


    public StatusGroup(GameObject from, GameObject target)
    {
        this.from = from;
        this.target = target;
    }

    public virtual void Update()
    {
        foreach (SingleStatus s in statuses)
        {
            s.Update();
        }
    }

    public virtual void MergeStatus(StatusGroup another){
        // TODO: Merge two status group with the same effect
    }

    public void Add(SingleStatus s)
    {
        // Assume s' from and target is properly set
        statuses.Add(s); 
        s.OnAttachedToEntity();
    }

    public virtual void RegisterEffect()
    {
        // each group will apply their effects differently
        Debug.Log($"Status Group ({this}) RegisterEffect.", this.target);
        foreach (SingleStatus s in statuses)
        {
            if (!s.expired)
            {
                s.RegisterEffect();
            }
        }
    }

    public override string ToString()
    {
        return $"StatusGroup: {name} ({from.name}->{target.name}).";
    }

    public override int GetHashCode()
    {
        return (name + from.name).GetHashCode();
    }
}
