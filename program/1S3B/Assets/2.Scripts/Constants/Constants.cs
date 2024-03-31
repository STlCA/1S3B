using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
    public class ConstantsString : MonoBehaviour
    {
        //AnimationController
        public static string IsStart = "isStart";
        public static string IsWalking = "isWalking";
        public static string IsDeath = "isDeath";

        public static string InputX = "inputX";
        public static string InputY = "inputY";

        public static string SaveX = "saveX";
        public static string SaveY = "saveY";
    }

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
        BLACKOUT,
        MAPCHANGE,
        SLEEP,
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

    public enum NeedUpdateObject
    {
        None,
        Need
    }
}
