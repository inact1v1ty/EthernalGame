using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Message
{
    public byte[] buffer;
    public int length;

    public Message(byte[] buffer, int length)
    {
        this.buffer = buffer;
        this.length = length;
    }
}
