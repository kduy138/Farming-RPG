using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FishBoolManager : MonoBehaviour
{
    [System.Serializable]
    public class FishDrop
    {
        public ItemScriptableObject itemSO;
        [Range(0f, 100f)]
        public float dropRate;
    }

    public List<FishDrop> fishDrops;

    public ItemScriptableObject GetRandomFish()
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
}
