using System;
using TMPro;
using Unity.Networking.Transport;
using UnityEngine;


/// <summary>
/// Clinet 
/// </summary>
public class Client : MonoBehaviour
{
    /// <summary>
    /// text ping
    /// </summary>
    [SerializeField] private TMP_Text pingCounterText;
    /// <summary>
    /// 
    /// </summary>
    private NetworkDriver driver;
    private NetworkConnection connection;

    private bool isActive = false;
    private bool isClientShutDown = false;
    private string playerName;
    private float timeBetweenEachPingSend = 1f;
    private float nextPingSend;
    private float preSendPingTime;

    public static Client Singleton { get; private set; }

    public Action OnClientDisconnect;
    public Action OnServerDisconnect;

    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Khởi tạo trước khi vào game
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="inputName"></param>
    public void Init(string ip, ushort port, string inputName)
    {
        if (isActive)
        {
            ClientReset();
        }

        driver = NetworkDriver.Create();
        // tạo địa chỉ IP vs cả port
        NetworkEndPoint endPoint = NetworkEndPoint.Parse(ip, port);

        connection = driver.Connect(endPoint);

        Debug.Log($"Attemping to connect to Server on {endPoint.Address}");

        isActive = true;
        isClientShutDown = false;

        RegisterToEvent();

        playerName = inputName != "" ? inputName : "I forgot to name myself";
        nextPingSend = Time.time + timeBetweenEachPingSend;
    }

    private void OnDestroy()
    {
        if (isActive)
            ClientReset();
    }

    private void ClientReset()
    {
        driver.Dispose();
        connection = default(NetworkConnection);
        isActive = false;
        UnregisterToEvent();
        isClientShutDown = true;
    }

    public void Shutdown()
    {
        if (isActive)
        {
            connection.Disconnect(driver);
            OnClientDisconnect?.Invoke();
        }
    }

    public void Update()
    {
        if (!isActive) return;

        driver.ScheduleUpdate().Complete();
        CheckAlive();
        SendClientPing();
        UpdateMessagePump();

        if (isClientShutDown)
            ClientReset();
    }

    private void SendClientPing()
    {
        if (Time.time >= nextPingSend)
        {
            nextPingSend = Time.time + timeBetweenEachPingSend;
            preSendPingTime = Time.time;
            SendToServer(new NetPing());
        }
    }

    private void CheckAlive()
    {
        if (!connection.IsCreated && isActive)
        {
            Debug.Log("Something went wrong, lost connection to server!");
            Shutdown();
        }
    }

    private void UpdateMessagePump()
    {
        NetworkEvent.Type cmd;

        while ((cmd = connection.PopEvent(driver, out DataStreamReader streamReader)) != NetworkEvent.Type.Empty)
        {
            switch (cmd)
            {
                case NetworkEvent.Type.Connect:
                    SendToServer(new NetSendName(playerName));
                    break;

                case NetworkEvent.Type.Data:
                    NetUtility.OnData(ref streamReader, default(NetworkConnection));
                    break;

                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Server has been shutdown!!");
                    OnServerDisconnect?.Invoke();
                    Shutdown();
                    break;
            }
        }
    }
    /// <summary>
    ///  Gửi gói tin về server 
    /// </summary>
    /// <param name="msg"></param>
    public void SendToServer(NetMessage msg)
    {
        driver.BeginSend(connection, out DataStreamWriter writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    #region Network Received
    /// <summary>
    ///  Sự kiện kiểm tra Ping vs cả trạng thái của người chơi đang tử vong hay sống giửi gói tin về server , thông báo cho người chơi khác biết
    /// </summary>
    private void RegisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE += OnClientReceivedKeepAliveMessage;
        NetUtility.C_PING += OnClientReceivedPingMessage;
    }
    /// <summary>
    /// Chết 
    /// </summary>
    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= OnClientReceivedKeepAliveMessage;
        NetUtility.C_PING -= OnClientReceivedPingMessage;
    }
    /// <summary>
    /// gửi ổn định về từ Server về
    /// </summary>
    /// <param name="message"></param>
    private void OnClientReceivedPingMessage(NetMessage message)
    {
        float deltaTime = (Time.time - preSendPingTime);
        deltaTime = Mathf.Round(deltaTime * 1000); // Multiplied by 1000 to get ms
        pingCounterText.SetText(StringUti.Format(StringUti.Singleton.PingCounter, deltaTime));
    }
    /// <summary>
    /// giửi lại để xem nhân vật cong sống hay không
    /// </summary>
    /// <param name="keepAliveMessage"></param>
    private void OnClientReceivedKeepAliveMessage(NetMessage keepAliveMessage)
    {
        // Send it back, to keep both side alive
        SendToServer(keepAliveMessage);
    }
    #endregion
}
