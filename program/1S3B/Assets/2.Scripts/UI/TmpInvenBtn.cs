using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpInvenBtn : MonoBehaviour
{
    [SerializeField] private GameObject inventory;

    public void OnClickBtn()
    {
        inventory.SetActive(!inventory.activeSelf);
    }
}
