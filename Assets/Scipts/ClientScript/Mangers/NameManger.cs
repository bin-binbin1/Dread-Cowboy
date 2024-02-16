using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NameManger : MonoBehaviour
{
    public TMP_InputField PlayerName;//用户名输入框
    public TextMeshProUGUI testLog;//用户名是否输入状态文本
    public GameObject networkManager;
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(setName);
    }
    /// <summary>
    /// 初始化输入框
    /// </summary>
    /// <summary>
    /// 绑定到登录按钮上
    /// </summary>
    public void setName()
    {
        Debug.Log($"尝试改为{PlayerName.text}");
        if (PlayerName.text.Length>15)
        {
            testLog.text = "用户名长度不能超过15";
            
        }
        else if (PlayerName.text.Length > 0)
        {
            networkManager.SendMessage("SendName", PlayerName.text);
        }
        else
        {
            testLog.text = "请输入用户名";
        }
    }
}
