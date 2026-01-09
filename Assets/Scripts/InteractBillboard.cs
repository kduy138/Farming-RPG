using Unity.Cinemachine;
using UnityEngine;

public class InteractBillboard : MonoBehaviour
{
    private CinemachineCamera camera;

    private void Awake()
    {
        camera = FindAnyObjectByType<CinemachineCamera>();
    }

    private void LateUpdate()
    {
        RotateBillboardToFaceCamera();
    }

    private void RotateBillboardToFaceCamera()
    {
        if (camera == null)
        {
            Debug.Log("Không tìm thấy Cinemachine Camera trên Scene hiện tại!");
            return;
        }
        Vector3 direction = transform.position - camera.transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
