using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutCr : MonoBehaviour
{
    public enum NutState
    {
        Idle,
        Rotate,
        Fire,
        Die
    }

    public NutState nutState = NutState.Idle;
    public Animator animator;
    public Coroutine stateCoroutine;

    public float waitTime = 5f;
    private bool isDetectingPlayer = true; // 플레이어 감지 여부 제어
    public float hp = 9;
    private float maxHp = 100f;
    public GameObject player;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        ChangeState(NutState.Idle); // 초기 상태 설정
    }

    public void ChangeState(NutState state)
    {
        nutState = state;

        Debug.Log(nutState + " >>  " + state );

        if (stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
        }

        animator.speed = state == NutState.Idle ? 0 : 1;
        switch (state)
        {
            case NutState.Idle:
                stateCoroutine = StartCoroutine(Idle());
                break;
            case NutState.Rotate:
                isDetectingPlayer = true;
                stateCoroutine = StartCoroutine(Rotate());
                break;
            case NutState.Fire:
                animator.SetTrigger("fire");
                stateCoroutine = StartCoroutine(Fire());
                break;
            case NutState.Die:
                stateCoroutine = StartCoroutine(Die());
                break;
        }
    }

    private IEnumerator Idle()
    {
        yield return new WaitForSeconds(waitTime); // 대기 시간

        ChangeState(NutState.Rotate); // 대기 후 Rotate 상태로 전환
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            // 애니메이션에서 트리거된 타이밍에 감지 수행
            yield return null;
        }
    }

    private IEnumerator Fire()
    {
        yield return new WaitForSeconds(1f);
        // Fire 애니메이션과 발사 로직을 처리
        if (!DetectPlayer()) // 플레이어가 감지되지 않으면
        {
            ChangeState(NutState.Idle); // Idle 상태로 전환
           
        }
        else
        {
            ChangeState(NutState.Fire);
        }
        
    }

    private IEnumerator Die()
    {
        // 죽는 애니메이션 처리
        yield return null;
    }

    public void TimeToDetection() // 애니메이션에서 호출하는 감지 함수
    {
        if (DetectPlayer())
        {
            ChangeState(NutState.Fire); // 플레이어를 감지하면 Fire 상태로 전환
        }
    }

    public void TimeToIdle()
    {
        if (DetectPlayer())
        {
            ChangeState(NutState.Fire); // 플레이어를 감지하면 Fire 상태로 전환
        }
        else
        {
            ChangeState(NutState.Idle);
        }
    }

    bool DetectPlayer()
    {
        LayerMask obstacleLayerMask = LayerMask.GetMask("Obstacle");
        LayerMask playerLayerMask = LayerMask.GetMask("Player");

        float sphereRadius = 0.5f;
        float detectionRange = 10.0f;
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = transform.forward;

        RaycastHit hitinfo;

        if (Physics.SphereCast(origin, sphereRadius, direction, out hitinfo, detectionRange, playerLayerMask))
        {
            /*CharacterController playerController = player.GetComponent<CharacterController>();
            float pVelocity = playerController.velocity.sqrMagnitude;
            if (!(pVelocity > 0)) return false;*/

            if (!Physics.Raycast(origin, (hitinfo.point - origin).normalized, out RaycastHit obstacleHit, hitinfo.distance, obstacleLayerMask))
            {
                Debug.Log("Player 감지 및 장애물 없음");
                isDetectingPlayer = false;
                return true;
            }
            else
            {
                Debug.Log("Player가 장애물 뒤에 있습니다.");
            }
        }

        return false;
    }
}
