using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Enter : MonoBehaviour
{

    public FSM_SoundCheck bossAI;


    void Start()
    {
       // bossAI = GetComponent<FSM_SoundCheck>();
    }

    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (bossAI != null)
            {
                bossAI.ActivateAI();
            }
            else
            {
                Debug.LogError("Boss AI reference is not set in Boss_Enter script!");
            }

            // 선택적: 트리거를 한 번만 작동하게 하려면 이 스크립트나 게임 오브젝트를 비활성화할 수 있습니다.
            // gameObject.SetActive(false);
        }
    }
}
