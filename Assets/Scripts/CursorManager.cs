using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [SerializeField]
    private bool isCursorLocked = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Update()
    {
        if (GameInput.Instance.isCursorAction() && isCursorLocked)
        {
            UnlockCursor();
            CameraManager.Instance.DisableCamera();
        }
        else if (GameInput.Instance.isCursorAction() && !isCursorLocked)
        {
            LockCursor();
            CameraManager.Instance.EnableCamera();
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLocked = false;
    }
}
