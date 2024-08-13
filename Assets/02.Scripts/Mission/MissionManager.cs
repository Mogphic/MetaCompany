using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Mission
{
    public int id;
    public string description;
    public string target;
    public bool isCompleted;
    public TargetArea area;
}
public class MissionManager : MonoSingleton<MissionManager>
{
    public List<Mission> missions = new List<Mission>();
    public List<GameObject> missionOneList = new List<GameObject>();
    public List<GameObject> missionTwoList = new List<GameObject>();

    public bool CheckMissionComplate(int idx)
    {
        switch (idx)
        {
            case 0:
                CompleteMission(idx);
                return true;
            case 1:
                foreach (var prob in missionTwoList)
                {
                    if (prob == null)
                    {
                        return false;
                    }
                    CompleteMission(idx);
                    return true;
                }
                break;
        }

        return false;
    }

    public void CompleteMission(int idx)
    {
        missions[idx].isCompleted = true;
    }
}

