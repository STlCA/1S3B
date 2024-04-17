using UnityEngine;
using System.Collections.Generic;
using System.IO;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[System.Serializable]
public class ItemInfo
{
    public int ID;
    public string Name;
    public List<string> Description;
    public string Type;
    public string EquipType;
    public int CropID;
    public int SellGold;
    public int BuyGold;
    public int Stack;
    public string Path;
    public List<string> SpriteName;
    public int DropPercent;

    public List<Sprite> SpriteList;
    public bool canStack;

    public void Init()
    {
        foreach (string path in SpriteName)
        {
            Sprite sprite = Resources.Load<Sprite>(Path + "/" + path);
            SpriteList.Add(sprite);
            if (sprite == null)
                Debug.Log(Path + path + " is null");
        }

        // 스택 가능한 아이템인지 
        canStack = Stack == 1 ? false : true;
    }
}

// 아이템에 대한 정보를 가져온다
public class Item
{
    int no;
    public ItemInfo ItemInfo { get; set; }
    public int quantity;

    // 강화단계
    // 내구도
}

public abstract class EquipItem
{
    Item Item { get; set; }

    public abstract bool CanUse(Vector3Int target);
    public abstract bool Use(Vector3Int target);
    public virtual bool NeedTarget()
    {
        return true;
    }
}


[System.Serializable]
public class ItemDatabase
{
    public List<ItemInfo> ItemData;
    public Dictionary<int, ItemInfo> itemDic = new();

    public void Initialize()
    {
        foreach (ItemInfo item in ItemData)
        {
            item.Init();
            itemDic.Add(item.ID, item);
        }
    }

    public ItemInfo GetItemByKey(int id)
    {
        if (itemDic.ContainsKey(id))
            return itemDic[id];

        return null;
    }

    public Item Gacha()
    {
        ItemInfo info = ItemData[Random.Range(0, ItemData.Count)];
        Item newItem = new();
        newItem.ItemInfo = info;

        return newItem;
    }
}
