using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetPlayer : MonoBehaviour
{

    public TextMeshPro text;
    public Transform textTransform;
    public int id;
    string nickname;
    public string Nickname
    {
        get
        {
            return nickname;
        }
        set
        {
            nickname = value;
            text.text = value;
        }
    }

    public void Update()
    {
        textTransform.LookAt(Camera.main.transform);
    }

    public void UpdatePosition(Vector3 position)
    {
        transform.position = position;
    }
}
