using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    private Animator animator;

    public bool isOpenDoor = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void DoorAnim(bool isOpen)
    {
        animator.SetBool("isOpenDoor", isOpen);
        isOpenDoor = isOpen;
    }
}
