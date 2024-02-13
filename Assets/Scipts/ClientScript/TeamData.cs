using System;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;

public class TeamData : MonoBehaviour
{
    public string myname;
    public int teamid;
    public string leadername;
    private string[] playername=new string[4];
    private string[] playerNameInRoom = new string[4];
    private TextMeshProUGUI[] text=new TextMeshProUGUI[4];
    private int myID;
    public GameObject[] players,cowBoys;
    private int currentPlayerNum;
    void Start()
    {
        currentPlayerNum = 1;
        for(int i = 0; i < players.Length; i++)
        {
            text[i] = players[i].GetComponentInChildren<TextMeshProUGUI>();
            if (i == 0)
            {
                players[i].SetActive(true);
                cowBoys[i].SetActive(true);
            }
            else
            {
                players[i].SetActive(false);
                cowBoys[i].SetActive(false);
            }
        }
    }
    public void setID(int id)
    {
        myID = id;
        teamid = myID;
        myname = $"牛仔{myID}";
        text[0].text= myname;
    }
    public void SetMyName(string name)
    {
        myname = name;
        playername[0] = myname;
        text[0].text= myname;
    }
    public void SetLeader(string name)
    {
        leadername = name;
    }
    public void JoinTeam(string name)
    {
        playername[currentPlayerNum] = name;
        text[currentPlayerNum].text= myname;
        currentPlayerNum++;

    }
    /// <summary>
    /// 自己离开
    /// </summary>
    public void Leave()
    {
        teamid = myID;
        leadername = myname;
        playername[0] = myname;
        for (int i = 1; i < 4; i++)
        {
            playername[i] = null;
            text[i].text = "";
        }
        leadername = myname;
        currentPlayerNum = 1;
    }
    /// <summary>
    /// 有人离开
    /// </summary>
    public bool HaveLeave(string name)
    {
        
        for (int i = 1; i < 4; i++)
        {
            if (playername[i] == name)
            {
                playername[i] = null;
                //////////////////处理
                for(int j=i;j<3; j++)
                {
                    playername[j] = playername[j+1];
                }
                currentPlayerNum--;
                playername[currentPlayerNum] = null;
                players[currentPlayerNum].SetActive(false);
                cowBoys[currentPlayerNum].SetActive(false);
                break;
            }
        }
        return currentPlayerNum == 1;
    }

    public void SetTeamData(int id,string name)//用作开始游戏
    {
        if (playername[id] == null)
        {
            players[id].SetActive(true);
            cowBoys[id].SetActive(true);
            text[id].text = name;
        }
        playerNameInRoom[id] = name;
    }
    public void gameEnd()
    {
        for(int i=0;i<4;i++)
        {
            text[i].text = playername[i] == null ? "" : playername[i];
            if (playername[i] == null)
            {
                players[i].SetActive(false);
                cowBoys[i].SetActive(false);
            }
        }
    }
    public bool isFull()
    {
        return currentPlayerNum == 4;
    }
}
