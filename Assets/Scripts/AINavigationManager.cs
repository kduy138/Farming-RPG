using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class AINavigationManager : MonoBehaviour
{
    public NavMeshAgent agent;

    private void Update()
    {
        if (GameInput.Instance.isLMBAITestAction()) {
            Vector2 mouseScreenPos = Mouse.current.position.ReadDefaultValue();

            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
            if(Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 pos = hitInfo.point;
                pos = new Vector3(pos.x, 0, pos.z);
                agent.SetDestination(pos);
            }
        }
    }
}
