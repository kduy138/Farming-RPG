using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private bool isInventoryOpen = false;

    [SerializeField]
    private CinemachineCamera camera;

    [Header("References")]
    [SerializeField]
    private FishingManager fm;

    [Header("Settings")]
    [SerializeField] 
    private float speed = 1f;

    [Header("Images")]
    public Image itemIcon;
    public Sprite removeIcon;
    public Sprite discardIcon;
    public Image weightBar;
    public Image slotBar;
    public GameObject itemPopUpIcon;
    public Image miniGameTimeBar;
    public Image staminaBar;
    public Image hpBar;
    public Image manaBar;
    public Image miningTimebar;
    public GameObject invContextMenu;

    [Header("Screens")]
    [SerializeField]
    private GameObject inventoryScreen;
    public GameObject confirmRemoveScreen;
    [SerializeField]
    private GameObject fishingScreen;
    public GameObject minigameScreen;
    [SerializeField]
    private GameObject itemToolTipScreen;
    public GameObject discardOptionHolder;
    public GameObject weightAndSlotCountHolder;
    public GameObject getItemPopUp;
    public GameObject playerDeadScreen;
    public GameObject miningScreen;
    public GameObject combatEquipmentSlotsContainer;
    public GameObject lifeSkillEquipmentSlotsContainer;

    [Header("Buttons")]
    public GameObject removeItemBtn;
    public Button confirmRemoveBtn;
    public Button cancelDiscardBtn;
    public Button getItemBtn;
    public Button useItemBtn;
    public Button splitItemBtn;

    [Header("Texts")]
    public TextMeshProUGUI confirmRemoveText;
    public TextMeshProUGUI silverCoinText;
    public TextMeshProUGUI castBtnText;
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI slotText;
    public TextMeshProUGUI getItemPopUpTxt;
    public TextMeshProUGUI miniGameTimeTxt;
    public TextMeshProUGUI miniGameTxt;
    public TextMeshProUGUI staminaTxt;
    public TextMeshProUGUI hpTxt;
    public TextMeshProUGUI manaTxt;
    public TextMeshProUGUI getItemWarningTxt;
    public TextMeshProUGUI levelTxt;
    public TextMeshProUGUI xpTxt;
    public TextMeshProUGUI miningTimeTxt;

    public static GameUI Instance;

    private void Awake()
    {
        Instance = this;
        DisableScreens();
        if (fishingScreen) fishingScreen.SetActive(false);
        if (minigameScreen) minigameScreen.SetActive(false);
    }

    private void Update()
    {
        ToggleInventoryScreen();
        CloseScreenWithEscape();
        ToggleFishingScreen();
        ToggleCastButton();
        ToggleMinigameScreen();
    }

    private void DisableScreens()
    {
        if (inventoryScreen) inventoryScreen.SetActive(false);
        if (confirmRemoveScreen) confirmRemoveScreen.SetActive(false);
        if (discardOptionHolder) discardOptionHolder.SetActive(false);
        if (getItemPopUp) getItemPopUp.SetActive(false);
        if (playerDeadScreen) playerDeadScreen.SetActive(false);
        if (miningScreen) miningScreen.SetActive(false);
        if (invContextMenu) invContextMenu.SetActive(false);
    }

    private void ToggleInventoryScreen()
    {
        if (!isInventoryOpen && GameInput.Instance.isInventoryAction())
        {
            isInventoryOpen = true;
            inventoryScreen.SetActive(true);
            CursorManager.Instance.UnlockCursor();
            CameraManager.Instance.DisableCamera();
        }
        else if (isInventoryOpen && GameInput.Instance.isInventoryAction())
        {
            isInventoryOpen = false;
            inventoryScreen.SetActive(false);
            itemToolTipScreen.SetActive(false);
            CursorManager.Instance.LockCursor();
            CameraManager.Instance.EnableCamera();
        }
    }

    private void ToggleFishingScreen()
    {
        if(fm.IsFishing() == true)
        {
            fishingScreen.SetActive(true);
            StartCoroutine(FadeInOutCastBtnText());
        }
        else
        {
            fishingScreen.SetActive(false);
            StopAllCoroutines();
        }
    }

    private void ToggleCastButton()
    {
        if(!fm.IsWaitingToCatch() && !fm.IsPlayingMinigame())
        {
            castBtnText.enabled = true;
            castBtnText.text = "NHẤN SPACE ĐỂ THẢ MỒI";
        }
        else if (fm.IsPlayingMinigame() && !minigameScreen.activeInHierarchy)
        {
            castBtnText.enabled = true;
            castBtnText.text = "NHẤN SPACE ĐỂ BẮT CÁ";
        }
        else
        {
            castBtnText.enabled = false;
        }
    }

    private void ToggleMinigameScreen()
    {
        if (fm.IsPlayingMinigame() && GameInput.Instance.isTriggerFishingMinigame())
        {
            minigameScreen.SetActive(true);
        }
    }

    private void CloseScreenWithEscape()
    {
        if (GameInput.Instance.isCloseUIAction())
        {
            DisableScreens();
            camera.GetComponent<CinemachineInputAxisController>().enabled = true;
        }
    }

    private IEnumerator FadeInOutCastBtnText()
    {
        while(true)
        {
            float elapsed = 0f;
            while (elapsed < speed)
            {
                elapsed += Time.deltaTime;
                float alpha = 1f - (elapsed / speed);
                Color c = castBtnText.color;
                c.a = alpha;
                castBtnText.color = c;
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < speed)
            {
                elapsed += Time.deltaTime;
                float alpha = elapsed / speed;
                Color c = castBtnText.color;
                c.a = alpha;
                castBtnText.color = c;
                yield return null;
            }
        }
    }

    public void ToggleGetItemPopUp()
    {
        getItemPopUp.SetActive(true);
    }
}