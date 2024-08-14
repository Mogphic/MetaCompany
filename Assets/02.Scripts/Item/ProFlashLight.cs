using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProFlashLight : MonoBehaviour
{
    [SerializeField] private int info;
    [SerializeField] private AudioClip[] turnOnOffSounds;
    [SerializeField] private AudioClip grabSound;
    private AudioSource audioSource;
    public GameObject lightObj;
    private bool isTurnOnOnce = false;
    private bool isTurnOffOnce = false;
    private bool isOnce = false;
    public int GetInfo()
    {
        return info;
    }

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(grabSound);
    }

    private void Start()
    {
        //audioSource = GetComponent<AudioSource>();
        
    }

    private void Update()
    {
        OnLight();
    }

    public void OnLight()
    {
        //if (isOnce == true)
        {
            if (InputManager.instance.ToggleTurnOnOff())
            {
                if (isTurnOnOnce == false)
                {
                    isTurnOffOnce = false;
                    audioSource.PlayOneShot(turnOnOffSounds[0]);
                    isTurnOnOnce = true;
                    isOnce = true;
                }
                lightObj.SetActive(true);
            }
            else
            {
                if (isTurnOffOnce == false)
                {
                    isTurnOnOnce = false;
                    if (isOnce == true)
                    {
                        audioSource.PlayOneShot(turnOnOffSounds[1]);
                    }
                    isTurnOffOnce = true;
                }
                lightObj.SetActive(false);
            }
        }
    }
}