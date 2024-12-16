using UnityEngine;
using Cinemachine;

public class CustomCameraOrbit : MonoBehaviour
{
    [Header("Camera Reference")]
    public CinemachineFreeLook freeLookCamera;

    [Header("Rotation Keys (Old Input System)")]
    public KeyCode rotateLeftKey = KeyCode.A;
    public KeyCode rotateRightKey = KeyCode.D;
    public KeyCode rotateUpKey = KeyCode.W;
    public KeyCode rotateDownKey = KeyCode.S;

    [Header("Rotation Settings")]
    [Tooltip("Degrees per second of horizontal rotation around the target.")]
    public float rotationSpeed = 50f;

    [Tooltip("Vertical rotation speed factor (0..1 range).")]
    public float verticalRotationSpeed = 0.5f;

    void Update()
    {
        if (freeLookCamera == null) return;

        float horizontalInput = 0f;
        float verticalInput = 0f;

        // Left/Right
        if (Input.GetKey(rotateLeftKey)) horizontalInput -= 1f;
        if (Input.GetKey(rotateRightKey)) horizontalInput += 1f;

        // Up/Down
        if (Input.GetKey(rotateUpKey)) verticalInput += 1f;
        if (Input.GetKey(rotateDownKey)) verticalInput -= 1f;

        // -- HORIZONTAL ORBIT --
        // CinemachineFreeLook's m_XAxis.Value is in degrees (0–360).
        freeLookCamera.m_XAxis.Value += horizontalInput * rotationSpeed * Time.deltaTime;

        // -- VERTICAL ORBIT --
        // CinemachineFreeLook's m_YAxis.Value is typically in [0..1] (bottom to top).
        float newYAxisValue = freeLookCamera.m_YAxis.Value
                              + verticalInput * verticalRotationSpeed * Time.deltaTime;

        // Clamp to [0..1] so you don't overshoot camera rigs.
        freeLookCamera.m_YAxis.Value = Mathf.Clamp01(newYAxisValue);
    }
}
