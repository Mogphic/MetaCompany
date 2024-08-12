using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionKillDoor : InteractionDoorDataBase
{
    public GameObject target;

    protected override void Awake()
    {
        base.Awake();
        if (target == null)
            Debug.LogError("Target없음");
    }
    public void Update()
    {
        if(target.IsDestroyed())
        {
            isOpened = true;
        }
    }
}
