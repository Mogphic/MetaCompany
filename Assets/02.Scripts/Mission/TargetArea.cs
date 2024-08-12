using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArea : MonoBehaviour
{
    public string probName = string.Empty;
    private MissionManager missionManager;

    void Start()
    {
        missionManager = FindObjectOfType<MissionManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains(probName))
        {
            missionManager.missionOneList.Add(other.gameObject);
            missionManager.CheckMissionOneList();
        }
    }
}
