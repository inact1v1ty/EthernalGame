using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Connect : MonoBehaviour
{
    public InputField ipField;
    public InputField portField;
    public InputField nicknameField;

    public void FireConnect()
    {
        var ip = IPAddress.Parse(ipField.text);
        var port = int.Parse(portField.text);
        Networking.Instance.Connect(new IPEndPoint(ip, port));
        Networking.Instance.BeginReceive();

        NetManager.Instance.nickname = nicknameField.text;

        SceneManager.LoadScene("Game");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FireConnect();
        }
    }
}
