using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport;
using UnityEngine;

/*
    This class in only available in the Server side.
    this class is for handling all tanks inputs and movement
*/
public class TankServerManager : MonoBehaviour
{
    private readonly int maxLives = 3;

    #region UnityFunctions
    public static TankServerManager Singleton { get; private set; }

    // Tank Movement
    [SerializeField] private float tankMoveSpeed = 10f;
    [SerializeField] private float tankRotateSpeed = 2.8f;

    // Tank tower rotation
    [SerializeField] private float towerRotationAngle = 45f;

    // Tank Grenade default speed;
    [SerializeField] private float grenadeSpeed = 30f;
    private float timeBetweenEachTFire = 0.5f;

    private float timeBetweenEachSend = 0.05f;
    private float nextSendTime;


    public Dictionary<byte, Rigidbody> TankRigidbodies;
    public Dictionary<byte, Vector3> PreRbPosition;
    public Dictionary<byte, Quaternion> PreRbRotation;
    public Dictionary<byte, float> NextSendTFireTime;

    public Dictionary<byte, int> CountDiePlayer;
    public int count = 0;


    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        TankRigidbodies = new Dictionary<byte, Rigidbody>();
        PreRbPosition = new Dictionary<byte, Vector3>();
        PreRbRotation = new Dictionary<byte, Quaternion>();
        NextSendTFireTime = new Dictionary<byte, float>();
        CountDiePlayer = new Dictionary<byte, int>();
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        registerToEvent(true);
    }

    private void Update()
    {
        if (IsSendable())
            SendTransformToAll();
    }

    /// <summary>
    /// có được giửi hay không
    /// </summary>
    /// <returns></returns>
    private bool IsSendable()
    {
        if (Time.time >= nextSendTime)
        {
            nextSendTime = Time.time + timeBetweenEachSend;
            return true;
        }

        return false;
    }

    private void SendTransformToAll()
    {
        foreach (byte id in TankRigidbodies.Keys)
        {
            if (PreRbPosition[id] != TankRigidbodies[id].position)
            {
                PreRbPosition[id] = TankRigidbodies[id].position;
                Server.Singleton.BroadCast(new NetTPosition(id, TankRigidbodies[id].position));
            }

            if (PreRbRotation[id].eulerAngles != TankRigidbodies[id].rotation.eulerAngles)
            {
                PreRbRotation[id] = TankRigidbodies[id].rotation;
                Server.Singleton.BroadCast(new NetTRotation(id, TankRigidbodies[id].transform.forward));
            }
        }
    }

    #endregion

    #region Server Events Handling Functions
    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.S_T_INPUT += OnServerReceivedTInputMessage;
            NetUtility.S_T_TOWER_INPUT += OnServerReceivedTTowerInputMessage;
            NetUtility.S_T_FIRE_INPUT += OnServerReceivedTFireInputMessage;

            TankSpawner.Singleton.OnNewTankAdded += OnNewTankAdded;
            TankSpawner.Singleton.OnTankRemoved += OnTankRemoved;

            NetUtility.S_GAMEOVER += OnServerReceviedGameOverMessage;

        }
        else
        {
            NetUtility.S_T_INPUT -= OnServerReceivedTInputMessage;
            NetUtility.S_T_TOWER_INPUT -= OnServerReceivedTTowerInputMessage;
            NetUtility.S_T_FIRE_INPUT -= OnServerReceivedTFireInputMessage;

            TankSpawner.Singleton.OnNewTankAdded -= OnNewTankAdded;
            TankSpawner.Singleton.OnTankRemoved -= OnTankRemoved;
            NetUtility.S_GAMEOVER -= OnServerReceviedGameOverMessage;
        }
    }

    /// <summary>
    /// xóa xe tăng khi chết
    /// </summary>
    /// <param name="removedTankID"></param>
    private void OnTankRemoved(byte removedTankID)
    {
        TankRigidbodies.Remove(removedTankID);
        PreRbPosition.Remove(removedTankID);
        PreRbRotation.Remove(removedTankID);
        NextSendTFireTime.Remove(removedTankID);
        /// moi lan chet se  - 1
        if (CountDiePlayer.ContainsKey(removedTankID))
        {
            CountDiePlayer[removedTankID]--;
        }
    }

    /// <summary>
    ///  khởi tạo xe tăng khi vào trận set vị trí id cho xe tăng
    /// </summary>
    /// <param name="addedTank"></param>
    private void OnNewTankAdded(GameObject addedTank)
    {
        TankInformation tankInformation = addedTank.GetComponent<TankInformation>();

        TankRigidbodies.Add(tankInformation.Player.ID, addedTank.GetComponent<Rigidbody>());
        PreRbPosition.Add(tankInformation.Player.ID, addedTank.transform.position);
        PreRbRotation.Add(tankInformation.Player.ID, addedTank.transform.rotation);
        NextSendTFireTime.Add(tankInformation.Player.ID, 0);

        if (!CountDiePlayer.ContainsKey(tankInformation.Player.ID))
        {
            CountDiePlayer.Add(tankInformation.Player.ID, maxLives);
        }
    }
    /// <summary>
    /// Nhận gói tin và sử lý viện đạn của xe tăng
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sentPlayer"></param>
    private void OnServerReceivedTFireInputMessage(NetMessage message, NetworkConnection sentPlayer)
    {
        NetTFireInput tFireInputMessage = message as NetTFireInput;
        if (!TankRigidbodies.ContainsKey(tFireInputMessage.ID)) return;


        float nextSendTFireTime;
        if (!NextSendTFireTime.TryGetValue(tFireInputMessage.ID, out nextSendTFireTime))
            NextSendTFireTime[tFireInputMessage.ID] = 0;

        nextSendTFireTime = NextSendTFireTime[tFireInputMessage.ID];
        if (Time.time >= nextSendTFireTime)
        {
            NextSendTFireTime[tFireInputMessage.ID] = Time.time + timeBetweenEachTFire;

            GameObject sentPlayerTankTower = TankRigidbodies[tFireInputMessage.ID].GetComponent<TankMovement>().TankTower;
            tFireInputMessage.FireDirection = sentPlayerTankTower.transform.forward;
            tFireInputMessage.Speed = grenadeSpeed;

            Server.Singleton.BroadCast(tFireInputMessage);
        }
    }

    private void OnServerReceivedTTowerInputMessage(NetMessage message, NetworkConnection sentPlayer)
    {
        NetTTowerInput tankTowerInputMessage = message as NetTTowerInput;
        if (!TankRigidbodies.ContainsKey(tankTowerInputMessage.ID)) return;

        GameObject sentPlayerTankTower = TankRigidbodies[tankTowerInputMessage.ID].GetComponent<TankMovement>().TankTower;

        sentPlayerTankTower.transform.localEulerAngles += tankTowerInputMessage.RotationInput * towerRotationAngle * Vector3.up;

        Server.Singleton.BroadCast(new NetTTowerRotation(tankTowerInputMessage.ID, sentPlayerTankTower.transform.localEulerAngles));
    }

    /// <summary>
    ///  nhận các gói tin từ clinet các nút di chuyển ,bắn, quay nòng súng
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sentPlayer"></param>
    private void OnServerReceivedTInputMessage(NetMessage message, NetworkConnection sentPlayer)
    {
        NetTInput tankInputMessage = message as NetTInput;
        if (!TankRigidbodies.ContainsKey(tankInputMessage.ID)) return;

        Server.Singleton.BroadCast(new NetTInput(tankInputMessage.ID, tankInputMessage.HorizontalInput, tankInputMessage.VerticalInput));

        if (!Mathf.Approximately(tankInputMessage.VerticalInput, 0f))
        {
            ConvertAndBroadCast(tankInputMessage);
        }
    }

    /*
        At this point, Server just received an movement input message from sentPlayer
        Server needs to calculate the sentPlayer position,rotation and send it back to all players

         Tại thời điểm này, Server vừa nhận được một thông báo đầu vào chuyển động từ sentPlayer
         Máy chủ cần tính toán vị trí Người chơi đã gửi, vòng quay và gửi lại cho tất cả người chơi
    */
    private void ConvertAndBroadCast(NetTInput tankInputMessage)
    {
        if (tankInputMessage.VerticalInput < 0)
        {
            tankInputMessage.HorizontalInput *= -1;
        }

        Rigidbody sentPlayerRigidbody = TankRigidbodies[tankInputMessage.ID];

        MoveSentPlayerRigidBodyBasedOnInput(ref sentPlayerRigidbody, tankInputMessage.HorizontalInput, tankInputMessage.VerticalInput);
        Server.Singleton.BroadCast(new NetTRotation(tankInputMessage.ID, sentPlayerRigidbody.transform.forward));
        Server.Singleton.BroadCast(new NetTVelocity(tankInputMessage.ID, sentPlayerRigidbody.velocity));
    }


    #endregion
    /// <summary>
    /// Tính toán các chức năng
    /// </summary>
    /// <param name="rb"></param>
    /// <param name="horizontalInput"></param>
    /// <param name="verticalInput"></param>

    #region Calculating Functions
    private void MoveSentPlayerRigidBodyBasedOnInput(ref Rigidbody rb, float horizontalInput, float verticalInput)
    {
        rb.velocity = (rb.transform.forward * verticalInput * tankMoveSpeed) + Vector3.up * rb.velocity.y;
        rb.transform.localEulerAngles += horizontalInput * tankRotateSpeed * Vector3.up;
    }
    #endregion
    #region Gameover
    /// <summary>
    /// ckeck xem map của id còn mạng không 
    /// </summary>
    /// <returns></returns>
    public bool CkeckSpawnablePlayer(byte id)
    {
        foreach (KeyValuePair<byte, int> a in CountDiePlayer)
        {
            if (a.Key == id&& a.Value>=0)
            {
                return true;
            }
        }
        return false;
    }
    private void OnServerReceviedGameOverMessage(NetMessage message, NetworkConnection sender)
    {
        NETTGameOver tankDieMessage = message as NETTGameOver;
        Debug.Log("so lan chet" + countDiePlayer(tankDieMessage));
        //Server.Singleton.BroadCast(new NETTGameOver(tankDieMessage.Team));
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
    #endregion
}
