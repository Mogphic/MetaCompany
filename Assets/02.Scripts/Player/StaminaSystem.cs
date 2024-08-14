﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    [SerializeField] private float curStamina = 0f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float increaseRatePerSecond = 10f;
    [SerializeField] private float decreaseRatePerSecond = 10f;
    [SerializeField] private float decreaseRateForJump = 15f;
    private WaitForSeconds increaseSec;
    private WaitForSeconds decreaseSec;
    private Coroutine staminaCoroutine;
    public float weight = 0f;
    public bool isExhausted = false;
    public bool isImpossibleJump = false;
    public bool isImpossibleRun = false;

    private void Start()
    {
        curStamina = maxStamina;
        increaseSec = new WaitForSeconds(1f / increaseRatePerSecond);
        decreaseSec = new WaitForSeconds(1f / (decreaseRatePerSecond + (weight * 0.05f)));
        staminaCoroutine = StartCoroutine(IncreaseStamina());
    }

    public void UpdateStamina(float value)
    {
        curStamina += value;
        if (curStamina > maxStamina)
        {
            curStamina = maxStamina;
        }
        if (curStamina <= 0)
        {
            isExhausted = true;
            curStamina = 0;
            ChangeCoroutine("Increase");
        }

        if (curStamina > decreaseRateForJump)
        {
            isExhausted = false;
        }

        if (isExhausted)
        {
            if (curStamina < decreaseRateForJump)
            {
                isImpossibleJump = true;
                ChangeCoroutine("Increase");
            }
            else
            {
                isImpossibleJump = false;
                isExhausted = false;
            }
        }

        UIManager.instance.UpdateStaminaUI(curStamina / maxStamina);
    }

    IEnumerator IncreaseStamina()
    {
        while (true)
        {
            yield return increaseSec;
            UpdateStamina(1f);
        }
    }

    IEnumerator DecreaseStamina()
    {
        while (true)
        {
            yield return decreaseSec;
            UpdateStamina(-1f);
        }
    }

    public void DecreaseStaminaForJump()
    {
        StopCoroutine(staminaCoroutine);
        UpdateStamina(-decreaseRateForJump);
    }

    public void ChangeCoroutine(string toName)
    {
        StopCoroutine(staminaCoroutine);
        switch (toName)
        {
            case "Increase":
                staminaCoroutine = StartCoroutine(IncreaseStamina());
                break;
            case "Decrease":
                staminaCoroutine = StartCoroutine(DecreaseStamina());
                break;
        }
    }
}
