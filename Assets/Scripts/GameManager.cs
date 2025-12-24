using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private InventoryScriptableObject inventory;
    [SerializeField]
    private InventoryScriptableObject equipment;
    [SerializeField]
    private Player player;

    private void Start()
    {
        inventory.Load();
        equipment.Load();
        player.Load();
    }

    private void OnApplicationQuit()
    {
        inventory.Save();
        equipment.Save();
        player.Save();
    }
}
