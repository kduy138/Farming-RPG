using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance;

    private InputActions inputActions;

    private void Awake()
    {
        Instance = this;
        inputActions = new InputActions();
        inputActions.Enable();
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }

    public bool isForwardAction()
    {
        return inputActions.Player.Player_Forward.IsPressed();
    }

    public bool isBackwardAction()
    {
        return inputActions.Player.Player_Backward.IsPressed();
    }

    public bool isRightAction()
    {
        return inputActions.Player.Player_Right.IsPressed();
    }

    public bool isLeftAction()
    {
        return inputActions.Player.Player_Left.IsPressed();
    }

    public bool isMovement()
    {
        if(isForwardAction() || isBackwardAction() || isRightAction() || isLeftAction())
        {
            return true;
        }
        return false;
    }

    public bool isWalkAction()
    {
        return inputActions.Player.Player_Walk.WasPressedThisFrame();
    }

    public bool isCollectAction()
    {
        return inputActions.Player.Player_Collect.WasPressedThisFrame();
    }

    public bool isInventoryAction()
    {
        return inputActions.UI.UI_Inventory.WasPressedThisFrame();
    }

    public bool isCloseUIAction()
    {
        return inputActions.UI.UI_Close.WasPressedThisFrame();
    }

    public bool isToggleFishing()
    {
        return inputActions.Player.Player_Fishing.WasPressedThisFrame();
    }

    public bool isTriggerCast()
    {
        return inputActions.Player.Player_Cast.IsPressed();
    }

    public bool isTriggerFishingMinigame()
    {
        return inputActions.UI.UI_Fishing_Minigame.IsPressed();
    }

    public bool isArrowUpAction()
    {
        return inputActions.Player.Player_FishingMinigame_Arrow_Up.WasPressedThisFrame();
    }

    public bool isArrowRightAction()
    {
        return inputActions.Player.Player_FishingMinigame_Arrow_Right.WasPressedThisFrame();
    }

    public bool isArrowDownAction()
    {
        return inputActions.Player.Player_FishingMinigame_Arrow_Down.WasPressedThisFrame();
    }

    public bool isArrowLeftAction()
    {
        return inputActions.Player.Player_FishingMinigame_Arrow_Left.WasPressedThisFrame();
    }

    public bool isArrowActions()
    {
        if(isArrowUpAction() || isArrowRightAction() || isArrowDownAction() || isArrowLeftAction())
        {
            return true;
        }
        return false;
    }
}