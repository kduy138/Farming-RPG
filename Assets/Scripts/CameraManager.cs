using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    private Vector2 _input;
    [SerializeField]
    private MouseSensitivity mouseSensitivity;
    private CameraRotation cameraRotation;
    [SerializeField]
    private CameraAngle cameraAngle;
    private CinemachineCamera camera;

    private InputActions inputActions;

    private void Awake()
    {
        Instance = this;
        inputActions = new InputActions();
        camera = FindAnyObjectByType<CinemachineCamera>();
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

    public void DisableCamera()
    {
        if (camera == null) return;
        camera.GetComponent<CinemachineInputAxisController>().enabled = false;
    }

    public void EnableCamera()
    {
        if (camera == null) return;
        camera.GetComponent<CinemachineInputAxisController>().enabled = true;
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
