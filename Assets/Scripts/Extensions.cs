using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class Extensions
{
    public static void Write(this BinaryWriter bw, Vector3 value)
    {
        bw.Write(value.x);
        bw.Write(value.y);
        bw.Write(value.z);
    }
    public static Vector3 ReadVector3(this BinaryReader br)
    {
        float x = br.ReadSingle();
        float y = br.ReadSingle();
        float z = br.ReadSingle();
        return new Vector3(x, y, z);
    }
}
