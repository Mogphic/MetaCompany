using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BossSoundManager : MonoBehaviour
{
    


    public FSM_SoundCheck fsm;

    public FSM_SoundCheck.EEnemyState FSM_SoundCheck_state;

    // 오디오 소스 컴포넌트
    private AudioSource audioSource;

    // 각 상태별 오디오 클립
    public AudioClip sleepSound;
    public AudioClip awakeSound;
    public AudioClip awakeUpSound;
    public AudioClip roarSound;
    public AudioClip idleSound;
    public AudioClip walkClamSound;
    public AudioClip rotateSound;
    public AudioClip chaseSound;
    public AudioClip attackSound;
    public AudioClip dieSound;
    public AudioClip biteSound;

    public float walkSpeed = 1.0f;
    public float chaseSpeed = 1.0f;



    public Coroutine nowCoroutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        fsm = FindAnyObjectByType<FSM_SoundCheck>();
    }
    private void Update()
    {
        if (fsm.currentState != FSM_SoundCheck_state)
        {
            PlayStateSound(fsm.currentState);
            FSM_SoundCheck_state = fsm.currentState;
        }
        
    }


    void PlayStateSound(global::FSM_SoundCheck.EEnemyState state)
    {
        if (audioSource == null) return;

        if(nowCoroutine != null)
        {
            StopCoroutine(nowCoroutine);
        }

        AudioClip clipToPlay = null;

        switch (state)
        {
            case global::FSM_SoundCheck.EEnemyState.Sleep:
                clipToPlay = sleepSound;
                break;
            case global::FSM_SoundCheck.EEnemyState.Awake:
                clipToPlay = awakeSound;
                break;
            case global::FSM_SoundCheck.EEnemyState.Awake_up:
                clipToPlay = awakeUpSound;
                break;
            case global::FSM_SoundCheck.EEnemyState.Roar:
                clipToPlay = roarSound;
                break;
            case global::FSM_SoundCheck.EEnemyState.Idle:
                clipToPlay = idleSound;
                break;
            case global::FSM_SoundCheck.EEnemyState.WalkClam:
                clipToPlay = walkClamSound;
                nowCoroutine = StartCoroutine(PlayWalkSound(walkSpeed));
                return;
            case global::FSM_SoundCheck.EEnemyState.Rotate_:
                clipToPlay = rotateSound;
                break;
            case global::FSM_SoundCheck.EEnemyState.Chase_:
                clipToPlay = chaseSound;
                nowCoroutine = StartCoroutine(PlayWalkSound(walkSpeed));
                return;
            case global::FSM_SoundCheck.EEnemyState.Attack:
                clipToPlay = attackSound;
                break;
            case global::FSM_SoundCheck.EEnemyState.Die_Dog:
                clipToPlay = dieSound;
                break;
            case global::FSM_SoundCheck.EEnemyState.Bite:
                clipToPlay = biteSound;
                break;
        }

        if (clipToPlay != null)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }

    public IEnumerator PlayWalkSound(float speed)
    {
        while (true)
        {
            audioSource.Play();
            yield return new WaitForSeconds(speed);
        }

    }
}
