using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

/// <summary>
/// Vị trí hồi sinh
/// </summary>
public class NetTSpawnReq : NetMessage
{
    /// <summary>
    /// Id của player
    /// </summary>
    public byte ID { get; set; }
    /// <summary>
    /// vị trí hồi sinh
    /// </summary>
    public Vector3 Position { get; set; }
    /// <summary>
    /// ví trí hồi sinh
    /// </summary>
    private bool HasSpawnPostion;

    public NetTSpawnReq(byte id, Vector3? position = null)
    {
        Code = OpCode.T_SPAWN_REQ;
        ID = id;

        HasSpawnPostion = position != null;
        if (HasSpawnPostion)
            Position = position.Value;
    }

    public NetTSpawnReq(ref DataStreamReader reader)
    {
        Code = OpCode.T_SPAWN_REQ;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(ID);
        writer.WriteFixedString32(HasSpawnPostion.ToString());

        if (HasSpawnPostion)
        {
            writer.WriteFloat(Position.x);
            writer.WriteFloat(Position.y);
            writer.WriteFloat(Position.z);
        }
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        ID = reader.ReadByte();

        string trueValue = true.ToString();
        HasSpawnPostion = reader.ReadFixedString32().ToString() == trueValue;

        if (HasSpawnPostion)
        {
            float x = reader.ReadFloat();
            float y = reader.ReadFloat();
            float z = reader.ReadFloat();

            Position = new Vector3(x, y, z);
        }
    }


    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_T_SPAWN_REQ?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_T_SPAWN_REQ?.Invoke(this, cnn);
    }
}
