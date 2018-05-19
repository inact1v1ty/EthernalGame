using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{

    public InputField input;

    TcpClient tcpClient;

    public void Connect()
    {
        Networking.Instance.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7777));
        Networking.Instance.BeginReceive();
    }

    public void SendReliable()
    {
        Networking.Instance.SendReliable(Encoding.UTF8.GetBytes(input.text));
    }
    public void SendUnreliable()
    {
        Networking.Instance.SendUnReliable(Encoding.UTF8.GetBytes(input.text));
    }

    void Update()
    {
        while (Networking.Instance.queue.Count > 0)
        {
            Message msg;
            if (Networking.Instance.queue.TryDequeue(out msg))
            {
                Debug.Log(Encoding.UTF8.GetString(msg.buffer, 0, msg.length));
            }
        }
    }
}
