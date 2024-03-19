using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltip; // 툴팁으로 사용할 UI 오브젝트
    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(true); // 마우스 포인터가 오브젝트 위에 올라오면 툴팁을 활성화
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false); // 마우스 포인터가 오브젝트를 벗어나면 툴팁을 비활성화
    }
}
