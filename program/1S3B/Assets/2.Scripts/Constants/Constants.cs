using System;
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
        public static string IsCarry = "isCarry";
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
        Farm_FarmRoad,
        FarmRoad_Town,
        Town_FarmRoad,
        FarmRoad_Farm,

        Farm_ForestRoad,
        ForestRoad_Forest,
        Forest_ForestRoad,
        ForestRoad_Farm,

        Forest_BossRoom,
        BossRoom_Forest,

        Town_TownRoad,
        TownRoad_Forest,
        Forest_TownRoad,
        TownRoad_Town,

        Town_Beach,
        Beach_Island,

        Beach_BeachR,
        BeachR_Quarry,
        Quarry_Island,
        Island_Quarry,
        Quarry_BeachR,
        BeachR_Beach,

        Beach_BeachL,
        BeachL_Beach,
        BeachL_Forest,
        Forest_Beach,
    }
    
    public enum PlayerMap
    {
        Farm,
        FarmRoad,
        Town,
        TownRoad,
        Forest,
        ForestRoad,
        Beach,
        BeachR,
        BeachL,
        BossRoom,
        Island,
        Quarry,
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
        Hand,
        PickUp,
        Hoe,
        Water,
        Axe,
        PickAxe,
        Sword,
        FishingRod,
        Carry,
        Seed,
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
        Need,
    }

    public enum TreeInteractable
    {
        None,
        Can
    }

    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }

    public enum SpawnType
    {
        NaturePoint,
        TreePoint,
        StonePoint,       
        Quarry,
        MapNature,
        UpForest,
        DownForest,
        Farm,
    }

    public enum SFXSound
    {
      
    }

    public enum WoodType
    {
        Wood = 4001,
    }
    public enum StoneType
    {
        Stone = 4011,
        Cooper,
        Iron,
        Gold,
        Diamond,
    }
}
