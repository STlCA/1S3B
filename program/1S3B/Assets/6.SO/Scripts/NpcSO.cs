using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NonPlayerCharacter", menuName = "Characters/Npc")]
public class NpcSO : ScriptableObject
{
    [field: SerializeField] public float PlayerChasingRange { get; private set; } = 0.1f;
    [field: SerializeField] public NpcGroundData groundedData { get; private set; }
}
