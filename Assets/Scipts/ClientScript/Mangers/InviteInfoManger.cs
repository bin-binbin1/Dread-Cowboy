using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InviteInfoManger : MonoBehaviour
{
    public GameObject panel;
    //自动生成对象
    public void SpawnObj(object[] o)
    {
        int leaderid=(int)o[0];
        string leadername=(string)o[1];
        GameObject info = Resources.Load("infos", typeof(GameObject)) as GameObject;
        
        info.name=leaderid.ToString();
        Debug.Log(info.name);
        Debug.Log("名字"+leadername);
        info.GetComponentInChildren<Text>().text = leadername+"邀请你";
        Instantiate(info,panel.transform);
        Debug.Log("已生成");
    }
}
