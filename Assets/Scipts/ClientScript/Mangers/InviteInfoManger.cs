using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InviteInfoManger : MonoBehaviour
{
    public GameObject panel;
    //�Զ����ɶ���
    public void SpawnObj(object[] o)
    {
        int leaderid=(int)o[0];
        string leadername=(string)o[1];
        GameObject info = Resources.Load("infos", typeof(GameObject)) as GameObject;
        
        info.name=leaderid.ToString();
        Debug.Log(info.name);
        Debug.Log("����"+leadername);
        info.GetComponentInChildren<Text>().text = leadername+"������";
        Instantiate(info,panel.transform);
        Debug.Log("������");
    }
}
