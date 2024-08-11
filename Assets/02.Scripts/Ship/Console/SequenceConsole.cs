using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Sequence
{
    public string name;
    public GameObject prefab;
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
        LoadAndDisplayStartScreen();
    }

    private void OnDisable()
    {
        RemoveInputFieldFocus();
        StopAllCoroutines();
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

        inputField.Select();
        inputField.ActivateInputField();
        SetupInputFieldFocus();
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
}
