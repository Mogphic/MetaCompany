﻿

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpSystem : MonoBehaviour
{
    // private float curHp;
    public float curHp;
    public bool isHitFromEnemy = false;
    [SerializeField] private float maxHp;

    [SerializeField] private float deathUIDelay = 3.0f;
    private VignetteController vignetteController;
    private RagdollExample ragdoll;
    private bool isPlayer = false;
    public string attackerName = string.Empty;

    private void Start()
    {
        vignetteController = FindObjectOfType<VignetteController>();
        isPlayer = gameObject.name.Contains("Player");
        if (isPlayer == true)
        {
            ragdoll = FindAnyObjectByType<RagdollExample>();
        }
        
        curHp = maxHp;
    }

    public void UpdateHp(float value, string name)
    {
        attackerName = name;
        curHp -= value;
        if (isPlayer == true)
        {
            detectHealthReduction();
            if (vignetteController != null)
            {
                UpdateVignetteEffect();
            }
            UIManager.instance.PlayerHit(curHp / maxHp);
        }
        if (curHp > maxHp)
        {
            curHp = maxHp;
        }
        else if (curHp <= 0)
        {
            curHp = 0;
            //Die();
        }
    }

    private void UpdateVignetteEffect()
    {
        if (curHp <= 30)
        {
            vignetteController.ApplySustainedRedEffect();
        }
        else if (curHp <= 65)
        {
            vignetteController.TriggerSingleBlink();
        }
        else
        {
            vignetteController.StopEffect();
        }
    }

    public void detectHealthReduction()
    {
        isHitFromEnemy = true;
    }

    public void Die()
    {
        //죽는 연출 생성
        //플레이어는 래그돌 > 플레이어 기능 정지
        // UIManager.instance.PlayerDie();
        ragdoll.ToggleRagdoll();
        InputManager.instance.EnableInput(false);
        StartCoroutine(DelayedDeath());
    }


    private IEnumerator DelayedDeath()
    {
        yield return new WaitForSeconds(deathUIDelay);
        UIManager.instance.PlayerDie();
    }
}




