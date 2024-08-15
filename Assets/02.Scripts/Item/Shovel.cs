using System.Collections;
using UnityEngine;

public class Shovel : MonoBehaviour
{
    private BoxCollider col;
    [SerializeField] private AudioClip[] ImpactSounds;
    [SerializeField] private AudioClip grabSound;
    [SerializeField] private AudioClip swingSound;
    [SerializeField] private AudioClip readySound;
    [SerializeField] private AudioClip chargingSound;
    private AudioSource audioSource;
    private bool hitOnce = false;
    private bool isCharging = false;
    private float chargeStartTime;
    private float currentChargeDuration;

    [SerializeField] private float minChargeDuration = 0.75f;
    [SerializeField] private float attackCooldown = 0f;
    private float lastAttackTime;

    private Coroutine attackCoroutine;

    private void OnEnable()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(grabSound);
        }
    }

    private void Start()
    {
        col = GetComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //if (inventorySystem.canAttack)
        {
            if (InputManager.instance.PlayerAttackStarted() && !isCharging && Time.time - lastAttackTime >= attackCooldown)
            {
                StartCharging();
            }

            if (InputManager.instance.PlayerAttackImacted() && isCharging)
            {
                StopCharging();
                Attack();
            }

            if (isCharging)
            {
                currentChargeDuration = Time.time - chargeStartTime;
            }
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        chargeStartTime = Time.time;
        audioSource.PlayOneShot(readySound);
        audioSource.loop = true;
        audioSource.clip = chargingSound;
        audioSource.Play();
    }

    private void StopCharging()
    {
        isCharging = false;
        audioSource.loop = false;
        audioSource.Stop();
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        float attackPower = Mathf.Max(1, currentChargeDuration / minChargeDuration);

        col.enabled = true;
        audioSource.PlayOneShot(swingSound);
        hitOnce = false;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(AttackCoroutine(attackPower));
    }

    private IEnumerator AttackCoroutine(float attackPower)
    {
        yield return new WaitForSeconds(0.3f);
        col.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hitOnce)
        {
            hitOnce = true;
            audioSource.PlayOneShot(ImpactSounds[Random.Range(0, ImpactSounds.Length)]);

            if (other.CompareTag("Enemy"))
            {
                DealDamageToEnemy(other);
            }
        }
    }

    private void DealDamageToEnemy(Collider enemy)
    {
        float damageMultiplier = Mathf.Max(1, currentChargeDuration / minChargeDuration);

        FSM_SoundCheck enemyFSM = enemy.GetComponent<FSM_SoundCheck>();
        NutCrack enemyFSM2 = enemy.GetComponent<NutCrack>();

        if (enemyFSM != null)
        {
            enemyFSM.TakeDamage(100.0f * damageMultiplier);
        }

        /*
        if (enemyFSM2 != null)
        {
            enemyFSM2.TakeDamage(35.0f * damageMultiplier);
        }
        */
    }
}