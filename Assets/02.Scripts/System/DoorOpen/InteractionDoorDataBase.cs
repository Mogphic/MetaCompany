using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDoorDataBase : MonoBehaviour
{
    public Animator doorAnimator;


    public bool _isOpened = false;

    public bool isOpened
    {
        get
        {
            return _isOpened;
        }
        set
        {
            Open(value);
            _isOpened = value;  

        }
        
    }

    protected virtual void Awake()
    {
        doorAnimator = GetComponent<Animator>();
        
    }
    private void Open(bool value)
    {
        if(value)
        {
            doorAnimator.SetBool("isOpened", true);
        }
    }

    

}
//

