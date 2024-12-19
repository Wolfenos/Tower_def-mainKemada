using UnityEngine;
using Cinemachine;

public class CombinedCameraOrbit : MonoBehaviour
{
    [Header("Cinemachine FreeLook Reference")]
    public CinemachineFreeLook freeLookCamera;

    [Header("Keyboard Rotation Settings")]
    public bool enableKeyboardRotation = true;
    public KeyCode rotateLeftKey = KeyCode.A;
    public KeyCode rotateRightKey = KeyCode.D;
    public KeyCode rotateUpKey = KeyCode.W;
    public KeyCode rotateDownKey = KeyCode.S;
    public float keyboardHorizontalSpeed = 50f;
    [Tooltip("Vertical speed is normalized for m_YAxis (0..1).")]
    public float keyboardVerticalSpeed = 0.5f;

    [Header("Mouse Drag Settings")]
    public bool enableMouseDragRotation = true;
    [Tooltip("Hold right mouse button to rotate the camera.")]
    public bool holdRightMouseToRotate = true;
    public float mouseHorizontalSpeed = 100f;
    public float mouseVerticalSpeed = 0.5f;

    void Update()
    {
        if (freeLookCamera == null)
            return;

        // -- KEYBOARD ROTATION --
        if (enableKeyboardRotation)
        {
            float horizontalInput = 0f;
            float verticalInput = 0f;

            if (Input.GetKey(rotateLeftKey)) horizontalInput -= 1f;
            if (Input.GetKey(rotateRightKey)) horizontalInput += 1f;
            if (Input.GetKey(rotateUpKey)) verticalInput += 1f;
            if (Input.GetKey(rotateDownKey)) verticalInput -= 1f;

            // Horizontal orbit (m_XAxis.Value in degrees)
            freeLookCamera.m_XAxis.Value += horizontalInput * keyboardHorizontalSpeed * Time.deltaTime;

            // Vertical orbit (m_YAxis.Value in [0..1])
            float newYAxisValue = freeLookCamera.m_YAxis.Value +
                                  verticalInput * keyboardVerticalSpeed * Time.deltaTime;
            freeLookCamera.m_YAxis.Value = Mathf.Clamp01(newYAxisValue);
        }

        // -- MOUSE DRAG ROTATION --
        if (enableMouseDragRotation)
        {
            // Only rotate with mouse if not requiring right-button OR if right-button is held
            bool isRightMouseHeld = Input.GetMouseButton(1);
            if (!holdRightMouseToRotate || isRightMouseHeld)
            {
                // "Mouse X" and "Mouse Y" come from Unity's old Input system axes
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                // Horizontal orbit
                freeLookCamera.m_XAxis.Value += mouseX * mouseHorizontalSpeed * Time.deltaTime;

                // Vertical orbit (again, clamp [0..1])
                float newYAxisValue = freeLookCamera.m_YAxis.Value +
                                      mouseY * mouseVerticalSpeed * Time.deltaTime;
                freeLookCamera.m_YAxis.Value = Mathf.Clamp01(newYAxisValue);
            }
        }
    }
}
