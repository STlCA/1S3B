using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectSave : MonoBehaviour
{
    public GameObject creat;	// 플레이어 닉네임 입력UI
    public Text[] slotText;		// 슬롯버튼 아래에 존재하는 Text들
    public Text newPlayerName;	// 새로 입력된 플레이어의 닉네임

    private bool[] savefile = new bool[3];

    void Start()
    {
        // 슬롯별로 저장된 데이터가 존재하는지 판단.
        for (int i = 0; i < 3; i++)
        {
            if (File.Exists(SaveSystem.path + $"{i}"))	// 데이터가 있는 경우
            {
                savefile[i] = true;
                SaveSystem.nowSlot = i;
                SaveSystem.SlotDataLoad();
                slotText[0].text = SaveSystem.slotData.Name;
                slotText[1].text = SaveSystem.slotData.Gold.ToString()+" G";                
            }
            else	// 데이터가 없는 경우
            {
                slotText[0].text = "비어있음";
                slotText[1].text = " G";
            }
        }

        SaveSystem.DataClear();
    }

    public void Slot(int number)
    {
        SaveSystem.nowSlot = number;

        if (savefile[number])	//true = 데이터 존재한다면
        {
            SaveSystem.Load();	// 데이터를 로드하고
            GoGame();	// 게임씬으로 이동
        }
        else	// bool 배열에서 현재 슬롯번호가 false라면 데이터가 없다는 뜻
        {
            Creat();	// 플레이어 닉네임 입력 UI 활성화
        }
    }

    public void Creat()	// 플레이어 닉네임 입력 UI를 활성화하는 메소드
    {
        creat.gameObject.SetActive(true);
    }

    public void GoGame()	// 게임씬으로 이동
    {
        if (!savefile[SaveSystem.nowSlot])	// false = 데이터가 없다면
        {
            SaveSystem.slotData.Name = newPlayerName.text; // 입력한 이름을 복사해옴
            SaveSystem.Save(); // 현재 정보를 저장함.
        }

        SceneManager.LoadScene(1); // 게임씬으로 이동
    }
}