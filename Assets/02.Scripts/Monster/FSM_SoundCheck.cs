using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.AI;

public class FSM_SoundCheck : MonoBehaviour
{
    public enum EEnemyState
    {
        Idle,
        WalkClam,
        Rotate_,
        Chase_,
        Attack,
        Die_Dog,
        Bite
    }

    // 현재 상태
    public EEnemyState currentState;

    // Animator 컴포넌트를 저장할 전역 변수
    Animator animator;

    // NavMeshAgent 컴포넌트를 저장한 전역 변수
    NavMeshAgent agent;

    // NavMeshSurface 경계를 저장한 전역 변수
    Bounds navMeshBounds;

    // 플레이어 찾기
    GameObject player;

    // 플레이어 방향 구하기 전역 변수
    Vector3 dir;

    // 플레이어 방향으로 회전하기 위해 어느정도 돌아야 하는 지 구하는 전역 변수
    Quaternion lookRotation;

    // 플레이어의 최대 체력
    HpSystem play_health;

    // 콜라이더 컴포넌트
    SphereCollider scollider;

    // 콜라이더의 라디어스 값
    float radius;

    // walk clam의 setDestination 남은 거리
    public float remaindistnace2;

    // player 방향 콕 알기
    Vector3 player_p;

    // 추격 중인지 아닌지
    bool isChasing;

    // 공격 중인지 아닌지
    bool isAttacking;

    float walkSpeed = 1.5f;
    float chaseSpeed = 10f;
    float attackSpeed = 5f;

    // 딱 감지되었을 때 player 처음 위치
    private Vector3 initialTargetPosition;



    Transform playerTransform;

    AudioSource playerAudioSource;

    float soundThreshold = 0.3f;

    bool isPlayerDetected;

    // 적의 체력
    public float health = 100f;

    GameObject item;
    ItemSpawner itemSpawn;

    void Start()
    {
        // 현재 게임 오브젝트에서 Animator 컴포넌트를 찾는다.
        animator = GetComponentInChildren<Animator>();

        // 현재 게임 오브젝트에서 NavMeshAgent 컴포넌트를 찾는다.
        agent = GetComponent<NavMeshAgent>();

        // NavMeshSurface 바운딩 박스를 설정한다.
        NavMeshSurface navMeshSurface = GameObject.Find("3floor").GetComponent<NavMeshSurface>();

        // 플레이어 찾기
        player = GameObject.Find("Player");

        // 플레이어의 체력 관련 메서드 가져오기
        play_health = player.GetComponent<HpSystem>(); 


        playerTransform = player.GetComponent<Transform>();

        // 원형 콜라이더 갖고 오기
        scollider = GetComponent<SphereCollider>();

        // 원형 콜라이더의 반지름
        radius = scollider.radius;

        // NavMeshSurface 경계를 가져온다.
        if (navMeshSurface != null)
        {
            navMeshBounds = navMeshSurface.navMeshData.sourceBounds;
        }

        // 애니메이터 작동하기
        // animator.SetBool("WalkClam", true);
        ChangState(EEnemyState.Idle);

        NavMeshHit hit;

        // 일단 start 부분에서 눈 없는 개 무작정 돌아다니게 하기
        if (NavMesh.SamplePosition(RandomPositionSetting(), out hit, navMeshBounds.size.magnitude, 1))
        {
            agent.SetDestination(hit.position);
            agent.speed = walkSpeed;
        }

        playerAudioSource = player.GetComponentInChildren<AudioSource>();

        item = GameObject.Find("ItemSpawner");
        itemSpawn = item.GetComponent<ItemSpawner>();
    }

