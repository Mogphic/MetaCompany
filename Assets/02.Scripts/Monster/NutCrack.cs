﻿using System.Collections;
using UnityEngine;

public class NutCrack : MonoBehaviour
{
    public enum EEnemyState
    {
        Rotate,
        ShootAttack,
        Die_nut,
    }

    // 현재 상태
    EEnemyState currentState;

    // Animator 컴포넌트를 저장할 전역 변수
    Animator animator;

    // player 찾기
    GameObject player;

    // 적의 최대 체력과 현재 체력
    public float maxHp = 100f;
    private float currentHp;

    // 회전 관련 변수
    public float rotationDuration = 5.0f; // 한 바퀴 도는데 걸리는 시간
    private float rotationSpeed;
    private Quaternion targetRotation;

    public GameObject shootParticle; // 발사 파티클 프리팹
    public float shootDamage = 10.0f; // 발사 데미지
    public float shootInterval = 2.5f; // 발사 간격

    void Start()
    {
        // 현재 게임 오브젝트에서 Animator 컴포넌트를 찾는다.
        animator = GetComponentInChildren<Animator>();

        // Player라는 오브젝트 찾기
        player = GameObject.Find("Player");

        // 체력을 최대값으로 초기화
        currentHp = maxHp;

        // 회전 속도 계산
        rotationSpeed = 360f / rotationDuration; // 360도 / 회전 시간

        // 초기 상태를 Rotate로 설정
        ChangState(EEnemyState.Rotate);
    }

    void Update()
    {
        // 상태에 따라 처리할 로직이 있음
    }

    void ChangState(EEnemyState state)
    {
        // 현재 상태를 변경하고, 애니메이터 상태를 조정
        currentState = state;

        switch (currentState)
        {
            case EEnemyState.Rotate:
                animator.SetBool("Attack", false);
                StopAllCoroutines(); // 이전 코루틴이 실행 중이면 중단
                StartCoroutine(RotateAndWait());
                break;

            case EEnemyState.ShootAttack:
                animator.SetBool("Attack", true);
                StopAllCoroutines(); // 이전 코루틴이 실행 중이면 중단
                StartCoroutine(ShootAtPlayer());
                break;

            case EEnemyState.Die_nut:
                animator.SetTrigger("Die");
                StopAllCoroutines(); // 모든 코루틴 중단
                break;
        }
    }

    IEnumerator RotateAndWait()
    {
        float rotationElapsed = 0f;
        float rotationInterval = rotationDuration / 4f; // 회전을 4등분하여 각도 설정

        while (rotationElapsed < rotationDuration)
        {
            // 다음 회전 설정
            SetNextRotation();

            // 회전 목표까지 회전하는 동안
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                if (DetectPlayer())
                {
                    // 플레이어를 감지하면 공격 상태로 전환하고 코루틴을 종료
                    ChangState(EEnemyState.ShootAttack);
                    yield break; // 코루틴 종료
                }

                // 회전
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                yield return null; // 다음 프레임까지 대기
            }

            rotationElapsed += rotationInterval;
            yield return null; // 다음 프레임까지 대기
        }

        // 회전 완료 후 5초 대기
        yield return new WaitForSeconds(5.0f);

        // 상태를 다시 Rotate로 설정하여 무한 반복
        ChangState(EEnemyState.Rotate);
    }

    bool DetectPlayer()
    {
        LayerMask obstacleLayerMask = LayerMask.GetMask("Obstacle");
        LayerMask playerLayerMask = LayerMask.GetMask("Player");
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hitinfo;

        if (Physics.Raycast(ray, out hitinfo, 5.0f, playerLayerMask))
        {
            if (!Physics.Raycast(transform.position + Vector3.up, (hitinfo.point - (transform.position + Vector3.up)).normalized, out RaycastHit obstacleHit, hitinfo.distance, obstacleLayerMask))
            {
                Debug.Log("Player 감지 및 장애물 없음");
                return true;
            }
            else
            {
                Debug.Log("Player가 장애물 뒤에 있습니다.");
            }
        }

        return false;
    }

    void SetNextRotation()
    {
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
    }

    IEnumerator ShootAtPlayer()
    {
        float initialTime = Time.time;

        while (Time.time - initialTime < 5.0f) // 5초 동안 총을 쏘는 루프
        {
            // 발사
            Shoot();
            yield return new WaitForSeconds(shootInterval); // 간격 후 발사
            initialTime = Time.time;
        }

        // 공격이 완료된 후 다시 회전 상태로 전환
        ChangState(EEnemyState.Rotate);
    }

    void Shoot()
    {
        // 플레이어 체력 감소
        player.GetComponent<HpSystem>().UpdateHp(shootDamage);

        // 발사 파티클 생성 (옵션)
        if (shootParticle != null)
        {
            GameObject particle = Instantiate(shootParticle, transform.position + transform.forward, transform.rotation);
            Destroy(particle, 0.1f); // 0.1초 후 파티클 제거
        }
    }

    void DieNut()
    {
        // 상태를 Die_nut로 변경
        ChangState(EEnemyState.Die_nut);

        // 사망 애니메이션이 완료된 후 오브젝트를 제거
        StartCoroutine(DestroyAfterDeath());
    }

    IEnumerator DestroyAfterDeath()
    {
        // 사망 애니메이션이 2초 동안 재생된 후에 오브젝트 제거
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    // 적이 피해를 받을 때 호출되는 메서드
    public void TakeDamage(float damage)
    {
        // 체력 감소
        currentHp -= damage;

        // 체력이 0 이하가 되면 DieNut 메서드 호출
        if (currentHp <= 0)
        {
            DieNut();
        }
    }
}
