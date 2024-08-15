using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Sequence
{
    public string name;
    public GameObject prefab;
    public bool isDisplay = false;
}

public class SequenceConsole : MonoBehaviour
{
    public static SequenceConsole instance;

    [Header("Data")]
    public SequenceListSO sequenceList; // 시퀀스 리스트를 관리하는 ScriptableObject

    [Header("프리팹")]
    public GameObject inputPrefab; // 입력(InputField) 프리팹

    [Header("타겟")]
    public Transform monitor; // 보여질 대상
    public GameObject Spacer; // 공간 띄울 대상
    public InputField inputField; // 집중된 InputField

    public Coroutine sequenceCoroutine;
    public int nowIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnEnable()
    {
        //LoadAndDisplayStartScreen();
        //sequenceCoroutine = StartCoroutine(DisplayAll());
        LoadAndDisplayCurrentSequence();
    }

    private void OnDisable()
    {
        RemoveInputFieldFocus();
        
        StopAllCoroutines();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            MoveToPreviousSequence();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            MoveToNextSequence();
        }
    }

    private void RemoveInputFieldFocus()
    {
        if (inputField != null)
        {
            inputField.onEndEdit.RemoveListener(OnInputFieldEndEdit);
        }
    }

    private void LoadAndDisplayStartScreen()
    {
        if (sequenceList != null)
        {
            var startCommand = sequenceList.sequences.Find(cmd => cmd.name == "Start");
            if (startCommand != null)
            {
                PrintToConsole(startCommand);
            }
        }
    }
    private void LoadAndDisplayCurrentSequence()
    {
        if (sequenceList != null && sequenceList.sequences.Count > 0)
        {
            Sequence currentSequence = sequenceList.sequences[nowIndex];
            PrintToConsole(currentSequence);
        }
    }

    private void PrintToConsole(Sequence sequence)
    {
        foreach (Transform child in monitor)
        {
            if (child.gameObject != Spacer)
            {
                Destroy(child.gameObject);
            }
        }

        Instantiate(sequence.prefab, monitor);
        GameObject inputInstance = Instantiate(inputPrefab, monitor);
        inputField = inputInstance.GetComponent<InputField>();
        inputField.transform.SetAsLastSibling();
        Canvas.ForceUpdateCanvases();

        //inputField.Select();
        //inputField.ActivateInputField();
        //SetupInputFieldFocus();
    }

    private void SetupInputFieldFocus()
    {
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(OnInputFieldEndEdit);
            inputField.ActivateInputField();
        }
    }

    private void OnInputFieldEndEdit(string value)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Enter(value);
        }

        StartCoroutine(RefocusInputField());
    }

    private IEnumerator RefocusInputField()
    {
        yield return null;
        inputField.ActivateInputField();
    }

    public void Enter(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            ProcessCommand(message);
        }
    }

    private void ProcessCommand(string message)
    {
        Sequence sequence = sequenceList.sequences.Find(s => s.name.Contains(message));
        if (sequence != null)
        {
            PrintToConsole(sequence);
        }
        else
        {
            Debug.LogWarning($"Command '{message}' not found.");
        }
    }

    private IEnumerator DisplayAll()
    {
        while (true)
        {
            Sequence sequence = sequenceList.sequences[nowIndex];

            // 현재 시퀀스가 표시 가능한지 확인
            while (!sequence.isDisplay)
            {
                // 표시 가능하지 않다면 인덱스를 증가시키고 다음 시퀀스로 이동
                nowIndex = (nowIndex + 1) >= sequenceList.sequences.Count ? 0 : nowIndex + 1;
                sequence = sequenceList.sequences[nowIndex];
            }

            // 표시 가능한 시퀀스가 발견되면 출력
            PrintToConsole(sequence);

            // 인덱스를 다음으로 이동
            nowIndex = (nowIndex + 1) >= sequenceList.sequences.Count ? 0 : nowIndex + 1;

            // 5초 대기
            yield return new WaitForSeconds(5);
        }
    }

    private void MoveToPreviousSequence()
    {
        do
        {
            nowIndex = (nowIndex - 1 + sequenceList.sequences.Count) % sequenceList.sequences.Count;
        } while (!sequenceList.sequences[nowIndex].isDisplay);

        LoadAndDisplayCurrentSequence();
    }

    private void MoveToNextSequence()
    {
        do
        {
            nowIndex = (nowIndex + 1) % sequenceList.sequences.Count;
        } while (!sequenceList.sequences[nowIndex].isDisplay);

        LoadAndDisplayCurrentSequence();
    }



}
