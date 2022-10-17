using System;
using System.Collections.Generic;
using Unity.Networking.Transport;

/// <summary>
/// Enum Team
/// </summary>
public enum Team
{
    /// <summary>
    /// Team mầu xanh
    /// </summary>
    Blue = 0,
    /// <summary>
    /// Team mầu đỏ
    /// </summary>
    Red = 1
}
/// <summary>
/// Trạng thái sẵn sàng
/// </summary>
public enum ReadyState
{
    //sẵn sàng
    Ready = 0,
    //chư sẵn sàng
    Unready = 1
}

/// <summary>
/// Lớp này dùng để lưu trữ thông tin người chơi của một vị trí cụ thể
/// </summary>
public class SlotPlayerInformation
{
    public byte Id;
    public Team Team;
    public byte SlotIndex;
    public string Name;
    public ReadyState ReadyState;

    /// <summary>
    /// Khởi tạo mặc định trước khi vào
    /// </summary>
    public SlotPlayerInformation()
    {
        Id = 0;
        Team = 0;
        SlotIndex = 0;
        Name = "";
        ReadyState = ReadyState.Unready;
    }
    /// <summary>
    /// Khởi tạo nhân vật
    /// </summary>
    /// <param name="id"></param>
    /// <param name="team"></param>
    /// <param name="slotIndex"></param>
    /// <param name="name"></param>
    /// <param name="readyState"></param>
    public SlotPlayerInformation(byte id, Team team, byte slotIndex, string name, ReadyState readyState)
    {
        Id = id;
        Team = team;
        SlotIndex = slotIndex;
        Name = name;
        ReadyState = readyState;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="slotPlayer"></param>
    public static void SerializeSlotPlayer(ref DataStreamWriter writer, SlotPlayerInformation slotPlayer)
    {
        writer.WriteByte(slotPlayer.Id);
        writer.WriteByte((byte)slotPlayer.Team);
        writer.WriteByte(slotPlayer.SlotIndex);
        writer.WriteFixedString32(slotPlayer.Name);
        writer.WriteByte((byte)slotPlayer.ReadyState);
    }

    public static SlotPlayerInformation DeserializeSlotPlayer(ref DataStreamReader reader)
    {
        byte playerId = reader.ReadByte();
        Team playerTeam = (Team)reader.ReadByte();
        byte playerSlotIndex = reader.ReadByte();
        string playerName = reader.ReadFixedString32().ToString();
        ReadyState readyState = (ReadyState)reader.ReadByte();

        return new SlotPlayerInformation(playerId, playerTeam, playerSlotIndex, playerName, readyState);
    }

    public static SlotPlayerInformation FindSlotPlayerWithID(List<SlotPlayerInformation> playerList, byte playerId)
    {
        foreach (SlotPlayerInformation player in playerList)
        {
            if (player.Id == playerId) return player;
        }
        return null;
    }

    public static SlotPlayerInformation FindSlotPlayerWithIDAndRemove(ref List<SlotPlayerInformation> playerList, byte playerId)
    {
        foreach (SlotPlayerInformation player in playerList)
        {
            if (player.Id == playerId)
            {
                playerList.Remove(player);
                return player;
            }
        }
        return null;
    }
    public static int FindSlotPlayerWithIDToTeam(ref List<SlotPlayerInformation> playerList, byte playerId)
    {
        foreach (SlotPlayerInformation player in playerList)
        {
            if (player.Id == playerId && player.Team == Team.Red)
            {
                Console.WriteLine("ID" + player.Id + "Team" + Team.Red);

                return (int)Team.Red;
            }
        }
        return (int)Team.Blue;
    }

    public static bool HaveAllPlayersReadied(List<SlotPlayerInformation> players)
    {
        foreach (SlotPlayerInformation player in players)
        {
            if (player.ReadyState == ReadyState.Unready)
                return false;
        }
        return true;
    }

    public static bool Have2TeamsEqual(List<SlotPlayerInformation> players)
    {
        return MathF.Abs(CountTeamPlayer(players, Team.Blue) - CountTeamPlayer(players, Team.Red)) <= 1;
    }

    public static int CountTeamPlayer(List<SlotPlayerInformation> players, Team team)
    {
        int counter = 0;
        foreach (SlotPlayerInformation player in players)
        {
            if (player.Team == team)
                counter++;
        }
        return counter;
    }
    /// <summary>
    /// chuyển nhóm
    /// </summary>
    public void SwitchTeam()
    {
        Team = Team == Team.Blue ? Team.Red : Team.Blue;
    }
    /// <summary>
    /// xem trạng thái của người chơi đã sãn sằng hay chưa
    /// </summary>
    public void SwitchReadyState()
    {
        ReadyState = ReadyState == ReadyState.Ready ? ReadyState.Unready : ReadyState.Ready;
    }

    public bool IsHost => Id == GameInformation.Singleton.HostId;
}
