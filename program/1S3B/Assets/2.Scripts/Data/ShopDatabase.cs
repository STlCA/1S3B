using UnityEngine;
using System.Collections.Generic;
using System.IO;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[System.Serializable]
public class ShopInfo
{
    public int ID;
    public string Name;
    public List<int> Items;
}

// 상점에 대한 정보를 가져온다
//public class Shop
//{
//    int no;
//    public ShopInfo ShopInfo { get; set; }
//}

[System.Serializable]
public class ShopDatabase
{
    public List<ShopInfo> ShopData;
    public Dictionary<int, ShopInfo> shopDic = new();

    public void Initialize()
    {
        foreach (ShopInfo shop in ShopData)
        {
            shopDic.Add(shop.ID, shop);
        }
    }

    public ShopInfo GetShopByKey(int id)
    {
        if (shopDic.ContainsKey(id))
            return shopDic[id];

        return null;
    }
}
