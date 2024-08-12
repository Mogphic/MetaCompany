using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Mission
{
    public int id;
    public string description;
    public bool isCompleted;
}
public class MissionManager : MonoSingleton<MissionManager>
{
    public List<Mission> missions = new List<Mission>();
    public List<GameObject> missionOneList = new List<GameObject>();

    void Start()
    {
        InitializeMissions();
    }

    void InitializeMissions()
    {
        missions.Add(new Mission { id = 1, description = "물체를 목표 지점에 놓으세요", isCompleted = false });
    }

    public bool CheckMissionOneList()
    {
        foreach (var prob in missionOneList)
        {
            if (prob == null)
            {
                return false;
            }
        }
        CompleteMission(0);
        return true;
    }

    public void CompleteMission(int idx)
    {
        missions[idx].isCompleted = true;
    }
}

