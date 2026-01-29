using Unity.Cinemachine;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteractable;

    [Header("Settings")]
    [SerializeField]
    private float interactDistance = 0.5f;
    [SerializeField]
    private float interactRadius = 1f;
    [SerializeField]
    private LayerMask interactLayer;

    private void Update()
    {
        DetectInteractable();

        if (currentInteractable == null) return;

        if (GameInput.Instance.isInteractAction())
        {
            currentInteractable.Interact(GetComponent<Player>());
        }
    }

    private void DetectInteractable()
    {
        Camera camera = Camera.main;
        if (camera == null) return;

        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        Debug.DrawRay(camera.transform.position, ray.direction, Color.red);

        if (Physics.SphereCast(ray, interactRadius, out RaycastHit hit, interactDistance, interactLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if (currentInteractable != interactable)
                {
                    ClearInteractable();
                    currentInteractable = interactable;
                    currentInteractable.OnFocus();
                }
                return;
            }
        }
        ClearInteractable();
    }

    private void ClearInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable.OnLostFocus();
            currentInteractable = null;
        }
    }
}
