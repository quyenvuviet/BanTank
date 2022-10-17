using Unity.Networking.Transport;
using UnityEngine;

public class NetMessage
{
    public OpCode Code { set; get; }

    /// <summary>
    /// biến đổi dạng nhị phân
    /// </summary>
    /// <param name="writer"></param>
    public virtual void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }
    /// <summary>
    ///  giải mã
    /// </summary>
    /// <param name="reader"></param>
    public virtual void Deserialize(ref DataStreamReader reader) { }
    /// <summary>
    /// Nhận gói tin từ Clinet
    /// </summary>
    public virtual void ReceivedOnClient() { }
    /// <summary>
    /// Nhận gói tin từ Server
    /// </summary>
    /// <param name="cnn"></param>
    public virtual void ReceivedOnServer(NetworkConnection cnn) { }
}
