using Unity.Networking.Transport;

/// <summary>
/// Sử lý gói tên của người chơi
/// </summary>
public class NetSendName : NetMessage
{
    public string Name { set; get; }

    public NetSendName(string name)
    {
        Code = OpCode.SEND_NAME;
        Name = name;
    }

    public NetSendName(ref DataStreamReader reader)
    {
        Code = OpCode.SEND_NAME;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteFixedString32(Name);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        Name = reader.ReadFixedString32().ToString();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_SEND_NAME?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_SEND_NAME?.Invoke(this, cnn);
    }
}
