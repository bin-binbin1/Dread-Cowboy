using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UIElements;
using UnityEngine.XR;
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
    public GameObject gameState;
    int roomid;
    public TeamData teamData;
    private Socket clientSocket;
    private const int BufferSize=1024;
    private byte[] buffer=new byte[BufferSize];
    public delegate void MessageHandler(byte[] msg);
    public TextMeshProUGUI testText;
    public GameObject chan;
    private int inviteLeaderId;
    // ����ί������
    private MessageHandler[] messageHandlers = new MessageHandler[20];
    state playerState,originState;
    
    // Start is called before the first frame update
    public async void Start()
    {
        playerState = state.alone;
        testText.text = "�뿪ʼ��Ϸ";
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAddress =System.Net.Dns.GetHostAddresses("whisperworld.cn")[0];
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

        messageHandlers[10] = HandleMatchStart;
        messageHandlers[11] = HandleGameStart;
        messageHandlers[12] = HandleGameOver;
        messageHandlers[13] = HandleMatchStop;
        //clientSocket.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, null);
        await StartReceiving();
    }
    //public void ReceiveCallback(IAsyncResult ar)
    //{
    //    int received = clientSocket.EndReceive(ar);

    //    // ����Ƿ��ѽ�����1KB������
    //    if (received == BufferSize)
    //    {
    //        // ������յ�������
    //        string data = Encoding.UTF8.GetString(buffer, 0, received);
    //        testText.text="Received data: " + buffer[0];
    //        if (buffer[0] < 20)
    //        {
    //            messageHandlers[buffer[0]](buffer);
    //        }
    //        else
    //        {
    //           testText.text=byte2str(buffer,1,8);
    //        }
    //        // ��ջ�������׼��������һ�����ݰ�
    //        Array.Clear(buffer, 0, BufferSize);
    //    }

    //    // �����û�н�����1KB�����ݣ��ͼ�������
    //    if (received < BufferSize)
    //    {
    //        clientSocket.BeginReceive(buffer, received, BufferSize - received, SocketFlags.None, ReceiveCallback, null);
    //    }
    //    else
    //    {
    //        // ����ѽ�����1KB�����ݣ��Ϳ�ʼ������һ�����ݰ�
    //        clientSocket.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, null);
    //    }
    //}

    public async Task StartReceiving()
    {
        while (true)
        {
            try
            {
                var received = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
                Debug.Log($"received:{received}");
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
        GameObject inviteinfo = GameObject.Find("InviteInfo");
        if (msg[1] == 0)
        {
            inviteinfo.GetComponent<Text>().text = "δ�ҵ����û�";
        }
        else
        {
            inviteinfo.GetComponent<Text>().text = $"�Ѿ���{byte2str(msg, 2, msg[0])}��������";
        }
    }
    private void HandleTeamJoin(byte[] msg)
    {
        string name = byte2str(msg, 2, msg[1]);
        playerState = userId==inviteLeaderId? state.leader:state.inTeam;
        teamData.JoinTeam(name);
    }
    private void HandleTeamLeave(byte[] msg)
    {
        string name = byte2str(msg, 2, msg[1]);
        if (teamData.HaveLeave(name))
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
        testText.text = "ƥ��ɹ������ڼ��ط��䡭��";
        
    }
    private void HandleRoundInfo(byte[] msg)
    {
        testText.text = "�غϿ�ʼ";
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
    private void HandleInviteReceived(byte[] msg)
    {
        // if û������
        if (true)
        {
            inviteLeaderId =byte2int(msg,1);
            Debug.Log("leader" + inviteLeaderId);
       
            string invitename= byte2str(msg, 6, msg[5]);
            object[] o = new object[2];
            o[0] = inviteLeaderId;
            o[1] = invitename;
            chan.SendMessage("SpawnObj", o);
        }
        
    }
    private void HandleMatchStart(byte[] msg)
    {
        playerState = state.matching;
        testText.text = "ƥ���С���";
        //���ض���
        //��Ϊȡ��ƥ�䰴ť
        // �����������Ĵ���
    }
    private void HandleGameStart(byte[] msg)
    {
        testText.text = "��Ϸ��ʼ!";
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
        testText.text = "��Ϸ����!";
        teamData.gameEnd();
        gameState.SendMessage("gameEnd");
        // �����������Ĵ���
    }
    private void HandleMatchStop(byte[] msg)
    {
        testText.text = "ȡ����ƥ��";
        // �����������Ĵ���
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
        teamData.SetMyName(name);
        teamData.SetLeader(name);
        str2byte(dataToSend,1,name);
        Debug.Log(dataToSend[0]);
        Debug.Log(dataToSend[1]);
        Debug.Log(dataToSend[2]);
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
        byte[]dataToSend=new byte[1024];
        dataToSend[0] = 2;
        int2byte(dataToSend,1,id);
        clientSocket.Send(dataToSend);
        Debug.Log(dataToSend[1]);
        Debug.Log(dataToSend[2]);
        Debug.Log(dataToSend[3]);
        Debug.Log(dataToSend[4]);
    }
    /// <summary>
    /// ͬ������
    /// </summary>
    public void Agree(object[] obj)
    {
        playerState = state.inTeam;
        int id=(int)obj[0];
        string leadername = (string)obj[1];
        byte[] dataToSend = new byte[1024];
        dataToSend[0] = 3;
        dataToSend[1]= 1;
        inviteLeaderId = id;
        int2byte(dataToSend, 2, id);
        clientSocket.Send(dataToSend);
        Debug.Log("����");
        Debug.Log(dataToSend[1]);
        Debug.Log(dataToSend[2]);
        Debug.Log(dataToSend[3]);
        Debug.Log(dataToSend[4]);
        teamData.SetLeader(leadername);
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
        Debug.Log("�ܾ�");
        Debug.Log(dataToSend[1]);
        Debug.Log(dataToSend[2]);
        Debug.Log(dataToSend[3]);
        Debug.Log(dataToSend[4]);
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
        GameObject invite = GameObject.Find("Invite");
        invite.SendMessage("CanInvite", !teamData.isFull());
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
        Array.Copy(b, 0, bytes, start + 1, b.Length);
        return start + 1 + b.Length;
    }
}