using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NameManger : MonoBehaviour
{
    public InputField PlayerName;//�û��������
    public Text stateTxt;//�û����Ƿ�����״̬�ı�
    private GameObject networkManager;
    /// <summary>
    /// ��ʼ�������
    /// </summary>
    void Start()
    {
        PlayerName = GameObject.Find("PlayerName").GetComponent<InputField>();
    }

    /// <summary>
    /// �󶨵���¼��ť��
    /// </summary>
    public void setName()
    {
        if (PlayerName.text.Length>15)
        {
            stateTxt.text = "�û������Ȳ��ܳ���15";
            
        }
        else if (PlayerName.text.Length > 0)
        {
            Debug.Log("�û�����" + PlayerName.text);
            networkManager.SendMessage("SendName", PlayerName.text);
        }
        else
        {
            stateTxt.text = "�������û���";
        }
    }
}
