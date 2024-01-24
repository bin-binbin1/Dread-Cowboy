using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using System.Net;


public class ClientAll : MonoBehaviour
{
    
    private const int PORT = 3690;
    public int userId=0;
    private int LeaderId =0;
    public byte[] da =new byte[1024] ;
    int teamnum = 1;
    int roomid;

    private ClientSocket clientSocket = new ClientSocket();
    TeamData teamData = new TeamData();
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        
       FirstConnect();
    } 
    private void FirstConnect()
    {
            if (clientSocket.connected)
            {
                // 断开
                clientSocket.CloseSocket();
            }
            else
            {
            IPAddress ipAddress = System.Net.Dns.GetHostAddresses("whisperworld.cn")[0];
            string ip = ipAddress.ToString();
            // 连接
            clientSocket.Connect(ip, PORT);
            
            }
        }

    private void Update()
    {
        if (clientSocket.connected)
        {
            clientSocket.BeginReceive();
        }
        byte[] msg = new byte[1024];
         
        
        if (clientSocket.GetMsgFromQueue())
        {
            msg = clientSocket.m_msgQueue.Dequeue();
            
            if (msg[0] == 0)
            {
                byte[] data = new byte[4];
                Array.Copy(msg, 1, data, 0, 4);
                Array.Copy(msg,da, msg.Length);
                Array.Reverse(data);
                userId=System.BitConverter.ToInt32(data);
                LeaderId = userId;
                Debug.Log("userId"+userId);
            }
            if (msg[0] == 1)
            {
                GameObject inviteinfo= GameObject.Find("InviteInfo");
                if (msg[1] == 0)
                {
                    inviteinfo.GetComponent<Text>().text = "未找到该用户";
                }
            }
            //告知有人入队
            if (msg[0] == 2)
            {
                byte[] data = new byte[msg[1]];
                Array.Copy(msg, 2, data, 0, msg[1] );
                string name=System.Text.Encoding.UTF8.GetString(data);
                teamData.InTeam(name, teamnum);
                teamnum++;
                for(int i = 0; i < teamnum; i++)
                {
                    Debug.Log(teamData.playername[i]);
                }
            }
            //告知有人离队
            if (msg[0] == 3)
            {
                byte[] data=new byte[msg[1]];
                Array.Copy(msg, 2, data, 0, msg[1]);
                string name = System.Text.Encoding.UTF8.GetString(data);
                teamData.HaveLeave(name);
                teamnum--;
            }
            //解散
            if (msg[0] == 4)
            {
                LeaderId = userId;
                teamnum = 1;
                teamData.Leave();
            }
            //匹配成功
            if(msg[0] == 5)
            {
                byte[] data = new byte[4];
                Array.Copy(msg, 1, data, 0, 4);
                Array.Reverse(data);
                roomid = System.BitConverter.ToInt32(data);
                Debug.Log("roomid:" + roomid);
            }
            //接收到回合信息
            if( msg[0] == 6)
            {
                GameObject gamestate = GameObject.Find("GameState");
                string st;
                int daoju = msg[1];
                int ptai=msg[2];
                st = ptai.ToString()+daoju.ToString();
                gamestate.SendMessage("roundStart", st);
            }
            //单回合结束
            if (msg[0] == 7)
            {
                GameObject gamestate = GameObject.Find("GameState");
                string st="";
                for(int i = 1; i < 5; i++)
                {
                    int cho = msg[i];
                    st += cho.ToString();
                }
                Debug.Log("选择" + st);
                gamestate.SendMessage("getChoices", st);
            }

            //接收到邀请
            if (msg[0] == 8)
            {
                GameObject chan = GameObject.Find("Chan");
                byte[] data = new byte[4];
                Array.Copy(msg, 1, data, 0, 4);
                Array.Reverse(data);
                Debug.Log("接受");
                Debug.Log(data[0]);
                Debug.Log(data[1]);
                Debug.Log(data[2]);
                Debug.Log(data[3]);
                int leaderid=System.BitConverter.ToInt32(data);
                Debug.Log("leader"+leaderid);
                byte[] data2=new byte[msg[5]];
                Array.Copy(msg, 6, data2, 0, msg[5]);
                string invitename=System.Text.Encoding.UTF8.GetString(data2);
                object[] o = new object[2];
                o[0] = leaderid;
                o[1] = invitename;
                chan.SendMessage("SpawnObj",o);
            }
           
            //开始匹配通知
            if(msg[0] == 10)
            {
                GameObject Match = GameObject.Find("Match");
                
            }
            //游戏开始
            if (msg[0] == 11)
            {
                GameObject gamestate = GameObject.Find("GameState");
                
                int playerid = msg[1];
                string state = playerid.ToString();
                
                gamestate.SendMessage("gameStart", state);
                int wei = 2;
                for (int i = 0; i < 4; i++)
                {
                    byte[] data = new byte[msg[wei]];
                    Array.Copy(msg, wei, data, 0, msg[wei]);
                    string playername=System.Text.Encoding.UTF8.GetString(data);
                    teamData.SetRoomData(i, playername);
                    wei+=msg[wei]+1;
                }
            }
            //游戏结束
            if (msg[0] == 12)
            {

            }
            //停止匹配通知
            if (msg[0] == 13)
            {

            }
        }
    }

    private void Send(string str)
    {
        // string转byte[]
        byte[] data = new byte[1024]; 
            data = System.Text.Encoding.UTF8.GetBytes(str);
        // 发送消息给服务端
        clientSocket.SendData(data);
    }

    private void OnApplicationQuit()
    {
        if (clientSocket.connected)
        {
            clientSocket.CloseSocket();
        }
    }

    public void SendName(string name)
    {
        teamData.SetMyName(name);
        teamData.SetLeader(name);
        byte[] data=new byte[1024];
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(name);
        data[0] = 1;
        data[1] = (byte)bytes.Length;
        
        
        Array.Copy(bytes, 0, data, 2, bytes.Length);
        Debug.Log(data[0]);
        Debug.Log(data[1]);
        Debug.Log(data[2]);
        clientSocket.SendData(data);
    }
    /// <summary>
    /// 玩家选择
    /// </summary>
    public void Choice(int choice)
    {
        byte[] data = new byte[1024];
        data[0] = 7;
        data[5] = (byte)choice;
        byte[] bytes = BitConverter.GetBytes(roomid);
        Array.Reverse(bytes);
        Array.Copy(bytes, 0, data, 1, 4);
        clientSocket.SendData(data);
    }

    /// <summary>
    /// 解散队伍
    /// </summary>
    public void DissolveTeam()
    {
        byte[] data = new byte[1024];
        data[0] = 5;
        byte[] bytes = BitConverter.GetBytes(LeaderId);
        Array.Reverse(bytes);
        Array.Copy(bytes, 0, data, 1, 4);
        clientSocket.SendData(data);
        teamnum = 1;
        teamData.Leave();
    }
    /// <summary>
    /// 离开队伍
    /// </summary>
    public void LeaveTeam()
    {
        byte[] data = new byte[1024];
        data[0] = 4;
        byte[] bytes = BitConverter.GetBytes(LeaderId);
        Array.Reverse(bytes);
        Array.Copy(bytes, 0, data, 1, 4);
        clientSocket.SendData(data);
        LeaderId = userId;
        teamnum = 1;
        teamData.Leave();
    }
    /// <summary>
    /// 发送开始匹配
    /// </summary>
    public void SendStartMatch()
    {
        byte[] data = new byte[1024];
        byte[] bytes= BitConverter.GetBytes(LeaderId);
        Array.Reverse(bytes);
        data[0] = 6;
        Array.Copy(bytes,0,data,1,4);
        clientSocket.SendData(data);
        Debug.Log("匹配");
        Debug.Log(data[1]);
        Debug.Log(data[2]);
        Debug.Log(data[3]);
        Debug.Log(data[4]);
    }
    /// <summary>
    /// 发送停止匹配
    /// </summary>
    public void SendStopMatch()
    {
        byte[] data = new byte[1024];
        data[0] = 8;
        clientSocket.SendData(data);
    }
    /// <summary>
    /// 发送邀请
    /// </summary>
    /// <param name="id"></param>
    public void InviteFriend(int id)
    {
        byte[]data=new byte[1024];
        data[0] = 2;
        byte[] bytes=BitConverter.GetBytes(id);
        Array.Reverse(bytes);
        Array.Copy(bytes,0,data,1,4);
        clientSocket.SendData(data);
        Debug.Log(data[1]);
        Debug.Log(data[2]);
        Debug.Log(data[3]);
        Debug.Log(data[4]);
    }
    /// <summary>
    /// 同意邀请
    /// </summary>
    public void Agree(object[] obj)
    {
        int id=(int)obj[0];
        string leadername = (string)obj[1];
        byte[] data = new byte[1024];
        data[0] = 3;
        data[1]= 1;
        byte[] bytes=BitConverter.GetBytes(id);
        LeaderId = id;
        Array.Reverse(bytes);
        Array.Copy(bytes, 0, data, 2, 4);
        clientSocket.SendData(data);
        Debug.Log("接受");
        Debug.Log(data[1]);
        Debug.Log(data[2]);
        Debug.Log(data[3]);
        Debug.Log(data[4]);
        teamData.SetLeader(leadername);
    }
    /// <summary>
    /// 拒绝
    /// </summary>
    public void Refuse(int id)
    {
        byte[] data=new byte[1024];
        data[0] = 3;
        data[1] = 0;
        byte[] bytes = BitConverter.GetBytes(id);
        Array.Reverse(bytes);
        Array.Copy(bytes, 0, data, 2, 4);
        clientSocket.SendData(data);
        Debug.Log("拒绝");
        Debug.Log(data[1]);
        Debug.Log(data[2]);
        Debug.Log(data[3]);
        Debug.Log(data[4]);
    }
    /// <summary>
    /// 是否是队长
    /// </summary>
    public void CanMatch()
    {
        GameObject match = GameObject.Find("Match");
        match.SendMessage("IsLeader", userId == LeaderId);
    }
    /// <summary>
    /// 是否满员
    /// </summary>

    public void IsFull()
    {
        GameObject invite = GameObject.Find("Invite");
        invite.SendMessage("CanInvite",teamnum == 4);
    }
}