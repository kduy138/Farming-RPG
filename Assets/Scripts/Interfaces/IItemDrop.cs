using NUnit.Framework;
using System.Collections.Generic;

public interface IItemDrop
{
    ItemScriptableObject GetRandomItem();
    int GetRandomItemAmount();
}
