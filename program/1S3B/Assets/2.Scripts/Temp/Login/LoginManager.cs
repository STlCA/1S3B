using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    // �α��� ���� �ν��Ͻ�
    //static DatabaseReference db;

    // UI
    public TMP_InputField input_id;
    public TMP_InputField input_pw;

    // ����
    public string userId;

    public void Login()
    {
        //db = FirebaseDatabase.DefaultInstance.RootReference;

        //db.Child("user").GetValueAsync().ContinueWithOnMainThread(task =>
        //{
        //    if (task.IsFaulted)
        //    {
        //        Debug.Log("����");
        //    }
        //    else{
        //        DataSnapshot snapshot = task.Result;
        //
        //        if (snapshot.Child(input_id.text).Value != null && 
        //            snapshot.Child(input_id.text).Value.Equals(input_pw.text)) // ���̵�, ��� ��ġ
        //        {
        //            userId = input_id.text;
        //
        //            Debug.Log("��ġ");
        //            DontDestroyOnLoad(gameObject);
        //            SceneManager.LoadScene(1);
        //        }
        //        else
        //        {
        //            Debug.Log("Ʋ��");
        //        }
        //    }
        //});
    }

    static void WriteData()
    {
        //db = FirebaseDatabase.DefaultInstance.RootReference;

        InputUser("12", "12");
        InputUser("34", "34");
        InputUser("56", "56");

        Debug.Log("�ۼ��Ϸ�");
    }

    public static void InputUser(string userId, string userPw)
    {
        //db.Child("user").Child(userId).Push();
        //db.Child("user").Child(userId).SetValueAsync(userPw);
    }
}
