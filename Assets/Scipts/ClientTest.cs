using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Text;

public class ClientTest : MonoBehaviour
{
    private Socket socket;
    private byte[] buffer = new byte[1024];
    // Start is called before the first frame update
    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect("127.0.0.1", 3690);//�����ӷ�����
        Debug.Log("�������������");
        StartReceive();
        Send();
    }

    void ReceiveCallBack(IAsyncResult iar)//�ص�����
    {
        int len = socket.EndReceive(iar);//��Ϣ����
        if (len == 0)
            return;
        //�������������Ϣ
        string str = Encoding.UTF8.GetString(buffer, 0, len);
        Debug.Log(str);
        StartReceive();
    }

    void StartReceive()//���ܷ���
    {
        socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallBack, null);
    }

    void Send()
    {
        string str = "Hello unity";
        socket.Send(Encoding.UTF8.GetBytes(str));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
