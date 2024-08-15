

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
    private DamageFlashEffect damageFlashEffect;

    private void Start()
    {
        damageFlashEffect = FindObjectOfType<DamageFlashEffect>();
        curHp = maxHp;
    }

    public void UpdateHp(float value)
    {
        curHp -= value;
        if (gameObject.name.Contains("Player"))
        {
            detectHealthReduction();
            if (damageFlashEffect != null)
            {
                damageFlashEffect.TriggerDamageFlash();
            }
        }
        if (curHp > maxHp)
        {
            curHp = maxHp;
        }
        else if (curHp <= 0)
        {
            curHp = 0;
            Die();
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

        StartCoroutine(DelayedDeath());
    }


    private IEnumerator DelayedDeath()
    {
        yield return new WaitForSeconds(deathUIDelay);
        UIManager.instance.PlayerDie();
    }
}




