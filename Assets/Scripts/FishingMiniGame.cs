using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingMiniGame : MonoBehaviour
{
    [System.Serializable]
    public class Key
    {
        public int ID;
        public GameObject keyPrefab;
    }

    [System.Serializable]
    public class SpawnedKey
    {
        public int ID;
        public GameObject spawnedKey;
    }

    public List<Key> keys;
    public List<SpawnedKey> keySequenceList;

    [SerializeField]
    private GameObject KeySequenceContainer;

    [SerializeField]
    private FishingManager fm;
    [SerializeField]
    private InventoryScriptableObject inventory;
    public FishBoolManager currentFishBoolManager;
    private ItemScriptableObject fishSO;

    private int keySequenceLength = 0;
    [SerializeField]
    private int currentInputActionID;
    [SerializeField]
    private int currentKeyIndex = 0;

    private const float MiniGameTime = 7.0f;
    private float currentMiniGameTime;
    private bool isStarted = false;
    private bool isResetting = false;

    private void Awake()
    {
        currentMiniGameTime = MiniGameTime;
    }

    private void Update()
    {
        SetCurrentInputActionID();
        HandleMinigame();
    }

    public void BeginMinigame(FishBoolManager fbm)
    {
        currentFishBoolManager = fbm;

        if (currentFishBoolManager == null) return;

        fishSO = currentFishBoolManager.GetRandomFish();
        SpawnKeySequence();
    }

    private void SetCurrentInputActionID()
    {
        if (GameInput.Instance.isArrowDownAction())
        {
            currentInputActionID = keys[0].ID;
        }
        else if(GameInput.Instance.isArrowLeftAction())
        {
            currentInputActionID = keys[1].ID;
        }
        else if (GameInput.Instance.isArrowRightAction())
        {
            currentInputActionID = keys[2].ID;
        }
        else if (GameInput.Instance.isArrowUpAction())
        {
            currentInputActionID = keys[3].ID;
        }
    }

    private void SetKeySequenceLength()
    {
        var fishGrade = fishSO.ColorGrade;

        if (fishGrade == ColorGrade.green)
        {
            keySequenceLength = 3;
        }
        else if (fishGrade == ColorGrade.blue)
        {
            keySequenceLength = 5;
        }
        else if (fishGrade == ColorGrade.yellow)
        {
            keySequenceLength = 8;
        }
        else
        {
            keySequenceLength = 11;
        }
    }

    private void HandleMinigame()
    {
        if (keySequenceList.Count <= 0) return;

        if (currentMiniGameTime <= 0f)
        {
            GameUI.Instance.miniGameTxt.text = "Hết thời gian, cá đã trốn thoát!";
            GameUI.Instance.miniGameTxt.color = Color.red;
            StartCoroutine(ResetMinigame());
            return;
        }

        if (isStarted)
        {
            currentMiniGameTime -= Time.deltaTime;
            currentMiniGameTime = Mathf.Max(currentMiniGameTime, 0f);

            GameUI.Instance.miniGameTimeBar.fillAmount =
                currentMiniGameTime / MiniGameTime;

            GameUI.Instance.miniGameTimeTxt.text =
                "Còn lại " + Mathf.CeilToInt(currentMiniGameTime) + " giây";
        }

        int maxKeyIndex = keySequenceLength - 1;

        if (GameInput.Instance.isArrowActions()) {
            isStarted = true;
            var currentKey = keySequenceList[currentKeyIndex];
            if (currentKey.ID != currentInputActionID)
            {
                GameUI.Instance.miniGameTxt.text = "Nhấn sai phím, cá đã trốn thoát!";
                GameUI.Instance.miniGameTxt.color = Color.red;
                currentKey.spawnedKey.GetComponent<Image>().color = Color.red;
                StartCoroutine(ResetMinigame());
                return;
            }
            currentKey.spawnedKey.GetComponent<Image>().color = Color.white;
            currentKeyIndex++;

            if (currentKeyIndex > maxKeyIndex)
            {
                GameUI.Instance.miniGameTxt.text = "Đã bắt được cá!";
                OnFishingSuccess();
                return;
            }
        }
    }

    private void SpawnKeySequence()
    {
        SetKeySequenceLength();
        keySequenceList = new List<SpawnedKey>();

        for (int i = 0; i < keySequenceLength; i++) {
            var randomKey = keys[Random.Range(0, keys.Count)];
            var spawnedKey = Instantiate(randomKey.keyPrefab, KeySequenceContainer.transform);
            keySequenceList.Add(new SpawnedKey
            {
                ID = randomKey.ID,
                spawnedKey = spawnedKey,
            });
        }
    }

    private void OnFishingSuccess()
    {
        StartCoroutine(ResetMinigame());

        if (fishSO != null)
        {
            int quantity = 1;
            StartCoroutine(GameUI.Instance.ToggleFishingPopUp());
            Item item = new Item(fishSO);
            GameUI.Instance.fishingPopUpTxt.text = "Đã bắt được " + item.ItemName + " x" + quantity;
            GameUI.Instance.fishingPopUpIcon.transform.Find("Icon").GetComponent<Image>().sprite = fishSO.Icon;
            GameUI.Instance.fishingPopUpIcon.transform.Find("Outline").GetComponent<Outline>().effectColor = ExtensionMethods.GetColorByGrade(fishSO.ColorGrade);
            inventory.AddItem(item, quantity);
            inventory.Save();
        }
    }

    private IEnumerator ResetMinigame()
    {
        if (isResetting) yield break;
        isResetting = true;
        isStarted = false;
        int timeBeforeReset = 2;
        yield return new WaitForSeconds(timeBeforeReset);
        isResetting = false;
        fm.CancelCastAnimation();
        fm.CancelCastOnTerrain();
        fm.ResetIsPlayingMinigame();
        currentKeyIndex = 0;
        keySequenceList.Clear();
        DestroyAllChildren(KeySequenceContainer.transform);
        currentMiniGameTime = MiniGameTime;
        ResetUI();
    }

    private void ResetUI()
    {
        GameUI.Instance.minigameScreen.SetActive(false);
        GameUI.Instance.miniGameTimeTxt.text = "Nhấn để bắt đầu";
        GameUI.Instance.miniGameTxt.text = "Nhấn lần lượt đúng theo các phím bên dưới trước khi cá kịp trốn thoát";
        GameUI.Instance.miniGameTxt.color = new Color32(255, 255, 0, 255);
        GameUI.Instance.miniGameTimeBar.fillAmount = currentMiniGameTime / MiniGameTime;
    }

    private void DestroyAllChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}