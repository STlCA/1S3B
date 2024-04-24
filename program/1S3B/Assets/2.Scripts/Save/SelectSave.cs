/*using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectSave : MonoBehaviour
{
    public GameObject creat;	// �÷��̾� �г��� �Է�UI
    public Text[] slotText;		// ���Թ�ư �Ʒ��� �����ϴ� Text��
    public Text newPlayerName;	// ���� �Էµ� �÷��̾��� �г���

    private bool[] savefile = new bool[3];

    void Start()
    {
        // ���Ժ��� ����� �����Ͱ� �����ϴ��� �Ǵ�.
        for (int i = 0; i < 3; i++)
        {
            if (File.Exists(SaveSystem.path + $"{i}"))	// �����Ͱ� �ִ� ���
            {
                savefile[i] = true;
                SaveSystem.nowSlot = i;
                SaveSystem.SlotDataLoad();
                slotText[0].text = SaveSystem.slotData.Name;
                slotText[1].text = SaveSystem.slotData.Gold.ToString()+"G";                
            }
            else	// �����Ͱ� ���� ���
            {
                slotText[0].text = "�������";
                slotText[1].text = "G";
            }
        }

        SaveSystem.DataClear();
    }

    public void Slot(int number)
    {
        SaveSystem.nowSlot = number;

        if (savefile[number])	//true = ������ �����Ѵٸ�
        {
            SaveSystem.Load();	// �����͸� �ε��ϰ�
            GoGame();	// ���Ӿ����� �̵�
        }
        else	// bool �迭���� ���� ���Թ�ȣ�� false��� �����Ͱ� ���ٴ� ��
        {
            Creat();	// �÷��̾� �г��� �Է� UI Ȱ��ȭ
        }
    }

    public void Creat()	// �÷��̾� �г��� �Է� UI�� Ȱ��ȭ�ϴ� �޼ҵ�
    {
        creat.gameObject.SetActive(true);
    }

    public void GoGame()	// ���Ӿ����� �̵�
    {
        if (!savefile[DataManager.instance.nowSlot])	// ���� ���Թ�ȣ�� �����Ͱ� ���ٸ�
        {
            DataManager.instance.nowPlayer.name = newPlayerName.text; // �Է��� �̸��� �����ؿ�
            DataManager.instance.SaveData(); // ���� ������ ������.
        }
        SceneManager.LoadScene(1); // ���Ӿ����� �̵�
    }
}*/