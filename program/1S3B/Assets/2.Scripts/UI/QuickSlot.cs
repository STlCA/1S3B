using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlot : MonoBehaviour
{
    [SerializeField] private SlotUI[] slots;

    public void Init(GameManager gameManager, UIManager uiManager, Player player)
    {
        // 퀵 슬롯 길이 초기화
        slots = new SlotUI[9];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
