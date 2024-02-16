using System;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;

public class TeamData : MonoBehaviour
{
    public int teamid;
    public string leadername;
    private string[] playername=new string[4];
    private TextMeshProUGUI[] text=new TextMeshProUGUI[4];
    private int myID;
    private int[] playerIDs = new int[4];
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
                setplayerActive(i, true);
            }
            else
            {
                setplayerActive(i,false);
            }
        }
        for(int i = 0; i < 4; i++)
        {
            playerIDs[i] = 0;
            playername[i] = "";
        }
    }
    public void setID(int id)
    {
        myID = id;
        teamid = myID;
        text[0].text = $"牛仔{myID}";
        playerIDs[0] = myID;
    }
    public void SetLeader(string name)
    {
        leadername = name;
    }
    public void JoinTeam(string name,int id)
    {
        playerIDs[currentPlayerNum] = id;
        playername[currentPlayerNum] = name;
        text[currentPlayerNum].text= name;
        setplayerActive(currentPlayerNum, true);
        currentPlayerNum++;

    }
    /// <summary>
    /// 自己离开
    /// </summary>
    public void Leave()
    {
        teamid = myID;
        leadername = playername[0];
        for (int i = 1; i < 4; i++)
        {
            setplayerActive(i, false);
            playername[i] = "";
            text[i].text = "";
            playerIDs[i] = 0;
        }
        currentPlayerNum = 1;
    }
    /// <summary>
    /// 有人离开
    /// </summary>
    public bool HaveLeave(int id)
    {
        
        for (int i = 1; i < 4; i++)
        {
            if (playerIDs[i] == id)
            {
                playername[i] = "";
                //////////////////处理
                for(int j=i;j<3; j++)
                {
                    playername[j] = playername[j+1];
                    playerIDs[j] = playerIDs[j + 1];
                }
                currentPlayerNum--;
                playername[currentPlayerNum] = "";
                setplayerActive(currentPlayerNum, false);
                break;
            }
        }
        return currentPlayerNum == 1;
    }

    public void SetTeamData(int id,string name)//用作开始游戏
    {
        if (playername[id] == "")
        {
            setplayerActive(id, true);
        }
        text[id].text = name;
    }
    public void gameEnd()
    {
        for (int i = 0; i < 4; i++)
        {
            text[i].text = playername[i];
            if (playername[i] == "") { 
                setplayerActive(i, false);
            }
        }
    }
    private void setplayerActive(int index,bool active)
    {
        players[index].SetActive(active);
        cowBoys[index].SetActive(active);
    }
    public bool isFull()
    {
        return currentPlayerNum == 4;
    }
    public void ChangeName(int id,string name)
    {
        for(int i=0;i<4; i++)
        {
            if (playerIDs[i]== id)
            {
                Debug.Log($"名字改为{name}");
                playername[i]= name;
                text[i].text= name;
                break;
            }
        }
    }
}
