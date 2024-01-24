using System;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class ClientSocket
{
    private Socket init()
    {
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // 接收的消息数据包大小限制为 1024
        m_recvBuff = new byte[1024];
        m_recvCb = new AsyncCallback(RecvCallBack);
        return clientSocket;
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="host">ip地址</param>
    /// <param name="port">端口号</param>
    public void Connect(string host, int port)
    {
        if (m_socket == null)
            m_socket = init();
        try
        {
            Debug.Log("connect: " + host + ":" + port);
            m_socket.SendTimeout = 3;
            m_socket.Connect(host, port);
            connected = true;

        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendData(byte[] bytes)
    {
        NetworkStream netstream = new NetworkStream(m_socket);
        netstream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// 尝试接收消息（每帧调用）
    /// </summary>
    public void BeginReceive()
    {
        m_socket.BeginReceive(m_recvBuff, 0, m_recvBuff.Length, SocketFlags.None, m_recvCb, this);
    }

    /// <summary>
    /// 当收到服务器的消息时会回调这个函数
    /// </summary>
    private void RecvCallBack(IAsyncResult ar)
    {
        int len = m_socket.EndReceive(ar);
        if (len == 0)
            return;
        // 将消息塞入队列中S
        byte[] war = new byte[1024];
        Array.Copy(m_recvBuff,war,len);
        m_msgQueue.Enqueue(war);
        // 将buffer清零
        for (int i = 0; i < m_recvBuff.Length; ++i)
        {
            m_recvBuff[i] = 0;
        }
    }

    /// <summary>
    /// 从消息队列中取出消息
    /// </summary>
    /// <returns></returns>
    public bool GetMsgFromQueue()
    {
        if (m_msgQueue.Count > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 关闭Socket
    /// </summary>
    public void CloseSocket()
    {
        Debug.Log("close socket");
        try
        {
            m_socket.Shutdown(SocketShutdown.Both);
            m_socket.Close();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            m_socket = null;
            connected = false;
        }
    }


    public bool connected = false;

    private byte[] m_recvBuff;
    private AsyncCallback m_recvCb;
    public Queue<byte[]> m_msgQueue = new Queue<byte[]>();
    public byte[] nowbyte=new byte[1024];
    private Socket m_socket;
}
