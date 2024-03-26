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


}
