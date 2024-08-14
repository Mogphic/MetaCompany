using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shovel : MonoBehaviour
{
    private BoxCollider col;
    private float time = 0f;
    [SerializeField] private AudioClip[] ImpactSounds;
    [SerializeField] private AudioClip grabSound;
    [SerializeField] private AudioClip swingSound;
    [SerializeField] private AudioClip readySound;
    private AudioSource audioSource;
    private bool hitOnce = false;
    private bool readyOnce = false;

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
            if (InputManager.instance.PlayerAttackStarted() && readyOnce == false)
            {
                readyOnce = true;
                audioSource.PlayOneShot(readySound);
            }
            if (InputManager.instance.PlayerAttackImacted())
            {
                col.enabled = true;
                audioSource.PlayOneShot(swingSound);
                hitOnce = false;
                readyOnce = false;
                StartCoroutine(Timer());
            }
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.3f);
        col.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hitOnce == false)
        {
            hitOnce = true;
            audioSource.PlayOneShot(ImpactSounds[Random.Range(0, ImpactSounds.Length)]);
        }
        else
        {
            return;
        }
        
        if (other.CompareTag("Enemy"))
        {
            FSM_SoundCheck enemyFSM = other.GetComponent<FSM_SoundCheck>();
            NutCrack enemyFSM2 = other.GetComponent<NutCrack>();
            if (enemyFSM != null)
            {
                enemyFSM.TakeDamage(100.0f);
            }
            
            
            if(enemyFSM2 != null)
            {
                enemyFSM2.TakeDamage(35.0f);
            }
            
        }
    }
}
