using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public int ID;
    public string Name;
    public List<string> Description;
    public string Type;
    public string Season;
    public int SellGold;
    public int BuyGold;
    public int Stack;
    public string Path;
    public List<string> SpriteName;
    public bool canStack;

    public List<Sprite> SpriteList;

    public void Init()
    {
        // 이미지 할당
        foreach (string path in SpriteName)
        {
            SpriteList.Add(Resources.Load<Sprite>(Path + path));
        }

        // 스택 가능한 아이템인지 
        canStack = Stack == 1 ? false : true;
    }
}

// 아이템에 대한 정보를 가져온다
public abstract class ItemInstance
{
    int no;
    public Item item;

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
    public List<Item> ItemData;
    public Dictionary<int, Item> itemDic = new();

    public void Initialize()
    {
        foreach (Item item in ItemData)
        {
            itemDic.Add(item.ID, item);
        }
    }

    public Item GetItemByKey(int id)
    {
        if (itemDic.ContainsKey(id))
            return itemDic[id];

        return null;
    }
}
