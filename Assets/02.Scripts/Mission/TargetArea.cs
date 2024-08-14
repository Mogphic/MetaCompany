using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArea : MonoBehaviour
{
    public int missionIndex = 0;
    public string probName = string.Empty;
    public bool isPutAble = false;
    private MissionManager missionManager;

    void Start()
    {
        missionManager = FindObjectOfType<MissionManager>();
    }

    [SerializeField]private float weight = 0;
    [SerializeField]private float maxWeight = 10f;

    public void PutInProbInArea(GameObject other)
    {
        switch (missionIndex)
        {
            case 0:
                weight += other.GetComponent<ItemComponent>().kg;
                if (weight >= maxWeight)
                {
                    missionManager.CheckMissionComplate(missionIndex);
                }
                break;
            case 1:
                if (other.gameObject.name.Contains(probName))
                {
                    isPutAble = true;
                    missionManager.missionTwoList.Add(other.gameObject);
                    other.gameObject.GetComponent<BoxCollider>().enabled = false;
                    other.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    other.gameObject.transform.position = transform.position;
                    missionManager.CheckMissionComplate(missionIndex);
                }
                else
                {
                    isPutAble = false;
                }
                break;
        }
    }
    /*void OnTriggerEnter(Collider other)
    {

    }*/
}
