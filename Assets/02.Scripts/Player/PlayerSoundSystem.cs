using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundSystem : MonoBehaviour
{
    [SerializeField] private AudioClip[] outerFootstepSounds;
    [SerializeField] private AudioClip JumpSound;
    [SerializeField] private AudioClip LandingSound;
    private AudioSource audioSource;
    [SerializeField] private float walkInterval = 0.5f; // 발걸음 소리 간격
    [SerializeField] private float runInterval = 0.25f; // 발걸음 소리 간격
    private Coroutine soundCoroutine;
    private InputManager inputManager;
    private PlayerController playerController;
    private StaminaSystem stamina;
    private float interval = 0f;
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        stamina = GetComponentInParent<StaminaSystem>();
        audioSource = GetComponent<AudioSource>();
        inputManager = InputManager.instance;
    }

    private bool isRunning = false;

    private void Update()
    {
        
        if (playerController.isLandingOnce && playerController.isLandingOnce && inputManager.GetPlayerMovement() != Vector2.zero && !inputManager.inputCrouch)
        {
            bool currentlyRunning = false;
            if (!stamina.isExhausted && !stamina.isImpossibleJump)
            {
                currentlyRunning = inputManager.PlayerRan();
            }
            else
            {
                currentlyRunning = false;
            }
            if (currentlyRunning != isRunning || soundCoroutine == null)
            {
                isRunning = currentlyRunning;
                RestartFootsteps();
            }
            else
            {
                StartFootsteps();
            }
        }
        else
        {
            StopFootsteps();
        }
        if (!stamina.isImpossibleJump && playerController.isLandingOnce && inputManager.PlayerJumpedThisFrame())
        {
            StartJump();
        }
    }

    #region FootStep
    private void RestartFootsteps()
    {
        StopFootsteps();
        soundCoroutine = StartCoroutine(PlayFootsteps());
    }

    private IEnumerator PlayFootsteps()
    {
        while (true)
        {
            PlayRandomSound("FootStep");
            yield return new WaitForSeconds(isRunning ? runInterval : walkInterval);
        }
    }
    private void StartFootsteps()
    {
        if (soundCoroutine == null)
        {
            soundCoroutine = StartCoroutine(PlayFootsteps());
        }
    }

    private void StopFootsteps()
    {
        if (soundCoroutine != null)
        {
            StopCoroutine(soundCoroutine);
            soundCoroutine = null;
        }
    }
    #endregion

    #region Jump
    public void StartJump()
    {
        //isLanding = false;
        PlayRandomSound("Jump");
    }

    public void StartLanding()
    {
        //isLanding = true;
        PlayRandomSound("Landing");
    }
    #endregion
    private void PlayRandomSound(string type)
    {
        switch (type)
        {
            case "FootStep":
                if (outerFootstepSounds.Length > 0)
                {
                    int randomIndex = Random.Range(0, outerFootstepSounds.Length);
                    audioSource.PlayOneShot(outerFootstepSounds[randomIndex]);
                }
                break;
            case "Jump":
                audioSource.PlayOneShot(JumpSound);
                break;
            case "Landing":
                audioSource.PlayOneShot(LandingSound);
                break;
        }
    }
}
