using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
    public enum RecoveryType
    {
        None,
        Bonfire,
        Spa,
    }

    public enum MapTriggerType
    {
        FarmToTown,
        TownToFarm,
    }
    
    public enum PlayerMap
    {
        Farm,
        Town,
        Home
    }

    public enum PlayerState
    {
        IDLE,
        TIRED,

    }

    public enum PlayerSkillType
    {
        Farming,
        Felling,
        Mining,
        Battle,
        Fishing,
    }

    public enum PlayerEquipmentType
    {
        PickUp,
        Hoe,
        Water,
        Axe,
        PickAxe,
        Sword,
        FishingRod,
    }

    public enum UpgradeEquipmentStep
    {
        None,
        Copper,
        Still,
        Gold,
        Diamond,
        Bless,
    }
}
