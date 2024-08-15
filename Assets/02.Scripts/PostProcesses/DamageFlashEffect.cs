using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class DamageFlashEffect : MonoBehaviour
{
    public Volume postProcessVolume;
    public float flashDuration = 0.5f;
    public float flashIntensity = 0.8f;

    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        if (postProcessVolume == null)
        {
            postProcessVolume = FindObjectOfType<Volume>();
        }

        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out colorAdjustments))
        {
            Debug.Log("ColorAdjustments found in Volume profile");
        }
        else
        {
            Debug.LogError("ColorAdjustments not found in Volume profile. Please add it to your Volume.");
        }
    }

    public void TriggerDamageFlash()
    {
        StartCoroutine(DamageFlashCoroutine());
    }

    private IEnumerator DamageFlashCoroutine()
    {
        if (colorAdjustments == null)
        {
            Debug.LogError("ColorAdjustments is null. Cannot apply effect.");
            yield break;
        }

        float elapsedTime = 0f;
        Color originalColorFilter = colorAdjustments.colorFilter.value;

        while (elapsedTime < flashDuration)
        {
            float t = elapsedTime / flashDuration;
            float intensity = Mathf.PingPong(t * 2, 1) * flashIntensity;

            Color flashColor = Color.Lerp(Color.white, Color.red, intensity);
            colorAdjustments.colorFilter.Override(flashColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        colorAdjustments.colorFilter.Override(originalColorFilter);
    }
}