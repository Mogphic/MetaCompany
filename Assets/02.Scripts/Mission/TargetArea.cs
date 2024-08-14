using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArea : MonoBehaviour
{
    public int missionIndex = 0;
    public string probName = string.Empty;
    public bool isPutAble = false;
    private MissionManager missionManager;
    //0 : Red //1: Green
    public GameObject[] alerts;

    void Start()
    {
        missionManager = FindObjectOfType<MissionManager>();
        alerts[0].SetActive(true);
        alerts[1].SetActive(false);
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
                    bool isComp1 = missionManager.CheckMissionComplate(missionIndex);
                    if (isComp1 == true)
                    {
                        alerts[0].SetActive(false);
                        alerts[1].SetActive(true);
                    }
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
                    
                    bool isComp2 = missionManager.CheckMissionComplate(missionIndex);
                    if (isComp2 == true)
                    {
                        alerts[0].SetActive(false);
                        alerts[1].SetActive(true);
                    }
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
