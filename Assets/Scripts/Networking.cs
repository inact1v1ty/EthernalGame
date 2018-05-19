using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using System;

public class Networking : Singleton<Networking>
{

    TcpClient tcpClient;
    UdpClient udpClient;

    byte[] buffer = new byte[1024];

    public ConcurrentQueue<Message> queue = new ConcurrentQueue<Message>();

    protected Networking() { }

    public void Init()
    {
        tcpClient = new TcpClient();
        udpClient = new UdpClient((tcpClient.Client.LocalEndPoint as IPEndPoint).Port);
    }

    void Start()
    {
        Init();
    }

    public void Connect(IPEndPoint endPoint)
    {
        tcpClient.Connect(endPoint);
        udpClient.Connect(endPoint);
    }

    public void BeginReceive()
    {
        tcpClient.Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnTcpReceived, null);
        udpClient.BeginReceive(OnUdpReceived, null);
    }

    void OnTcpReceived(IAsyncResult ar)
    {
        try
        {
            int read = tcpClient.Client.EndReceive(ar);

            if (read == 0)
                return;

            queue.Enqueue(new Message(buffer, read));

            tcpClient.Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnTcpReceived, null);
        }
        catch (System.Exception)
        {
            tcpClient.Close();
        }
    }

    public void SendReliable(byte[] data)
    {
        tcpClient.Client.BeginSend(data, 0, data.Length, SocketFlags.None, OnSendTcp, null);
    }

    private void OnSendTcp(IAsyncResult ar)
    {
        tcpClient.Client.EndSend(ar);
    }

    public void SendUnReliable(byte[] data)
    {
        udpClient.BeginSend(data, data.Length, OnSendUdp, null);
    }

    private void OnSendUdp(IAsyncResult ar)
    {
        udpClient.EndSend(ar);
    }

    void OnUdpReceived(IAsyncResult ar)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, (udpClient.Client.LocalEndPoint as IPEndPoint).Port);
        byte[] data = udpClient.EndReceive(ar, ref endPoint);

        queue.Enqueue(new Message(data, data.Length));

        udpClient.BeginReceive(OnUdpReceived, null);
    }
}
