using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
#if UNITY_EDITOR
        // 두 오브젝트의 프리팹 인스턴스 ID를 가져옵니다.
        Object detectedPrefab = PrefabUtility.GetCorrespondingObjectFromSource(detectedObject);
        Object comparePrefab = PrefabUtility.GetCorrespondingObjectFromSource(prefab);
        // 프리팹 인스턴스 ID가 같은지 비교합니다.
        return detectedPrefab == comparePrefab && detectedPrefab != null;
#else
        // 빌드 환경에서는 다른 방식으로 비교하거나 항상 false를 반환합니다.
        // 여기에 빌드 환경에서 사용할 대체 로직을 구현하세요.
        Debug.LogWarning("프리팹 비교는 에디터에서만 지원됩니다.");
        return false;
#endif
    }
}