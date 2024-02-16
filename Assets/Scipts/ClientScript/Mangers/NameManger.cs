using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NameManger : MonoBehaviour
{
    public TMP_InputField PlayerName;//�û��������
    public TextMeshProUGUI testLog;//�û����Ƿ�����״̬�ı�
    public GameObject networkManager;
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(setName);
    }
    /// <summary>
    /// ��ʼ�������
    /// </summary>
    /// <summary>
    /// �󶨵���¼��ť��
    /// </summary>
    public void setName()
    {
        Debug.Log($"���Ը�Ϊ{PlayerName.text}");
        if (PlayerName.text.Length>15)
        {
            testLog.text = "�û������Ȳ��ܳ���15";
            
        }
        else if (PlayerName.text.Length > 0)
        {
            networkManager.SendMessage("SendName", PlayerName.text);
        }
        else
        {
            testLog.text = "�������û���";
        }
    }
}
