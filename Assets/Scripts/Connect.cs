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
    public InputField privateKeyField;

    public void FireConnect()
    {
        var ip = IPAddress.Parse(ipField.text);
        var port = int.Parse(portField.text);

        //EtheriumAccount.Instance.privateKey = privateKeyField.text;
        //EtheriumAccount.Instance.Init();
        //StartCoroutine (EtheriumAccount.Instance.getBlockNumber());
        //StartCoroutine (EtheriumAccount.Instance.payForCreation());

        Networking.Instance.Connect(new IPEndPoint(ip, port));
        Networking.Instance.BeginReceive();

        //NetManager.Instance.nickname = nicknameField.text;
        //NetManager.Instance.address = EtheriumAccount.Instance.address;

        SceneManager.LoadScene("Demo");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FireConnect();
        }
    }
}
