using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

/// <summary>
/// vị trí của xe tăng trong không gian
/// </summary>
public class NetTVelocity : NetMessage
{
    /// <summary>
    /// ID của người chơi
    /// </summary>
    public byte ID { get; set; }
    /// <summary>
    /// Vị trí người chơi trong game
    /// </summary>
    public Vector3 Velocity { get; set; }

    /// <summary>
    /// Khởi tạo sẽ nhận ID và vị trí của người chơi
    /// </summary>
    /// <param name="id"></param>
    /// <param name="velocity"></param>
    public NetTVelocity(byte id, Vector3 velocity)
    {
        Code = OpCode.T_VELOCITY;
        ID = id;
        Velocity = velocity;
    }
    /// <summary>
    /// Đọc gói tin
    /// </summary>
    /// <param name="reader"></param>
    public NetTVelocity(ref DataStreamReader reader)
    {
        Code = OpCode.T_VELOCITY;
        Deserialize(ref reader);
    }
    /// <summary>
    /// Viết gói tin
    /// </summary>
    /// <param name="writer"></param>
    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(ID);
        writer.WriteFloat(Velocity.x);
        writer.WriteFloat(Velocity.y);
        writer.WriteFloat(Velocity.z);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        ID = reader.ReadByte();
        float x = reader.ReadFloat();
        float y = reader.ReadFloat();
        float z = reader.ReadFloat();
        Velocity = new Vector3(x, y, z);
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_T_VELOCITY?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_T_VELOCITY?.Invoke(this, cnn);
    }
}