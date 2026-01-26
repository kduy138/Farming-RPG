using System.Collections.Generic;
using UnityEngine;

public class FishBoolManager : MonoBehaviour, IItemDrop
{
    [System.Serializable]
    public class FishDrop
    {
        public ItemScriptableObject itemSO;
        [Range(0f, 100f)]
        public float dropRate;
    }

    public List<FishDrop> fishDrops;

    public ItemScriptableObject GetRandomItem()
    {
        float randomNumber = Random.Range(0f, 100f);
        List<FishDrop> possibleDrops = new List<FishDrop>();

        foreach (var drop in fishDrops)
        {
            if (randomNumber <= drop.dropRate)
                possibleDrops.Add(drop);
        }

        if (possibleDrops.Count == 0)
            return null;

        return possibleDrops[Random.Range(0, possibleDrops.Count)].itemSO;
    }

    public int GetRandomItemAmount()
    {
        return Random.Range(1, 1);
    }
}
