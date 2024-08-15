using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetArea : MonoBehaviour
{
    public int missionIndex = 0;
    public string probName = string.Empty;
    public bool isPutAble = false;
    private MissionManager missionManager;
    //0 : Red //1: Green
    public GameObject[] alerts;
    public GameObject textObj;
    private TextMeshProUGUI text;
    private PlayerSoundSystem sound;
    void Start()
    {
        missionManager = FindObjectOfType<MissionManager>();
        sound = FindObjectOfType<PlayerSoundSystem>();
        alerts[0].SetActive(true);
        alerts[1].SetActive(false);
        
        if (missionIndex == 0)
        {
            text = textObj.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"{weight} / {maxWeight}";
            }
        }
    }

    [SerializeField]private float weight = 0;
    [SerializeField]private float maxWeight = 10f;

    public void PutInProbInArea(GameObject other)
    {
        switch (missionIndex)
        {
            case 0:
                weight += other.GetComponent<ItemComponent>().kg;
                text.text = $"{weight} / {maxWeight}";
                if (weight >= maxWeight)
                {
                    bool isComp1 = missionManager.CheckMissionComplate(missionIndex);
                    if (isComp1 == true)
                    {
                        sound.MissionSound(0);
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
                        sound.MissionSound(0);
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
