using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotUI : ScrollSlotUI
{
    Inventory inventory;
    Item item;

    public override void Init()
    {
        base.Init();
        inventory = GameManager.Instance.Player.Inventory;
    }

    public override void Set(int idx)
    {
        base.Set(idx);
        item = inventory.GetItem(idx);


        // item 가지고 셋팅
    }
}


//public class StoreSlotUI : ScrollSlotUI
//{

//    public override void Set(int idx)
//    {
//        base.Set(idx);
//    }
//}
