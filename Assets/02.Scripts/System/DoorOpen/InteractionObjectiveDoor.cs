using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObjectiveDoor : InteractionDoorDataBase
{
    public List<DoorRequirement> doorRequirements;

    
    public GameObject activeObject;
  
    public void OnDisable()
    {
        StopAllCoroutines();
    }
    

    //Interactive에서 접근
    public void OpenCommand(bool value)
    {
        if (doorAnimator == null) return;
        foreach (DoorRequirement d in doorRequirements)
        {
            if (d.isSuccess == false)
            {
                StartCoroutine(DeactiveInTime());
                return;
            }
        }
        isOpened = true;
        return ;

    }
    public IEnumerator DeactiveInTime()
    {
        activeObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        activeObject.SetActive(false);
    }
}
