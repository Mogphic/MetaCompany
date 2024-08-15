using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollExample : MonoBehaviour
{
    public Animator animator;
    public Rigidbody[] rigidbodies;
    public Collider[] colliders;

    private bool isRagdoll = false;

    private void Start()
    {
        // 시작 시 래그돌 비활성화
        SetRagdollState(false);
    }

    public void ToggleRagdoll()
    {
        isRagdoll = !isRagdoll;
        print(isRagdoll);
        SetRagdollState(isRagdoll);
    }

    private void SetRagdollState(bool state)
    {
        // Animator 컴포넌트 활성화/비활성화
        animator.enabled = !state;

        // 모든 Rigidbody 컴포넌트의 kinematic 속성 설정
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = !state;
        }

        // 모든 Collider 컴포넌트 활성화/비활성화
        foreach (Collider col in colliders)
        {
            col.enabled = state;
        }
    }
}