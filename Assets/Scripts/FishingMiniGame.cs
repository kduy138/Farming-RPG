using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private void Update()
    {
        SetCurrentInputActionID();
        HandleMinigame();
    }

    public void BeginMinigame(FishBoolManager fbm)
    {
        GameUI.Instance.castBtnText.text = "CÁ ĐÃ CẮN CÂU\nNHẤN SPACE ĐỂ KÉO";
        GameUI.Instance.castBtnText.enabled = true;

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
        int maxKeyIndex = keySequenceLength - 1;

        if (GameInput.Instance.isArrowActions()) {
            var currentKey = keySequenceList[currentKeyIndex];
            if (currentKey.ID != currentInputActionID)
            {
                Debug.Log("Minigame Failed");
                ResetMinigame();
                return;
            }
            currentKey.spawnedKey.GetComponent<Image>().color = Color.white;
            currentKeyIndex++;

            if (currentKeyIndex > maxKeyIndex)
            {
                Debug.Log("Minigame Finished");
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
        ResetMinigame();

        if (fishSO != null)
        {
            Item item = new Item(fishSO);
            inventory.AddItem(item, 1);
            inventory.Save();
        }
    }

    private void ResetMinigame()
    {
        fm.CancelCastAnimation();
        fm.CancelCastOnTerrain();
        fm.ResetIsPlayingMinigame();
        currentKeyIndex = 0;
        keySequenceList.Clear();
        DestroyAllChildren(KeySequenceContainer.transform);
        GameUI.Instance.minigameScreen.SetActive(false);
        GameUI.Instance.castBtnText.text = "NHẤN ĐỂ THẢ MỒI";
    }

    private void DestroyAllChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}