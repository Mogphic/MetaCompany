using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProFlashLight : MonoBehaviour
{
    [SerializeField]
    private int info;
    public GameObject light;

    public int GetInfo()
    {
        return info;
    }

    private void Update()
    {
        OnLight();
    }

    public void OnLight()
    {
        if (InputManager.instance.ToggleTurnOnOff())
        {
            light.SetActive(true);
        }
        else
        {
            light.SetActive(false);
        }
    }
}