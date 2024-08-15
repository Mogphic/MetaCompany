using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class VignetteController : MonoBehaviour
{
    public Volume postProcessVolume;
    public float effectDuration = 0.5f;
    public float maxVignetteIntensity = 0.5f;
    public Color damageColor = Color.red;

    private Vignette vignette;
    private Coroutine currentEffectCoroutine;

    private void Start()
    {
        if (postProcessVolume == null)
        {
            postProcessVolume = FindObjectOfType<Volume>();
        }

        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out vignette))
        {
            Debug.Log("Vignette effect found in Volume profile");
        }
        else
        {
            Debug.LogError("Vignette effect not found in Volume profile. Please add it to your Volume.");
        }
    }

    public void TriggerSingleBlink()
    {
        if (currentEffectCoroutine != null)
        {
            StopCoroutine(currentEffectCoroutine);
        }
        currentEffectCoroutine = StartCoroutine(SingleBlinkVignetteEffect());
    }

    public void ApplySustainedRedEffect()
    {
        if (currentEffectCoroutine != null)
        {
            StopCoroutine(currentEffectCoroutine);
        }
        currentEffectCoroutine = StartCoroutine(SustainedRedVignetteEffect());
    }

    public void StopEffect()
    {
        if (currentEffectCoroutine != null)
        {
            StopCoroutine(currentEffectCoroutine);
        }
        ResetVignette();
    }

    private IEnumerator SingleBlinkVignetteEffect()
    {
        if (vignette == null) yield break;

        yield return ApplyVignetteEffect(0, maxVignetteIntensity, Color.black, damageColor);
        yield return ApplyVignetteEffect(maxVignetteIntensity, 0, damageColor, Color.black);
    }

    private IEnumerator SustainedRedVignetteEffect()
    {
        if (vignette == null) yield break;

        yield return ApplyVignetteEffect(0, maxVignetteIntensity, Color.black, damageColor);
    }

    private IEnumerator ApplyVignetteEffect(float startIntensity, float endIntensity, Color startColor, Color endColor)
    {
        float elapsedTime = 0f;

        while (elapsedTime < effectDuration)
        {
            float t = elapsedTime / effectDuration;
            float intensity = Mathf.Lerp(startIntensity, endIntensity, t);
            Color color = Color.Lerp(startColor, endColor, t);

            vignette.intensity.Override(intensity);
            vignette.color.Override(color);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        vignette.intensity.Override(endIntensity);
        vignette.color.Override(endColor);
    }

    private void ResetVignette()
    {
        vignette.intensity.Override(0);
        vignette.color.Override(Color.black);
    }
}