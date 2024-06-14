using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SChangeScene, MakePacket<S_ChangeScene>);
		_handler.Add((ushort)MsgId.SChangeScene, PacketHandler.S_ChangeSceneHandler);		
		_onRecv.Add((ushort)MsgId.SEnterRoom, MakePacket<S_EnterRoom>);
		_handler.Add((ushort)MsgId.SEnterRoom, PacketHandler.S_EnterRoomHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveRoom, MakePacket<S_LeaveRoom>);
		_handler.Add((ushort)MsgId.SLeaveRoom, PacketHandler.S_LeaveRoomHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.SConnect, MakePacket<S_Connect>);
		_handler.Add((ushort)MsgId.SConnect, PacketHandler.S_ConnectHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SSyncPosAndRot, MakePacket<S_SyncPosAndRot>);
		_handler.Add((ushort)MsgId.SSyncPosAndRot, PacketHandler.S_SyncPosAndRotHandler);		
		_onRecv.Add((ushort)MsgId.SSkill, MakePacket<S_Skill>);
		_handler.Add((ushort)MsgId.SSkill, PacketHandler.S_SkillHandler);		
		_onRecv.Add((ushort)MsgId.SChangeBaseStat, MakePacket<S_ChangeBaseStat>);
		_handler.Add((ushort)MsgId.SChangeBaseStat, PacketHandler.S_ChangeBaseStatHandler);		
		_onRecv.Add((ushort)MsgId.SChangeAdditionalStat, MakePacket<S_ChangeAdditionalStat>);
		_handler.Add((ushort)MsgId.SChangeAdditionalStat, PacketHandler.S_ChangeAdditionalStatHandler);		
		_onRecv.Add((ushort)MsgId.SLevelUp, MakePacket<S_LevelUp>);
		_handler.Add((ushort)MsgId.SLevelUp, PacketHandler.S_LevelUpHandler);		
		_onRecv.Add((ushort)MsgId.SChangeState, MakePacket<S_ChangeState>);
		_handler.Add((ushort)MsgId.SChangeState, PacketHandler.S_ChangeStateHandler);		
		_onRecv.Add((ushort)MsgId.SStop, MakePacket<S_Stop>);
		_handler.Add((ushort)MsgId.SStop, PacketHandler.S_StopHandler);		
		_onRecv.Add((ushort)MsgId.SPlayerDie, MakePacket<S_PlayerDie>);
		_handler.Add((ushort)MsgId.SPlayerDie, PacketHandler.S_PlayerDieHandler);		
		_onRecv.Add((ushort)MsgId.SRespawn, MakePacket<S_Respawn>);
		_handler.Add((ushort)MsgId.SRespawn, PacketHandler.S_RespawnHandler);		
		_onRecv.Add((ushort)MsgId.SCheckAttackRangeReq, MakePacket<S_CheckAttackRangeReq>);
		_handler.Add((ushort)MsgId.SCheckAttackRangeReq, PacketHandler.S_CheckAttackRangeReqHandler);		
		_onRecv.Add((ushort)MsgId.SChat, MakePacket<S_Chat>);
		_handler.Add((ushort)MsgId.SChat, PacketHandler.S_ChatHandler);		
		_onRecv.Add((ushort)MsgId.SAddItem, MakePacket<S_AddItem>);
		_handler.Add((ushort)MsgId.SAddItem, PacketHandler.S_AddItemHandler);		
		_onRecv.Add((ushort)MsgId.SAddMoney, MakePacket<S_AddMoney>);
		_handler.Add((ushort)MsgId.SAddMoney, PacketHandler.S_AddMoneyHandler);		
		_onRecv.Add((ushort)MsgId.SDamage, MakePacket<S_Damage>);
		_handler.Add((ushort)MsgId.SDamage, PacketHandler.S_DamageHandler);		
		_onRecv.Add((ushort)MsgId.SAddSkill, MakePacket<S_AddSkill>);
		_handler.Add((ushort)MsgId.SAddSkill, PacketHandler.S_AddSkillHandler);		
		_onRecv.Add((ushort)MsgId.SEquip, MakePacket<S_Equip>);
		_handler.Add((ushort)MsgId.SEquip, PacketHandler.S_EquipHandler);		
		_onRecv.Add((ushort)MsgId.SMyPartyInfo, MakePacket<S_MyPartyInfo>);
		_handler.Add((ushort)MsgId.SMyPartyInfo, PacketHandler.S_MyPartyInfoHandler);		
		_onRecv.Add((ushort)MsgId.SPartiesInfo, MakePacket<S_PartiesInfo>);
		_handler.Add((ushort)MsgId.SPartiesInfo, PacketHandler.S_PartiesInfoHandler);		
		_onRecv.Add((ushort)MsgId.SPartyWarning, MakePacket<S_PartyWarning>);
		_handler.Add((ushort)MsgId.SPartyWarning, PacketHandler.S_PartyWarningHandler);		
		_onRecv.Add((ushort)MsgId.SChangeSkillState, MakePacket<S_ChangeSkillState>);
		_handler.Add((ushort)MsgId.SChangeSkillState, PacketHandler.S_ChangeSkillStateHandler);		
		_onRecv.Add((ushort)MsgId.SChangeTarget, MakePacket<S_ChangeTarget>);
		_handler.Add((ushort)MsgId.SChangeTarget, PacketHandler.S_ChangeTargetHandler);		
		_onRecv.Add((ushort)MsgId.SCreateMeteo, MakePacket<S_CreateMeteo>);
		_handler.Add((ushort)MsgId.SCreateMeteo, PacketHandler.S_CreateMeteoHandler);		
		_onRecv.Add((ushort)MsgId.SPlayerAttacked, MakePacket<S_PlayerAttacked>);
		_handler.Add((ushort)MsgId.SPlayerAttacked, PacketHandler.S_PlayerAttackedHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		if (CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}