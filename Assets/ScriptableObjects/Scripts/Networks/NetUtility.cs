using System;
using Unity.Networking.Transport;
using UnityEngine;

public enum OpCode
{
    PING = 0,
    KEEP_ALIVE = 1,

    //For MainMenu + Lobby
    SEND_NAME,
    JOIN,
    DISCONNECT,
    WELCOME,
    READY,
    SWITCHTEAM,
    START,

    // For tanks input
    T_INPUT,
    T_TOWER_INPUT,
    T_FIRE_INPUT,

    // Game over
    T_GAMEOVER,
    // For tanks movement
    T_TOWER_ROTATION,
    T_TRANSFORM,
    T_VELOCITY,
    //vị trí 
    T_POSITION,
    //Hướng xoay 
    T_ROTATION,

    //Đạn xe tăng
    GRENADE_EXPLOSION,

    // For tank Interactions
    /// <summary>
    /// hồi sinh
    /// </summary>
    T_SPAWN,
    /// <summary>
    /// Yêu cầu hồi sinh
    /// </summary>
    T_SPAWN_REQ,
    /// <summary>
    /// Xe Tăng chết
    /// </summary>
    T_DIE,
    /// <summary>
    /// Kill
    /// </summary>
    T_KILL,

    // UI
    UI_SPAWN_COUNTDOWN,



    NONE
}

public static class NetUtility
{
    // Net Lobby events
    /// <summary>
    /// Giửi Ping
    /// </summary>
    public static Action<NetMessage> C_PING;
    /// <summary>
    /// gói đang ở trạng thái gì
    /// </summary>
    public static Action<NetMessage> C_KEEP_ALIVE;
    /// <summary>
    /// Tên của nhân vật
    /// </summary>
    public static Action<NetMessage> C_SEND_NAME;
    /// <summary>
    /// Gói tin thông báo có thêm người cào trong phong
    /// </summary>
    public static Action<NetMessage> C_JOIN;
    /// <summary>
    /// Gói tin xem đang làm chủ phòng hay là người thamg gia
    /// </summary>
    public static Action<NetMessage> C_WELCOME;
    /// <summary>
    /// Gói tin xóa người chơi ra khỏi phòng
    /// </summary>
    public static Action<NetMessage> C_DISCONNECT;
    public static Action<NetMessage> C_GAMEOVER;

    public static Action<NetMessage> C_READY;
    public static Action<NetMessage> C_SWITCHTEAM;
    public static Action<NetMessage> C_START;
    public static Action<NetMessage, NetworkConnection> S_PING;
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;

    public static Action<NetMessage, NetworkConnection> S_SEND_NAME { get; set; }
    public static Action<NetMessage, NetworkConnection> S_JOIN;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_DISCONNECT;
    public static Action<NetMessage, NetworkConnection> S_READY;
    public static Action<NetMessage, NetworkConnection> S_SWITCHTEAM;
    public static Action<NetMessage, NetworkConnection> S_START;
    public static Action<NetMessage, NetworkConnection> S_GAMEOVER;

    /// <summary>
    /// số phòng đang được tạo
    /// </summary>
    public static Action<NetMessage> C_ConnectRoom;

    // Net Tank input events
    //C= clinet , T = tank, S = Sever
    public static Action<NetMessage> C_T_INPUT;
    public static Action<NetMessage> C_T_TOWER_INPUT;
    public static Action<NetMessage> C_T_FIRE_INPUT { get; set; }
    public static Action<NetMessage, NetworkConnection> S_T_INPUT;
    public static Action<NetMessage, NetworkConnection> S_T_TOWER_INPUT;
    public static Action<NetMessage, NetworkConnection> S_T_FIRE_INPUT;

    // Net Tank movement events
    public static Action<NetMessage> C_T_TRANSFORM;
    public static Action<NetMessage> C_T_TOWER_ROTATION;
    public static Action<NetMessage> C_T_VELOCITY;
    public static Action<NetMessage> C_T_POSITION;
    public static Action<NetMessage> C_T_ROTATION;
    public static Action<NetMessage, NetworkConnection> S_T_TRANSFORM;
    public static Action<NetMessage, NetworkConnection> S_T_TOWER_ROTATION;
    public static Action<NetMessage, NetworkConnection> S_T_VELOCITY;
    public static Action<NetMessage, NetworkConnection> S_T_POSITION;
    public static Action<NetMessage, NetworkConnection> S_T_ROTATION;

