using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Net;
using System.Net.Sockets;
using TMPro;
using System.Threading.Tasks;

public class ClientAll : MonoBehaviour
{
    enum state
    {
        alone,inTeam,leader,inGame,matching
    }
    private const int PORT = 3690;
    public int userId=0;
    public GameObject gameState,inviteManager;
    int roomid;
    public TeamData teamData;
    private Socket clientSocket;
    private const int BufferSize=1024;
    private byte[] buffer=new byte[BufferSize];
    public delegate void MessageHandler(byte[] msg);
    public TextMeshProUGUI testText;
    public GameObject matchAnime;
    private int inviteLeaderId,inviteID;
    private string inviteName;
    // ����ί������
    private MessageHandler[] messageHandlers = new MessageHandler[20];
    state playerState,originState;
    private bool pvp = false;
    public bool test;
    // Start is called before the first frame update
    public async void Start()
    {
        playerState = state.alone;
        testText.text = "�뿪ʼ��Ϸ";
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAddress =test?IPAddress.Parse("127.0.0.1"): System.Net.Dns.GetHostAddresses("whisperworld.cn")[0];
        string ip = ipAddress.ToString();
        // ����
        clientSocket.Connect(ip, PORT);
        messageHandlers[0] = HandleUserId;
        messageHandlers[1] = HandleUserNotFound;
        messageHandlers[2] = HandleTeamJoin;
        messageHandlers[3] = HandleTeamLeave;
        messageHandlers[4] = HandleTeamDisband;
        messageHandlers[5] = HandleMatchSuccess;
        messageHandlers[6] = HandleRoundInfo;
        messageHandlers[7] = HandleRoundEnd;
        messageHandlers[8] = HandleInviteReceived;
        messageHandlers[9] = HandleTeamFailure;
        messageHandlers[10] = HandleMatchStart;
        messageHandlers[11] = HandleGameStart;
        messageHandlers[12] = HandleGameOver;
        messageHandlers[13] = HandleMatchStop;
        messageHandlers[14] = HandleNameChange;
        //clientSocket.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, null);
        await StartReceiving();
    }
    public async Task StartReceiving()
    {
        while (true)
        {
            try
            {
                var received = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
                if (received == BufferSize)
                {
                    // ������յ�������
                    Debug.Log("Received data: " + buffer[0]);
                    if (buffer[0] < 20)
                    {
                        messageHandlers[buffer[0]](buffer);
                    }
                    else
                    {
                        Debug.Log(byte2str(buffer, 1, 8));
                    }
                    // ��ջ�������׼��������һ�����ݰ�
                    Array.Clear(buffer, 0, BufferSize);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Error receiving: " + ex.Message);
                break;
            }
        }
    }

    private void HandleUserId(byte[] msg)
    {

        userId=byte2int(msg,1);
        
        teamData.setID(userId);
        inviteLeaderId = userId;
    }
    private void HandleUserNotFound(byte[] msg)
    {
        //����ĳ���Ϣ��ʾ�İ���
        ////////////////////////////////////////////
        
        if (msg[1] == 0)
        {
            testText.text = "δ�ҵ����û�";
        }
        else
        {
            testText.text = $"�Ѿ���{byte2str(msg, 2, msg[1])}��������";
        }
    }
    private void HandleTeamJoin(byte[] msg)
    {
        string name = byte2str(msg, 6, msg[5]);
        Debug.Log("name=" + name);
        playerState = userId==inviteLeaderId? state.leader:state.inTeam;
        teamData.JoinTeam(name,byte2int(msg,1));
    }
    private void HandleTeamLeave(byte[] msg)
    {
       
        if (teamData.HaveLeave(byte2int(msg,1)))
        {
            playerState = state.alone;
        }
    }
    private void HandleTeamDisband(byte[] msg)
    {
        teamData.Leave();
        playerState=state.alone;
    }
    private void HandleMatchSuccess(byte[] msg)
    {
        roomid=byte2int(msg,1);
        playerState = state.inGame;
        testText.text = "Success��Loading game����";
        
    }
    private void HandleRoundInfo(byte[] msg)
    {
        testText.text = "Round Start!";
        string st;
        int daoju = msg[1];
        int ptai = msg[2];
        st = ptai.ToString() + daoju.ToString();
        gameState.SendMessage("roundStart", st);
    }
    private void HandleRoundEnd(byte[] msg)
    {
        testText.text = "�غϽ���!";
        string st = "";
        for (int i = 1; i < 7; i++)
        {
            int cho = msg[i];
            st += cho.ToString();
            Debug.Log($"{i}ѡ��{cho}");
        }
        Debug.Log("ѡ��" + st);
        gameState.SendMessage("getChoices", st);
    }
    private void HandleTeamFailure(byte[] msg)
    {
        playerState = state.alone;
        inviteLeaderId = userId;
        testText.text = "�������ʧ�ܣ�����������ˣ������Ѿ���ʼƥ��";
    }
    private void HandleInviteReceived(byte[] msg)
    {
        // if û������
        if (playerState!=state.matching&&playerState!=state.inGame)
        {
            inviteID =byte2int(msg,1);
            inviteName= byte2str(msg, 6, msg[5]);
            testText.text = $"{inviteName}���㷢������";
        }
        
    }
    private void HandleMatchStart(byte[] msg)
    {
        playerState = state.matching;
        testText.text = "ƥ����";
        matchAnime.SetActive(true);
    }
    private void HandleGameStart(byte[] msg)
    {
        testText.text = "��Ϸ������ʼ!";
        gameState.SendMessage("gameStart", msg[1].ToString());
        int wei = 2;
        for (int i = 0; i < 4; i++)
        {
            string playername = byte2str(msg, wei + 1, msg[wei]);
            Debug.Log($"playername:{playername}");
            teamData.SetTeamData(i, playername);
            gameState.SendMessage("setName",i + playername);
            wei += msg[wei] + 1;
        }
    }
    private void HandleGameOver(byte[] msg)
    {
        testText.text = "��Ϸ������";
        teamData.gameEnd();
        gameState.SendMessage("gameEnd");
    }
    private void HandleMatchStop(byte[] msg)
    {
        testText.text = "ȡ����ƥ��";
        matchAnime.SetActive(false);
    }
    private void HandleNameChange(byte[] msg)
    {
        teamData.ChangeName(byte2int(msg, 1), byte2str(msg, 6, msg[5]));
    }

    void OnApplicationQuit()
    {
        if (clientSocket != null && clientSocket.Connected)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }

    public void SendName(string name)
    {
        byte[] dataToSend = new byte[BufferSize];
        dataToSend[0] = 1;
        dataToSend[1]=(byte)str2byte(dataToSend,2,name);
        clientSocket.Send(dataToSend);
    }

    public void makeChoice(int choice)
    {
        byte[] dataToSend = new byte[BufferSize];
        dataToSend[0] = 7;
        dataToSend[5] = (byte)choice;
        int2byte(dataToSend,1,roomid);
        clientSocket.Send(dataToSend);
    }


    public void DissolveTeam()
    {
        byte[] dataToSend = new byte[BufferSize];
        dataToSend[0] = 5;
        int2byte(dataToSend, 1, userId);
        clientSocket.Send(dataToSend);
        teamData.Leave();
        playerState = state.alone;
    }

    public void LeaveTeam()
    {
        byte[] dataToSend = new byte[1024];
        dataToSend[0] = 4;
        int2byte(dataToSend, 1, inviteLeaderId);
        inviteLeaderId = userId;
        playerState = state.alone;
        clientSocket.Send(dataToSend);
        teamData.Leave();
    }

    public void SendStartMatch()
    {
        originState = playerState;
        playerState = state.matching;
        byte[] dataToSend = new byte[1024];
        dataToSend[0] = 6;
        int2byte(dataToSend,1,inviteLeaderId);
        clientSocket.Send(dataToSend);
        pvp = true;
        Debug.Log("ƥ��");
    }

    public void SendStopMatch()
    {
        byte[] dataToSend = new byte[1024];
        dataToSend[0] = 8;
        clientSocket.Send(dataToSend);
        playerState = originState;
    }
    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="id"></param>
    public void InviteFriend(int id)
    {
        Debug.Log("�����id"+id);
        byte[] dataToSend=new byte[1024];
        dataToSend[0] = 2;
        int2byte(dataToSend,1,id);
        clientSocket.Send(dataToSend);
    }
    /// <summary>
    /// ͬ������
    /// </summary>
    public void Agree()
    {
        if (inviteID == 0)
            return;
        if(playerState==state.inTeam)
        {
            LeaveTeam();
        }else if (playerState == state.leader)
        {
            DissolveTeam();
        }
        playerState = state.inTeam;
        
        byte[] dataToSend = new byte[1024];
        dataToSend[0] = 3;
        dataToSend[1]= 1;
        inviteLeaderId = inviteID;
        int2byte(dataToSend, 2, inviteID);
        clientSocket.Send(dataToSend);
        teamData.SetLeader(inviteName);
        inviteID= 0;
    }
    /// <summary>
    /// �ܾ�
    /// </summary>
    public void Refuse(int id)
    { 
        
        byte[] dataToSend=new byte[1024];
        dataToSend[0] = 3;
        dataToSend[1] = 0;
        int2byte(dataToSend, 1, id);
        clientSocket.Send(dataToSend);
        inviteID=0;
        testText.text = "�Ѿܾ�";
    }
    public void leave()
    {
        if(playerState==state.inTeam)
        {
            LeaveTeam();
        }
        else if(playerState == state.leader)
        {
            DissolveTeam();
        }
    }
    /// <summary>
    /// �Ƿ��Ƕӳ�
    /// </summary>
    public void CanMatch()
    {
        GameObject match = GameObject.Find("Match");
        match.SendMessage("IsLeader", playerState == state.leader || playerState==state.alone) ;
    }
    /// <summary>
    /// �Ƿ���Ա
    /// </summary>

    public void IsFull()
    {
       
        inviteManager.SendMessage("CanInvite", teamData.isFull());
    }
    public void pe()
    {
        pvp = false;
        playerState = state.matching;
        byte[] dataToSend= new byte[1024];
        dataToSend[0] = 10;
        clientSocket.Send(dataToSend);
    }
    public void playAgain()
    {
        if(inviteLeaderId == userId)
        {
            if(pvp)
            {
                SendStartMatch();
            }
            else
            {
                pe();
            }
        }
    }
    private string byte2str(byte[] b,int start ,int l)
    {
        byte[] subset = new byte[l];
        Array.Copy(b, start, subset, 0, l);
        string result = Encoding.UTF8.GetString(subset);
        return result;
    }
    private int byte2int(byte[] b,int start)
    {
        byte[] data = new byte[4];
        Array.Copy(b, start, data, 0, 4);
        Array.Reverse(data);
        return BitConverter.ToInt32(data);
    }
    private void int2byte(byte[] bytes,int start, int val)
    {
        byte[] intBytes = BitConverter.GetBytes(val);
        Array.Reverse (intBytes);
        // ��ת������ֽ����鸴�Ƶ�Ŀ���ֽ������ָ��λ��
        Array.Copy(intBytes, 0, bytes, start, sizeof(int));
    }
    private int str2byte(byte[] bytes, int start, string val)
    {
        byte[] b=Encoding.UTF8.GetBytes(val);
        Array.Copy(b, 0, bytes, start, b.Length);
        return b.Length;
    }
}