    void Update()
    {
        // Sound_Check 로직
        CheckPlayerSound();

        switch (currentState)
        {
            case EEnemyState.WalkClam:
                // Debug.Log("State: WalkClam");
                UpdateWalkClam();
                break;

            case EEnemyState.Rotate_:
                // Debug.Log("State: Rotate_");
                UpdateRotate();
                break;

            case EEnemyState.Chase_:
                // Debug.Log("State: Chase_");
                UpdateChase_Check();
                break;

            case EEnemyState.Attack:
                // Debug.Log("State: Attack");
                Attack();
                break;
        }

        // 사운드 체크 로직 (그러면 조금이라도 돌아볼텐데)
        if (isPlayerDetected && currentState != EEnemyState.Attack && currentState != EEnemyState.Die_Dog)
        {
            if (!isChasing && !isAttacking)
            {
                // Debug.Log("Player detected, transitioning to Rotate_ state.");
                if (currentState == EEnemyState.WalkClam)
                {
                    ChangState(EEnemyState.Rotate_);
                }
                else
                {
                    UpdateRotate();
                }
            }
        }

    }

    void ChangState(EEnemyState state)
    {
        currentState = state;

        switch (currentState)
        {
            case EEnemyState.Idle:
                animator.SetBool("WalkClam", false);
                animator.SetBool("Attack_", false);
                animator.SetBool("Rotate_", false);
                animator.SetBool("Chase_", false);
                agent.isStopped = true;
                isChasing = false;
                isAttacking = false;
                break;

            case EEnemyState.WalkClam:
                animator.SetBool("WalkClam", true);
                animator.SetBool("Attack_", false);
                animator.SetBool("Rotate_", false);
                animator.SetBool("Chase_", false);
                agent.isStopped = false;
                isChasing = false;
                isAttacking = false;
                break;

            case EEnemyState.Rotate_:
                animator.SetBool("Rotate_", true);
                animator.SetBool("WalkClam", false);
                animator.SetBool("Attack_", false);
                animator.SetBool("Chase_", false);
                agent.isStopped = true;
                isChasing = false;
                isAttacking = false;
                break;

            case EEnemyState.Chase_:
                animator.SetBool("Chase_", true);
                animator.SetBool("Rotate_", false);
                animator.SetBool("WalkClam", false);
                animator.SetBool("Attack_", false);
                agent.isStopped = false;
                agent.speed = chaseSpeed;
                isChasing = true;
                isAttacking = false;
                break;

            case EEnemyState.Attack:
                animator.SetBool("Attack_", true);
                animator.SetBool("Chase_", false);
                animator.SetBool("WalkClam", false);
                animator.SetBool("Rotate_", false);
                agent.isStopped = false;
                isChasing = false;
                agent.speed = attackSpeed;
                isAttacking = true;
                break;

            case EEnemyState.Die_Dog:
                animator.SetTrigger("Die 0");
                // agent.enabled = false;
                int randomIndex = Random.Range(0, itemSpawn.itemList.Length); // 랜덤으로 아이템 생성 만약에 이 코드가 없고
                // ItemSpawner에서 false로 설정되어 있다면 항상 itemList 5번 인덱스 아이템이 생성
                Vector3 pos = transform.position;
                pos.y = 1;
                GameObject item = itemSpawn.SpawnItem(randomIndex, pos);

                // itemSpawn.SpawnItem(5, transform.position);
                StartCoroutine(DestroyAfterDelay());
                break;

            case EEnemyState.Bite:
                animator.SetTrigger("Bite");
                // animator.SetBool("Attack_", false);
                // ChangState(EEnemyState.WalkClam);
                break;
        }
    }