    // Grenade events
    public static Action<NetMessage> C_GRENADE_EXPLOSION;
    public static Action<NetMessage, NetworkConnection> S_GRENADE_EXPLOSION;

    // Tank Interactions
    public static Action<NetMessage> C_T_DIE;
    public static Action<NetMessage> C_T_SPAWN_REQ;
    public static Action<NetMessage> C_T_SPAWN;
    public static Action<NetMessage> C_T_SPAWN_READY;
    public static Action<NetMessage> C_T_KILL { get; set; }
    public static Action<NetMessage, NetworkConnection> S_T_DIE;
    public static Action<NetMessage, NetworkConnection> S_T_SPAWN_REQ;
    public static Action<NetMessage, NetworkConnection> S_T_SPAWN;
    public static Action<NetMessage, NetworkConnection> S_T_KILL;

    // UI 
    public static Action<NetMessage> C_UI_SPAWN_COUNTDOWN;
    public static Action<NetMessage, NetworkConnection> S_UI_SPAWN_COUNTDOWN;

    public static void OnData(ref DataStreamReader streamReader, NetworkConnection cnn, Server server = null)
    {
        NetMessage msg = null;
        var OpCode = (OpCode)streamReader.ReadByte();
        switch (OpCode)
        {
            case OpCode.PING:
                msg = new NetPing(ref streamReader);
                break;

            case OpCode.KEEP_ALIVE:
                msg = new NetKeepAlive(ref streamReader);
                break;

            //Lobby
            case OpCode.SEND_NAME:
                msg = new NetSendName(ref streamReader);
                break;

            case OpCode.JOIN:
                msg = new NetJoin(ref streamReader);
                break;

            case OpCode.DISCONNECT:
                msg = new NetDisconnect(ref streamReader);
                break;

            case OpCode.WELCOME:
                msg = new NetWelcome(ref streamReader);
                break;

            case OpCode.READY:
                msg = new NetReady(ref streamReader);
                break;

            case OpCode.SWITCHTEAM:
                msg = new NetSwitchTeam(ref streamReader);
                break;

            case OpCode.START:
                msg = new NetStartGame(ref streamReader);
                break;

            //Tank Input
            case OpCode.T_INPUT:
                msg = new NetTInput(ref streamReader);
                break;

            case OpCode.T_TOWER_INPUT:
                msg = new NetTTowerInput(ref streamReader);
                break;

            case OpCode.T_FIRE_INPUT:
                msg = new NetTFireInput(ref streamReader);
                break;

            //Tank Movement
            case OpCode.T_TRANSFORM:
                msg = new NetTTransform(ref streamReader);
                break;

            case OpCode.T_TOWER_ROTATION:
                msg = new NetTTowerRotation(ref streamReader);
                break;

            case OpCode.T_VELOCITY:
                msg = new NetTVelocity(ref streamReader);
                break;

            case OpCode.T_POSITION:
                msg = new NetTPosition(ref streamReader);
                break;

            case OpCode.T_ROTATION:
                msg = new NetTRotation(ref streamReader);
                break;

            //Grenade
            case OpCode.GRENADE_EXPLOSION:
                msg = new NetGrenadeExplosion(ref streamReader);
                break;

            // Tank Interactions
            case OpCode.T_SPAWN_REQ:
                msg = new NetTSpawnReq(ref streamReader);
                break;

            case OpCode.T_SPAWN:
                msg = new NetTSpawn(ref streamReader);
                break;

            case OpCode.T_DIE:
                msg = new NetTDie(ref streamReader);
                break;

            case OpCode.T_KILL:
                msg = new NetTKill(ref streamReader);
                break;

            // UI
            case OpCode.UI_SPAWN_COUNTDOWN:
                msg = new NetUISpawnCountDown(ref streamReader);
                break;
            case OpCode.T_GAMEOVER:
                msg = new NETTGameOver(ref streamReader);
                break;
            default:
                Debug.LogError("Message received has no OpCode");
                break;
        }

        if (server != null)
            msg.ReceivedOnServer(cnn);
        else
            msg.ReceivedOnClient();
    }
}
