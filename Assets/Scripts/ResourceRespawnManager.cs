using System.Collections;
using UnityEngine;

public class ResourceRespawnManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float respawnTime = 240f;

    [Header("References")]
    private IInteractable currentInteractable;

    [Header("Flags")]
    private bool isDepleted;

    private void Awake()
    {
        if (currentInteractable == null)
        {
            currentInteractable = GetComponent<IInteractable>();
        }
    }

    public void DepleteResource()
    {
        if (isDepleted) return;

        isDepleted = true;
        StartCoroutine(ResourceRespawn());
    }

    private IEnumerator ResourceRespawn()
    {
        yield return new WaitForSeconds(respawnTime);

        isDepleted = false;
    }

    public bool IsDepleted() => isDepleted;
}
