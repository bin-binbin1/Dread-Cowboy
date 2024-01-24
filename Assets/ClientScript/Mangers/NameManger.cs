using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NameManger : MonoBehaviour
{
    public InputField PlayerName;//�û��������
    public Text stateTxt;//�û����Ƿ�����״̬�ı�
    private GameObject receiveCube;
    /// <summary>
    /// ��ʼ�������
    /// </summary>
    void Start()
    {
        PlayerName = GameObject.Find("PlayerName").GetComponent<InputField>();
        receiveCube = GameObject.Find("Client");
    }

    /// <summary>
    /// �󶨵���¼��ť��
    /// </summary>
    public void Login()
    {
        if (PlayerName.text.Length!=0)
        {
            Debug.Log("�û�����" + PlayerName.text);
            SceneManager.LoadScene(2);
            receiveCube.SendMessage("SendName", PlayerName.text);
        }
        else if (PlayerName.text.Length > 15)
        {
            stateTxt.text = "�û������Ȳ��ܳ���15";
        }
        else
        {
            stateTxt.text = "�������û���";
        }
    }
}
