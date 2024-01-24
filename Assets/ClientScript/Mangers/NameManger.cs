using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NameManger : MonoBehaviour
{
    public InputField PlayerName;//用户名输入框
    public Text stateTxt;//用户名是否输入状态文本
    private GameObject receiveCube;
    /// <summary>
    /// 初始化输入框
    /// </summary>
    void Start()
    {
        PlayerName = GameObject.Find("PlayerName").GetComponent<InputField>();
        receiveCube = GameObject.Find("Client");
    }

    /// <summary>
    /// 绑定到登录按钮上
    /// </summary>
    public void Login()
    {
        if (PlayerName.text.Length!=0)
        {
            Debug.Log("用户名：" + PlayerName.text);
            SceneManager.LoadScene(2);
            receiveCube.SendMessage("SendName", PlayerName.text);
        }
        else if (PlayerName.text.Length > 15)
        {
            stateTxt.text = "用户名长度不能超过15";
        }
        else
        {
            stateTxt.text = "请输入用户名";
        }
    }
}
