﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public enum BattleStatus
    {
        PreBattle,
        InBattle,
        PostBattle,
    }
    public List<Enemy> enemies = new List<Enemy>();
    public List<SinglePlayer> players = new List<SinglePlayer>();
    // default MT
    public SinglePlayer.StratPosition playerPosition = SinglePlayer.StratPosition.MT;
    public Scenario scenario;
    private BattleStatus prevBattleStatus = BattleStatus.PreBattle;
    private BattleStatus battleStatus
    {
        // Check enemies status
        // if all pre -> pre
        // if all post -> post
        // if more than 0 in battle -> in
        get
        {
            bool prebattle = true;
            foreach (Enemy e in enemies)
            {
                switch (e.battleStatus)
                {
                    case EntityBattleStatus.InBattle:
                        prebattle = false;
                        return BattleStatus.InBattle;
                    case EntityBattleStatus.PlayerLose:
                    case EntityBattleStatus.PlayerWin:
                        prebattle = false;
                        break;
                    case EntityBattleStatus.PreBattle:
                        break;
                }
            }
            if (prebattle) return BattleStatus.PreBattle;
            else return BattleStatus.PostBattle;
        }
    }

    // Pending Effects 
    private List<SingleStatus> eventQueue = new List<SingleStatus>();


    //
    // ─── BUFF DEBUFF MANAGEMENT ──────────────────────────────────────────────────────
    //

    private float checkEvery = 3f;

    //
    // ─── UI ─────────────────────────────────────────────────────────────────────────
    //

    private UIManager uIManager;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("RegisterStatusEffect");
        uIManager = GetComponent<UIManager>();
        scenario = GetComponent<Scenario>();
        scenario.Init();
        RegisterEntities();
    }

    // Update is called once per frame
    void Update()
    {
        CheckBattleStatus();
        ApplyEventQueue();
        UpdateUI();
        // if (battleStatus == BattleStatus.InBattle)
        // {
        //     ApplyEventQueue();
        // }
    }

    void InitUI()
    {

    }

    void UpdateUI()
    {

    }

    void RegisterEntities()
    {
        // Find enemies and players in the scene
        foreach (GameObject en in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(en.GetComponent<Enemy>());
        }
        foreach (GameObject en in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(en.GetComponent<SinglePlayer>());
        }
        Debug.Log("BM register enemy " + enemies.Count, this.gameObject);
        Debug.Log("BM register players " + players.Count, this.gameObject);
        foreach (Enemy e in enemies)
        {
            Debug.Log("BM register enemy " + e, this.gameObject);
            e.RegisterEntities();
        }
        foreach (SinglePlayer p in players)
            p.RegisterEntities();
    }

    public void AddEvent(SingleStatus status)
    {
        Debug.Log($"BM AddEvent: {status}");
        eventQueue.Add(status);
    }

    void ApplyEventQueue()
    {
        // TODO: Lock?
        // Debug.Log("BM: Apply all effects.");
        foreach (SingleStatus s in eventQueue)
        {
            s.Apply();
        }
        eventQueue.Clear();
    }

    // called every Interval seconds
    IEnumerator RegisterStatusEffect()
    {
        while (true)
        {
            Debug.Log("BM RegisterStatusEffect", this.gameObject);
            foreach (Enemy e in enemies)
            {
                e.RegisterEffect();
            }
            foreach (SinglePlayer p in players)
            {
                p.RegisterEffect();
            }
            yield return new WaitForSeconds(checkEvery);
        }
    }

    void CheckBattleStatus()
    {
        if (prevBattleStatus != battleStatus)
        {
            if (prevBattleStatus == BattleStatus.PreBattle && battleStatus == BattleStatus.InBattle)
                OnBattleStart();
            else if (prevBattleStatus == BattleStatus.InBattle && battleStatus == BattleStatus.PostBattle)
                OnBattleEnd();
            prevBattleStatus = battleStatus;
        }
    }

    // only deal with players' status
    public void OnBattleStart()
    {
        // pre->in
        // Start Scenario
        // Set active enemies' inbattle=true. this happens if only one enemy detects the player, but not the others.
        Debug.Log("BM: OnBattleStart");
    }

    public void OnBattleEnd()
    {
        // in->end
        // UI transition
        // Reset CD, HP,
        // Reset Scenario
        // reset enemy status to prepare
        Debug.Log("BM: OnBattleEnd");
    }

}
