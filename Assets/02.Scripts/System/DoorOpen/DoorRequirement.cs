using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DoorRequirement : MonoBehaviour
{
    public bool isSuccess = false;

    public GameObject prefabToCompare;
    

   
    private void OnTriggerEnter(Collider other)
    {
        if (IsSamePrefabType(other.gameObject, prefabToCompare))
        {
            Debug.Log(other.name + "는 지정된 프리팹과 동일한 타입입니다.");
            isSuccess = true;
        }
    }

    private bool IsSamePrefabType(GameObject detectedObject, GameObject prefab)
    {
        // 두 오브젝트의 프리팹 인스턴스 ID를 가져옵니다.
        Object detectedPrefab = PrefabUtility.GetCorrespondingObjectFromSource(detectedObject);
        Object comparePrefab = PrefabUtility.GetCorrespondingObjectFromSource(prefab);

        // 프리팹 인스턴스 ID가 같은지 비교합니다.
        return detectedPrefab == comparePrefab && detectedPrefab != null;
    }
}
