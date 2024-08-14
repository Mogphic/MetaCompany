using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtUpDown : MonoBehaviour
{
    public float minStandingY = 0.75f;
    public float maxStandingY = 2.3f;
    public float minCrouchingY = -2.0f;
    public float maxCrouchingY = 1.2f;
    public float sensitivity = 0.01f;
    public float crouchTransitionSpeed = 5f;

    [SerializeField] private float targetY;
    [SerializeField] private Transform CameraVec;

    private bool isCrouching = false;
    private bool isTransitioning = false;
    private float transitionStartY;
    private float transitionTargetY;
    private float transitionStartTime;

    private void Update()
    {
        UpdateCrouchState();
        UpdateYPosition();
        UpdateCameraPosition();
    }

    private void UpdateCrouchState()
    {
        if (isCrouching != InputManager.instance.inputCrouch)
        {
            isCrouching = InputManager.instance.inputCrouch;
            StartCrouchTransition();
        }
    }

    private void StartCrouchTransition()
    {
        isTransitioning = true;
        transitionStartY = transform.localPosition.y;
        transitionStartTime = Time.time;

        // Calculate the relative position within the current range
        float currentRange = isCrouching ? (maxStandingY - minStandingY) : (maxCrouchingY - minCrouchingY);
        float currentMin = isCrouching ? minStandingY : minCrouchingY;
        float relativePosition = (transitionStartY - currentMin) / currentRange;

        // Calculate the target Y position in the new range
        float newRange = isCrouching ? (maxCrouchingY - minCrouchingY) : (maxStandingY - minStandingY);
        float newMin = isCrouching ? minCrouchingY : minStandingY;
        transitionTargetY = newMin + (relativePosition * newRange);

        // Instantly update the camera's rotation
        UpdateCameraRotation();
    }

    private void UpdateYPosition()
    {
        if (isTransitioning)
        {
            float t = (Time.time - transitionStartTime) * crouchTransitionSpeed;
            if (t >= 1f)
            {
                t = 1f;
                isTransitioning = false;
            }
            float newY = Mathf.Lerp(transitionStartY, transitionTargetY, t);
            SetYPosition(newY);
        }
        else
        {
            float mouseY = Input.GetAxis("Mouse Y");
            targetY = transform.localPosition.y + mouseY * sensitivity; // Not inverted for player position
            targetY = Mathf.Clamp(targetY, isCrouching ? minCrouchingY : minStandingY, isCrouching ? maxCrouchingY : maxStandingY);
            SetYPosition(targetY);
        }
    }

    private void SetYPosition(float yPos)
    {
        Vector3 currentPosition = transform.localPosition;
        transform.localPosition = new Vector3(currentPosition.x, yPos, currentPosition.z);
    }

    private void UpdateCameraPosition()
    {
        if (CameraVec != null)
        {
            // Update the camera's Y position
            Vector3 cameraLocalPos = CameraVec.localPosition;
            float targetCameraY = isCrouching ? 1.142f : 1.618f;

            if (isTransitioning)
            {
                float t = (Time.time - transitionStartTime) * crouchTransitionSpeed;
                t = Mathf.Clamp01(t);
                cameraLocalPos.y = Mathf.Lerp(cameraLocalPos.y, targetCameraY, t);
            }
            else
            {
                cameraLocalPos.y = targetCameraY;
            }

            CameraVec.localPosition = cameraLocalPos;

            // Update camera rotation every frame
            UpdateCameraRotation();
        }
    }

    private void UpdateCameraRotation()
    {
        if (CameraVec != null)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            // Rotate the camera based on mouse input
            // Invert the mouseY for vertical rotation
            CameraVec.Rotate(Vector3.left * -mouseY);
            transform.Rotate(Vector3.up * mouseX);

            // Clamp the vertical rotation
            Vector3 currentRotation = CameraVec.localEulerAngles;
            if (currentRotation.x > 180f) currentRotation.x -= 360f;
            currentRotation.x = Mathf.Clamp(currentRotation.x, -90f, 90f);
            CameraVec.localEulerAngles = currentRotation;
        }
    }
}