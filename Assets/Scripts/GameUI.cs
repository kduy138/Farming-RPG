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
    private float defaultXSpeed;
    private float defaultYSpeed;

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

    [Header("Buttons")]
    public GameObject removeItemBtn;
    public Button confirmRemoveBtn;
    public Button cancelDiscardBtn;

    [Header("Texts")]
    public TextMeshProUGUI confirmRemoveText;
    public TextMeshProUGUI silverCoinText;
    public TextMeshProUGUI castBtnText;
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI slotText;

    public static GameUI Instance;

    private void Awake()
    {
        Instance = this;
        DisableScreens();
    }

    private void Update()
    {
        ToggleInventoryScreen();
        CloseScreenWithEscape();
        ToggleFishingScreen();
        ToggleCastButton();
        ToggleMinigameScreen();

        if(fm.IsPlayingMinigame())
        {
            castBtnText.enabled = false;
        }
    }

    private void DisableScreens()
    {
        if (inventoryScreen) inventoryScreen.SetActive(false);
        if (confirmRemoveScreen) confirmRemoveScreen.SetActive(false);
        if (fishingScreen) fishingScreen.SetActive(false);
        if (minigameScreen) minigameScreen.SetActive(false);
        if (discardOptionHolder) discardOptionHolder.SetActive(false);
    }

    private void ToggleInventoryScreen()
    {
        if (!isInventoryOpen && GameInput.Instance.isInventoryAction())
        {
            isInventoryOpen = true;
            inventoryScreen.SetActive(true);
            camera.GetComponent<CinemachineInputAxisController>().enabled = false;
        }
        else if (isInventoryOpen && GameInput.Instance.isInventoryAction())
        {
            isInventoryOpen = false;
            inventoryScreen.SetActive(false);
            itemToolTipScreen.SetActive(false);
            camera.GetComponent<CinemachineInputAxisController>().enabled = true;
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
        if(fm.IsWaitingToCatch() == false || !minigameScreen.activeInHierarchy)
        {
            castBtnText.enabled = true;
        }
        else
        {
            castBtnText.enabled = false;
        }
    }

    private void ToggleMinigameScreen()
    {
        if (fm.IsPlayingMinigame() == true && GameInput.Instance.isTriggerFishingMinigame())
        {
            minigameScreen.SetActive(true);
        }
    }

    private void CloseScreenWithEscape()
    {
        if (GameInput.Instance.isCloseUIAction())
        {
            DisableScreens();
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
}
