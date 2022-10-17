using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;


public class NETTGameOver : NetMessage
{
    public Team Team { set; get; }
   

    public NETTGameOver(Team team)
    {
        Code = OpCode.T_GAMEOVER;
        Team = team;
       
    }

    public NETTGameOver(ref DataStreamReader reader)
    {
        Code = OpCode.T_GAMEOVER;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteInt((int)Team);
       

    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        Team = (Team)reader.ReadInt();
      
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_GAMEOVER?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_GAMEOVER?.Invoke(this, cnn);
    }
}