    void UpdateWalkClam()
    {
        remaindistnace2 = 0.5f;
        // Debug.Log($"Walking, remaining distance: {agent.remainingDistance}");

        if (agent.remainingDistance < remaindistnace2)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(RandomPositionSetting(), out hit, navMeshBounds.size.magnitude, 1))
            {
                agent.speed = walkSpeed;
                agent.SetDestination(hit.position);
            }
        }

        else
        {
            // 현재 목적지의 방향을 계산
            Vector3 directionToDestination = (agent.destination - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToDestination);

            // 회전이 완료되었는지 확인
            if (Quaternion.Angle(transform.rotation, targetRotation) > 20.0f)
            {
                // 아직 회전 중이라면 회전 먼저 수행
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
                agent.isStopped = true; // 회전 중에는 이동하지 않도록 정지
            }
            else
            {
                // 회전이 완료되었으면 이동 시작
                agent.isStopped = false;
            }
        }
    }

    Vector3 RandomPositionSetting()
    {
        Vector3 randomPosition = new Vector3
        (
            Random.Range(navMeshBounds.min.x, navMeshBounds.max.x),
            Random.Range(navMeshBounds.min.y, navMeshBounds.max.y),
            Random.Range(navMeshBounds.min.z, navMeshBounds.max.z)

        );

        return randomPosition;
    }

    // 회전에 걸리는 시간 (초)
    private float rotationDuration = 1.0f;
    // 회전 시작 시간을 저장하는 변수
    private float rotationStartTime;

    // 플레이어를 향해 AI가 회전하도록 하는 로직을 처리하는 메서드
    void UpdateRotate()
    {
        player_p = player.transform.position;
        initialTargetPosition = player_p;

        dir = player_p - transform.position;
        lookRotation = Quaternion.LookRotation(dir);

        if (rotationStartTime == 0)
        {
            rotationStartTime = Time.time;
            //ChangState(EEnemyState.Rotate_);
        }

        float elapsedTime = Time.time - rotationStartTime;
        float t = Mathf.Clamp01(elapsedTime / rotationDuration); // 회전 진행도를 0에서 1 사이의 값으로 정규화합니다.

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, t);

        float angle = Quaternion.Angle(transform.rotation, lookRotation);

        if (angle < 00.001f && t >= 1)
        {
            rotationStartTime = 0;
            if (radius <= dir.magnitude) 
            {
                ChangState(EEnemyState.Chase_);
            }
            else
            {
                ChangState(EEnemyState.Attack);
            }
        }

        // 여기서 else는  => AI는 계속해서 회전을 시도할 것이며, 아직 상태전환이 일어나지 않는다.
        // 여기서 else가 없디 때문에 계속 회전을 완료할려고 한다.
    }



    private void UpdateChase_Check()
    {
        // 플레이어의 현재 위치로 AI를 이동시킴
        // agent.SetDestination(player_p);

        agent.SetDestination(initialTargetPosition);

        // AI가 플레이어에게 충분히 가까워졌는지 확인
        if (radius >= agent.remainingDistance) 
        {
            ChangState(EEnemyState.Attack);
        }


        /*


        // AI가 목적지(플레이어의 마지막 알려진 위치)에 도달했는지 확인
        else if (agent.remainingDistance < 0.01f)
        {
            // 플레이어가 현재 인식 범위에 있는지 확인
            if (!isPlayerDetected)
            {
                // 목적지에 도달했지만 플레이어가 없다면, 추적을 중단하고 평상시 상태로 돌아감
                isChasing = false;
                animator.SetBool("WalkClam", true);
                ChangState(EEnemyState.WalkClam);
            }
            else
            {
                // 플레이어가 아직 인식 범위 내에 있다면 다시 추적
                UpdateChase_Check();
            }
        }
        // 위의 조건들에 해당하지 않으면 계속 추적 상태를 유지


        */

    }


    // 공격 동작의 지속 시간을 설정하는 변수 (초 단위)
    private float attackDuration = 2.0f;
    private float attackStartTime;

    // 공격 후 대기하는 시간을 설정하는 변수 (초 단위)
    private float postAttackWaitDuration = 1.0f;
    private float postAttackWaitStartTime;

    void Attack()
    {
        Vector3 extendedDestination = transform.position + transform.forward;

        // NavMesh 위의 유효한 위치로 조정합니다.
        NavMeshHit hit;
        if (NavMesh.SamplePosition(extendedDestination, out hit, 3.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        if (attackStartTime == 0)
        {
            attackStartTime = Time.time;

            // 현재 위치에서 목표 위치까지의 방향을 계산합니다.
            Vector3 directionToTarget = (initialTargetPosition - transform.position).normalized;

            // 목표 위치에서 약간 더 나아간 위치를 계산합니다.
            // 여기서 2.0f는 추가 거리입니다. 필요에 따라 조절하세요.
            //Vector3 extendedDestination = initialTargetPosition + directionToTarget * 2.0f;

            //Vector3 extendedDestination = transform.position + transform.forward * 5;

            // NavMesh 위의 유효한 위치로 조정합니다.
            //NavMeshHit hit;
            if (NavMesh.SamplePosition(extendedDestination, out hit, 3.0f, NavMesh.AllAreas))
            {
               // agent.SetDestination(hit.position);
            }
            else
            {
                // 유효한 NavMesh 위치를 찾지 못했을 경우, 원래 목표 위치를 사용합니다.
                agent.SetDestination(initialTargetPosition);
            }
        }

        if (Time.time - attackStartTime < attackDuration)
        {
            // 공격 애니메이션 유지
            isPlayerDetected = false;
        }
        else
        {
            // 공격 지속 시간이 지나면 상태 전환
            attackStartTime = 0;
            isAttacking = false;
            animator.SetBool("Attack_", false);
            CheckPlayerSound();
            if (isPlayerDetected)
            {
                ChangState(EEnemyState.Rotate_);
            }
            else
            {
                ChangState(EEnemyState.WalkClam);
            }
        }
    }



    // Sound_Check의 로직을 메서드로 분리
    void CheckPlayerSound()
    {
        if (playerAudioSource != null && playerAudioSource.isPlaying)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            float volume_distance = playerAudioSource.maxDistance;

            if (distance <= volume_distance && playerAudioSource.volume > soundThreshold)
            {
                isPlayerDetected = true;
            }
            else
            {
                isPlayerDetected = false;
            }
        }
        else
        {
            isPlayerDetected = false;
        }
    }

    // IsPlayerDetected 메서드는 그대로 유지
    public bool IsPlayerDetected()
    {
        return isPlayerDetected;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die_dog();

        }
    }

    void Die_dog()
    {
        if (currentState != EEnemyState.Die_Dog)
        {
            ChangState(EEnemyState.Die_Dog);
        }
    }

    // 사망 후 오브젝트 제거를 위한 코루틴
    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(4.0f); // 3.5초 후 제거 (애니메이션 재생 시간에 맞춰 조정)
        Destroy(gameObject);
    }

    private float invincibilityDuration = 3f; // 무적 시간 (초)
    private float lastHitTime = -100f; // 마지막으로 맞은 시간


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && currentState == EEnemyState.Attack)
        {
            HpSystem playerHealth = other.GetComponent<HpSystem>();
            CharacterController playerController = other.GetComponent<CharacterController>();
            if (playerHealth != null && playerHealth.curHp > 0)
            {
                // 무적 시간 체크
                if (Time.time - lastHitTime > invincibilityDuration)
                {
                    playerHealth.UpdateHp(50.0f);
                    lastHitTime = Time.time; // 마지막 피격 시간 갱신

                    if (playerController != null)
                    {
                        Vector3 pushDirection = (other.transform.position - transform.position).normalized;
                        pushDirection.y = 0; // Y축 이동 방지
                        StartCoroutine(PushPlayerSmoothly(playerController, pushDirection, 15.0f, 0.5f));
                    }

                    if (playerHealth.curHp <= 0)
                    {
                        ChangState(EEnemyState.Bite);
                        playerHealth.Die();
                    }
                }
                /*
                else
                {
                    // 무적 시간 중이라면 밀어내기만 수행
                    if (playerController != null)
                    {
                        Vector3 pushDirection = (other.transform.position - transform.position).normalized;
                        pushDirection.y = 0; // Y축 이동 방지
                        StartCoroutine(PushPlayerSmoothly(playerController, pushDirection, 10f, 2.0f));
                    }
                }
                */
            }
        }
    }

    private IEnumerator PushPlayerSmoothly(CharacterController controller, Vector3 pushDirection, float distance, float duration)
    {
        float elapsedTime = 0;
        Vector3 startPosition = controller.transform.position;
        Vector3 targetPosition = startPosition + pushDirection * distance;

        while (elapsedTime < duration)
        {
            controller.Move(pushDirection * (distance / duration) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 정확한 위치로 조정
        controller.Move(targetPosition - controller.transform.position);
    }

    public void ActivateAI()
    {
        if (currentState == EEnemyState.Idle)
        {
            ChangState(EEnemyState.WalkClam);
        }
    }
}