using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InviteManger : MonoBehaviour
{
    GameObject friendname;
    bool isfull = false;
    private GameObject receiveCube;
    InputField tt;
    GameObject info;
    int lastid=0;
    public float timer = 100f;

    void Start()
    {
        receiveCube = GameObject.Find("Client");
        friendname = GameObject.Find("FriendName");
        info = GameObject.Find("InviteInfo");
    }

    void Update()
    {
        timer -= Time.deltaTime;
    }

    void CanInvite(bool l)
    {
        isfull = l;
    }

    public void InviteFriend()
    {
        info.GetComponent<Text>().text = "";
        receiveCube.SendMessage("IsFull");
        if (!isfull)
        {
            tt = friendname.GetComponentInChildren<InputField>();
            if (tt.text.Length!=0)
            {
                int frid=0;

                Debug.Log(tt.text);
                if (!int.TryParse(tt.text,out frid))
                    info.GetComponent<Text>().text = "������ID";
                else
                {
                    frid = int.Parse(tt.text);Debug.Log(frid);
                    if (lastid == frid && timer != 0)
                    {
                        info.GetComponent<Text>().text = "10s��ֻ����������һ��";
                    }
                    else
                    {
                        timer = 100f;
                        receiveCube.SendMessage("InviteFriend", frid);
                        lastid = frid;
                    }
                }
            }
        }
        else
        {
            info.GetComponent<Text>().text = "��Ա��";
        }
    }


}
