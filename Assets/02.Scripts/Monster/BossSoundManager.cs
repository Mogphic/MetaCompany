using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BossSoundManager : MonoBehaviour
{
    public AudioSource AudioSource;
    
    //클립 로드
    public AudioClip AudioClip;


    public FSM_SoundCheck fsm;

    public FSM_SoundCheck.EEnemyState FSM_SoundCheck;

    private void Update()
    {
        if (fsm.currentState != FSM_SoundCheck)
        {
            ChangeState();
            FSM_SoundCheck = fsm.currentState;
        }
        
    }

    void ChangeState()
    {
        switch (FSM_SoundCheck)
        {
            case global::FSM_SoundCheck.EEnemyState.Sleep:
                break;
        }
    }
}
