using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

/*
    Only host can have this 
*/
public class TankServerSpawner : MonoBehaviour
{
    private readonly int maxLives = 3;

    public static TankServerSpawner Singleton { get; private set; }
    public bool Spawnable { get; private set; }

    public SpawnPosition spawnPosition;
    public TankSpawnerData tankSpawnerData;
    private List<byte> isInCountDown;
    public Dictionary<byte, int> CountDiePlayer;

    private void Awake()
    {
        Singleton = this;
        isInCountDown = new List<byte>();
        CountDiePlayer = new Dictionary<byte, int>();
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        registerToEvent(true);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.S_T_SPAWN_REQ += OnServerReceivedTankSpawnRequestMessage;
            NetUtility.S_T_SPAWN += OnServerReceivedTankSpawnMessage;
            NetUtility.S_T_DIE += OnServerReceivedTDieMessage;
            NetUtility.S_T_KILL += OnServerReceivedTKillMessage;

        }
        else
        {
            NetUtility.S_T_SPAWN_REQ -= OnServerReceivedTankSpawnRequestMessage;
            NetUtility.S_T_SPAWN -= OnServerReceivedTankSpawnMessage;
            NetUtility.S_T_DIE -= OnServerReceivedTDieMessage;
            NetUtility.S_T_KILL -= OnServerReceivedTKillMessage;

        }
    }

    private void OnServerReceivedTKillMessage(NetMessage message, NetworkConnection sender)
    {
        Server.Singleton.BroadCast(message as NetTKill);
    }

    private void OnServerReceivedTankSpawnMessage(NetMessage message, NetworkConnection sender)
    {
        NetTSpawn tSpawnMessage = message as NetTSpawn;

        if (!CountDiePlayer.ContainsKey(tSpawnMessage.ID))
        {
            CountDiePlayer.Add(tSpawnMessage.ID, maxLives);
        }

        Server.Singleton.BroadCastExcept(tSpawnMessage, sender);
    }

    private void OnServerReceivedTankSpawnRequestMessage(NetMessage message, NetworkConnection sender)
    {
        NetTSpawnReq tSpawnReqMessage = message as NetTSpawnReq;

        Player sendPlayerInfo = PlayerManager.Singleton.GetPlayer(tSpawnReqMessage.ID);
        Vector3 newSpawnPosition = Vector3.zero;
        if (CkeckSpawnablePlayer(tSpawnReqMessage.ID))
        {
            newSpawnPosition = spawnPosition.GetPosition(sendPlayerInfo.Role).position;
        }
        Server.Singleton.SendToClient(sender, new NetTSpawnReq(tSpawnReqMessage.ID, newSpawnPosition));
    }

    private void OnServerReceivedTDieMessage(NetMessage message, NetworkConnection sender)
    {
        NetTDie tankDieMessage = message as NetTDie;
        Player player = PlayerManager.Singleton.GetPlayer(tankDieMessage.ID);
        float countDuration = tankSpawnerData.GetRespawnTime(player.Role);
        tankDieMessage.NextSpawnDuration = countDuration;
        if (CountDiePlayer.ContainsKey(tankDieMessage.ID))
        {
            CountDiePlayer[tankDieMessage.ID]--;
        }

        // if (TankServerManager.Singleton.CkeckSpawnablePlayer(tankDieMessage.ID))
        // {

        //     NETTGameOver tankTeamMessage = message as NETTGameOver;
        //     Server.Singleton.BroadCast(new NETTGameOver(tankTeamMessage.Team));
        // }
        if (aWholeTeamDie())

            // At this point a tank just die so send the dead message to all player
            // Which contains the death time, use this to show UI
            Server.Singleton.BroadCast(tankDieMessage);

        // after a countDuration send a spawn message to the death player
        HandleSpawnTank(tankDieMessage.ID, player, countDuration, sender);
    }

    private void HandleSpawnTank(byte id, Player player, float countDuration, NetworkConnection sender)
    {
        if (!isInCountDown.Contains(id))
        {
            StartCoroutine(SendSpawnMessageCountDown(id, player, countDuration, sender));
        }
    }

    /// <summary>
    /// This function will send a NetTSpawnReq back to client after a countDown which based on the RoleController
    /// đếm ngược
    /// </summary>
    private IEnumerator SendSpawnMessageCountDown(byte id, Player player, float countDuration, NetworkConnection sender)
    {
        isInCountDown.Add(id);

        while (countDuration > 0)
        {
            countDuration -= Time.deltaTime;
            yield return null;
        }

        isInCountDown.Remove(id);

        Server.Singleton.SendToClient(sender, new NetTSpawnReady(id));

        //new message
    }

    /// <summary>
    /// ckeck xem map của id còn mạng không 
    /// </summary>
    /// <returns></returns>
    public bool CkeckSpawnablePlayer(byte id)
    {
        foreach (KeyValuePair<byte, int> a in CountDiePlayer)
        {
            if (a.Key == id && a.Value > 0)
            {
                return true;
            }
        }
        return false;
    }

    private int countDiePlayer(NETTGameOver tankDieMessage)
    {
        foreach (KeyValuePair<byte, int> a in CountDiePlayer)
        {
            if (a.Key == PlayerManager.Singleton.GetIDToTeam(tankDieMessage.Team))
            {
                return a.Value;
            }
        }
        return 0;
    }
}
