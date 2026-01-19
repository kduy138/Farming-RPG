using Unity.Cinemachine;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private float interactDistance = 3f;
    [SerializeField]
    private LayerMask interactLayer;
    [SerializeField]
    private Camera camera;
    private IInteractable currentInteractable;

    private void Update()
    {
        DetectInteractable();

        if (currentInteractable == null) return;

        if (GameInput.Instance.isMiningAction())
        {
            currentInteractable.Interact(GetComponent<Player>());
        }
    }

    private void DetectInteractable()
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
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

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.TryGetComponent(out IInteractable interactable))
    //    {
    //        currentInteractable = interactable;
    //        currentInteractable.OnFocus();
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.TryGetComponent(out IInteractable interactable))
    //    {
    //        if (currentInteractable == interactable)
    //        {
    //            currentInteractable.OnLostFocus();
    //            currentInteractable = null;
    //        }
    //    }
    //}
}
