using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private Vector2 _input;
    [SerializeField]
    private MouseSensitivity mouseSensitivity;
    private CameraRotation cameraRotation;
    [SerializeField]
    private CameraAngle cameraAngle;

    private InputActions inputActions;

    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Look.performed += Look;
        inputActions.Player.Look.canceled += Look;
    }

    private void OnDisable()
    {
        inputActions.Player.Look.performed -= Look;
        inputActions.Player.Look.canceled -= Look;
        inputActions.Disable();
    }

    private void Update()
    {
        cameraRotation.x += _input.x * mouseSensitivity.horizontal * Time.deltaTime;
        cameraRotation.y += _input.y * mouseSensitivity.vertical * Time.deltaTime;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y, cameraAngle.min, cameraAngle.max);
    }

    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(cameraRotation.y, cameraRotation.x, 0.0f);
    }

    public void Look(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }
}

[Serializable]
public struct MouseSensitivity
{
    public float horizontal;
    public float vertical;
}

public struct CameraRotation
{
    public float x;
    public float y;
}

[Serializable]
public struct CameraAngle
{
    public float min;
    public float max;
}
