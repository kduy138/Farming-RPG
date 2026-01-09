using System.Collections.Generic;
using UnityEngine;

public class MiningResourceManager : MonoBehaviour
{
    [System.Serializable]
    public class MiningResource
    {
        public ItemScriptableObject itemSO;
        [Range(0f, 100f)]
        public float dropRate;
    }

    public List<MiningResource> resources;

    public ItemScriptableObject GetRandomResource()
    {
        float randomNumber = Random.Range(0f, 100f);
        List<MiningResource> possibleResources = new List<MiningResource>();

        foreach (var resource in resources)
        {
            if (randomNumber <= resource.dropRate)
                possibleResources.Add(resource);
        }

        if (possibleResources.Count == 0)
            return null;

        return possibleResources[Random.Range(0, possibleResources.Count)].itemSO;
    }
}
