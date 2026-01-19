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
        player.LoadPlayerData();
        CursorManager.Instance.LockCursor();
    }

    private void OnApplicationQuit()
    {
        inventory.Save();
        equipment.Save();
        player.SavePlayerData();
    }
}
