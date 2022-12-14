using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;


/// <summary>
///  Gói tin quản lý người chơi ra vào phòng 
/// </summary>
public class NetTSpawnReady : NetMessage
{
    public byte ID { set; get; }

    public NetTSpawnReady(byte id )
    {
        Code = OpCode.T_SPAWN_READY;
        ID = id;
    }

    public NetTSpawnReady(ref DataStreamReader reader)
    {
        Code = OpCode.T_SPAWN_READY;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(ID);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        ID = reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_T_SPAWN_READY?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_T_SPAWN_READY?.Invoke(this, cnn);
    }
}
