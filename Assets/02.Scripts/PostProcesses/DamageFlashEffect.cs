using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;
using System.Collections;

public class DamageFlashEffect : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    public float flashDuration = 0.5f;
    public float flashIntensity = 0.8f;

    private ColorGrading colorGrading;

    private void Start()
    {
        // 메인 카메라에서 PostProcessVolume 찾기
        if (postProcessVolume == null)
        {
            Camera mainCamera = Camera.main;
            postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();

            // PostProcessVolume이 없다면 추가
            if (postProcessVolume == null)
            {
                postProcessVolume = mainCamera.gameObject.AddComponent<PostProcessVolume>();
                postProcessVolume.isGlobal = true;
            }
        }

        // Post-processing 프로필이 없다면 새로 생성
        if (postProcessVolume.profile == null)
            postProcessVolume.profile = ScriptableObject.CreateInstance<PostProcessProfile>();

        // Color Grading 효과를 추가하거나 가져오기
        if (!postProcessVolume.profile.TryGetSettings(out colorGrading))
            colorGrading = postProcessVolume.profile.AddSettings<ColorGrading>();

        // 초기 설정
        colorGrading.enabled.Override(true);
        colorGrading.colorFilter.Override(Color.white);
    }

    public void TriggerDamageFlash()
    {
        StartCoroutine(DamageFlashCoroutine());
    }

    private IEnumerator DamageFlashCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < flashDuration)
        {
            float t = elapsedTime / flashDuration;
            float intensity = Mathf.PingPong(t * 2, 1) * flashIntensity;

            Color flashColor = Color.Lerp(Color.white, Color.red, intensity);
            colorGrading.colorFilter.Override(flashColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 효과 종료 후 원래 색상으로 복귀
        colorGrading.colorFilter.Override(Color.white);
    }
}