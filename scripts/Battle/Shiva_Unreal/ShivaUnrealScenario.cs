﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ShivaUnrealScenario : Scenario
{
    private Enemy Shiva;
    [SerializeField]
    private string P1MusicPath = "music/祖堅正慶 - 雪上の足跡 ～蛮神シヴァ前哨戦～";
    private string P3MusicPath = "music/祖堅正慶 - 忘却の彼方 ～蛮神シヴァ討滅戦～";

    //
    // ─── ANIMATOR PARAMETERS ────────────────────────────────────────────────────────
    //


    private int hashShivaHP = Animator.StringToHash("ShivaHP");
    private int hashIsSword = Animator.StringToHash("isSword");
    private int hashNext = Animator.StringToHash("Next");
    private int hashSecondPhaseInP3 = Animator.StringToHash("SecondPhaseInP3");
    private int hashPhase = Animator.StringToHash("Phase");
    private int hashAddsTime = Animator.StringToHash("AddsTime");

    private int hash_Shiva_Unreal_3_Wand = Animator.StringToHash("Base Layer.Shiva_Unreal_3_Wand");
    private int hash_Shiva_Unreal_3_Ring_Bow = Animator.StringToHash("Base Layer.Shiva_Unreal_3_Ring_Bow");
    private int hash_Shiva_Unreal_Enrage = Animator.StringToHash("Base Layer.Shiva_Unreal_Enrage");
    private int hash_Shiva_Unreal_3_Sword = Animator.StringToHash("Base Layer.Shiva_Unreal_3_Sword");
    private int hash_Shiva_Unreal_1_Wand = Animator.StringToHash("Base Layer.Shiva_Unreal_1_Wand");
    private int hash_Shiva_Unreal_2_Adds_Player_Lose = Animator.StringToHash("Base Layer.Shiva_Unreal_2_Adds_Player_Lose");
    private int hash_Shiva_Unreal_2_Adds_Shiva_Invul = Animator.StringToHash("Base Layer.Shiva_Unreal_2_Adds_Shiva_Invul");
    private int hash_Shiva_Unreal_Final_Player_Win = Animator.StringToHash("Base Layer.Shiva_Unreal_Final_Player_Win");
    private int hash_Shiva_Unreal_1_Normal = Animator.StringToHash("Base Layer.Shiva_Unreal_1_Normal");
    private int hash_Shiva_Unreal_1_Sword = Animator.StringToHash("Base Layer.Shiva_Unreal_1_Sword");
    private int hash_Shiva_Unreal_2_Adds = Animator.StringToHash("Base Layer.Shiva_Unreal_2_Adds");
    private int hash_Shiva_Unreal_Diamond_Dust = Animator.StringToHash("Base Layer.Shiva_Unreal_Diamond_Dust");
    private int hash_Shiva_Unreal_0_AutoAtk = Animator.StringToHash("Base Layer.Shiva_Unreal_0_AutoAtk");
    private int hash_Shiva_Unreal_2_Adds_Player_Win = Animator.StringToHash("Base Layer.Shiva_Unreal_2_Adds_Player_Win");
    private int hash_Shiva_Unreal_0 = Animator.StringToHash("Base Layer.Shiva_Unreal_0");

    private Constants.GameSystem.ScenarioDictStruct infoStruct;

    //
    // ─── BATTLE INFO ─────────────────────────────────────────────────
    //

    private ShivaStanceGroup stanceGroup;

    public override void Init()
    {
        base.Init();
        infoStruct = Constants.GameSystem.boss2meta[SupportedBoss.Shiva_Unreal];
        foreach (SinglePlayer singlePlayer in players)
        {
            singlePlayer.target = Shiva.gameObject;
        }
        foreach (Enemy enemy in enemies)
        {
            enemy.aggro = new Dictionary<GameObject, int>(aggro);
        }
        audioSource.clip = Resources.Load<AudioClip>(P1MusicPath);
        audioSource.Play();
        scenarioAnimator = GetComponent<Animator>();
        scenarioAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(infoStruct.animControllerPath);
        Shiva.healthPoint = 60000;
        stanceGroup = new ShivaStanceGroup(Shiva.gameObject, Shiva.gameObject);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        switch (scenarioAnimator.GetInteger(hashPhase))
        {
            case 2:
                CheckShivaHP();
                float addsTime = scenarioAnimator.GetFloat(hashAddsTime);
                scenarioAnimator.SetFloat(hashAddsTime, addsTime + Time.deltaTime);
                break;
            case 3:
                CheckShivaHP();
                break;
        }
    }

    public override void GenerateEntities()
    {
        base.GenerateEntities();
        // gen enemies
        GameObject enemyPrefab = Resources.Load<GameObject>(Constants.GameSystem.boss2meta[SupportedBoss.Shiva_Unreal].modelPrefabPath);
        GameObject ShivaParent = Instantiate(enemyPrefab, new Vector3(0, .1f, 6.4f), Quaternion.identity);
        Shiva = ShivaParent.transform.Find("Shiva").GetComponent<Enemy>();
        SceneManager.MoveGameObjectToScene(ShivaParent, SceneManager.GetSceneByName("Battle"));
        enemies.Add(Shiva);
    }



    public void Slowdown()
    {
        Debug.Log("Shiva_ex: SlowDown!!!!!");
        controlledPlayer.AddStatusGroup(new SlowdownGroup(Shiva.gameObject, controlledPlayer.gameObject, 6f));
        // foreach (SinglePlayer singlePlayer in players)
        // {
        //     singlePlayer.AddStatusGroup(new SlowdownGroup(Shiva.gameObject, singlePlayer.gameObject, 6f));
        // }
    }

    public void Absolute_Zero()
    {
        Debug.Log("Shiva_ex: Casting Absolute Zero...");

        foreach (SinglePlayer singlePlayer in players)
        {
            Shiva.GetComponent<Enemy>().AddStatusGroup(new CastGroup(Shiva.gameObject, Shiva.gameObject, 4f,
            new DealDamage(Shiva.gameObject, singlePlayer.gameObject, 100, "Absolute Zero", Constants.Battle.RaidWideDistance)));
        }
    }

    //
    // ─── STATEMACHINE CHANGE ────────────────────────────────────────────────────────
    //



    public void TriggerNext()
    {
        Debug.Log($"Next Triggered.");
        scenarioAnimator.SetTrigger(hashNext);
    }

    public void SetRandomSword()
    {
        scenarioAnimator.SetBool(hashIsSword, Random.Range(0f, 1f) > .5);
    }

    public void Shiva_Unreal_0_Next()
    {
        if (Shiva.inBattle)
        {
            TriggerNext();
            SetRandomSword();
        }
    }
    public void CheckShivaHP()
    {
        float hpPercent = Shiva.healthPoint / Shiva.maxHP;
        // Debug.Log($"CheckShivaHP: {hpPercent}");
        scenarioAnimator.SetFloat(hashShivaHP, hpPercent);
    }

    public void Shiva_Unreal_0_AutoAtk_Enter()
    {
        Shiva.AddStatusGroup(stanceGroup);
        stanceGroup.ChangeStance(ShivaStanceGroup.StanceEnum.None);
    }

    public void Shiva_Unreal_1_Sword_sp_Enter()
    {
        Debug.Log("Shiva_Unreal_1_Sword_sp_Enter");
        stanceGroup.ChangeStance(ShivaStanceGroup.StanceEnum.Sword);
    }

    public void Shiva_Unreal_1_Wand_sp_Enter()
    {
        Debug.Log("Shiva_Unreal_1_Wand_sp_Enter");
        stanceGroup.ChangeStance(ShivaStanceGroup.StanceEnum.Wand);

    }
    public void Shiva_Unreal_1_Normal_Enter()
    {
        Debug.Log("Shiva_Unreal_1_Normal_Enter");
        stanceGroup.ChangeStance(ShivaStanceGroup.StanceEnum.None);
    }

    public void Shiva_Unreal_1_Next()
    {
        CheckShivaHP();
        TriggerNext();
    }

    public void Shiva_Unreal_2_Adds_Enter()
    {
        // Spawn Adds
        // Init Adds time counter
        scenarioAnimator.SetInteger(hashPhase, 2);
        scenarioAnimator.SetFloat(hashAddsTime, 10f);


    }

    public void Shiva_Unreal_Diamond_Dust_Enter()
    {

    }
    public void Shiva_Unreal_3_Start_Enter()
    {
        if (!scenarioAnimator.GetBool(hashSecondPhaseInP3))
        {
            // TODO reset timing
            SetRandomSword();
        }
        audioSource.clip = Resources.Load<AudioClip>(P3MusicPath);
        audioSource.Play();
        scenarioAnimator.SetInteger(hashPhase, 3);
    }

    public void Shiva_Unreal_3_Ring_Bow_Enter()
    {
    }

    public void Shiva_Unreal_3_Ring_Bow_Next()
    {
        Debug.Log("Shiva_Unreal_3_Ring_Bow_Next");
        TriggerNext();
    }

    public void Shiva_Unreal_3_Next()
    {
        Debug.Log("Shiva_Unreal_3_Next");
        if (scenarioAnimator.GetBool(hashSecondPhaseInP3))
        {
            Debug.Log("hashSecondPhaseInP3 the second phase has ended.");
            scenarioAnimator.SetBool(hashSecondPhaseInP3, false);
        }
        else
        {
            Debug.Log("hashSecondPhaseInP3 the first phase has ended.");
            scenarioAnimator.SetBool(hashSecondPhaseInP3, true);

        }
        TriggerNext();
    }
}
