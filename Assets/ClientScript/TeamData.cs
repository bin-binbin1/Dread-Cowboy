using System;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
public class TeamData
{
    public string myname;
    public int teamid;
    public string leadername;
    public string[] playername=new string[4];
    public string[] room=new string[4];

    public void SetMyName(string name)
    {
        myname = name;
        playername[0] = myname;
    }
    public void SetLeader(string name)
    {
        leadername = name;
    }
    public void InTeam(string name,int num)
    {
        playername[num] = name;
    }
    /// <summary>
    /// 自己离开
    /// </summary>
    public void Leave()
    {
        for (int i = 1; i < 4; i++)
        {
            playername[i] = "";
        }
        leadername = myname;
    }
    /// <summary>
    /// 有人离开
    /// </summary>
    public void HaveLeave(string name)
    {
        for (int i = 1; i < 4; i++)
        {
            if (playername[i] == name)
                playername[i] = "";
        }
    }

    public void SetRoomData(int id,string playername)
    {
        room[id] = playername;
    }
    
}
