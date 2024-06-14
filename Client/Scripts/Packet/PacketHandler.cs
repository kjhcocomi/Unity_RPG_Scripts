using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
    public static void S_ChangeSceneHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_ChangeScene changeScenePacket = packet as S_ChangeScene;

        RoomType roomType = changeScenePacket.RoomType;

        switch (roomType)
        {
            case RoomType.Village:
                Managers.Scene.LoadScene(Define.Scene.Village);
                break;
            case RoomType.Boss:
                Managers.Scene.LoadScene(Define.Scene.Boss);
                break;
        }
    }

    public static void S_EnterRoomHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_EnterRoom enterRoomPacket = packet as S_EnterRoom;

        // 씬 입장, 플레이어 생성
        ObjectInfo objectInfo = enterRoomPacket.Player;
        Managers.Object.Add(objectInfo, true);

        UI_Hud hud = Managers.UI.SceneUI as UI_Hud;
        if(hud != null)
        {
            hud.SetInfo(objectInfo);
        }
        Managers.Data.BaseStat = objectInfo.BaseStat;
        Managers.Data.AdditionalStat = objectInfo.AdditionalStat;
    }

    public static void S_LeaveRoomHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_LeaveRoom leaveRoomPacket = packet as S_LeaveRoom;

        Managers.Object.Clear();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_Spawn spawnPacket = packet as S_Spawn;

        foreach (ObjectInfo obj in spawnPacket.Objects)
        {
            Managers.Object.Add(obj, false);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_Despawn despawnPacket = packet as S_Despawn;

        foreach (int id in despawnPacket.ObjectIds)
        {
            Managers.Object.Remove(id);
        }
    }

    public static void S_ConnectHandler(PacketSession session, IMessage packet)
    {
        LobbyScene lobbyScene = Managers.Scene.CurrentScene as LobbyScene;
        if (lobbyScene)
        {
            UI_Lobby ui_Lobby = Managers.UI.SceneUI as UI_Lobby;
            if (ui_Lobby)
            {
                ui_Lobby.SetConnectText(true);
            }
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_Move movePacket = packet as S_Move;

        GameObject go = Managers.Object.FindById(movePacket.ObjectId);
        if(go != null)
        {
            CreatureMovement movement = go.GetComponent<CreatureMovement>();
            movement.SetDirectionAndLookRotation(movePacket.TargetPos, movePacket.LookRotation);
        }
    }

    public static void S_SyncPosAndRotHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_SyncPosAndRot syncPacket = packet as S_SyncPosAndRot;

        foreach(SyncInfo info in syncPacket.SyncInfo)
        {
            int id = info.ObjectId;
            PositionInfo posInfo = info.PosInfo;
            PQuaternion rotInfo = info.RotInfo;

            GameObject go = Managers.Object.FindById(id);
            if (go != null)
            {
                go.transform.position = new Vector3(posInfo.PosX, posInfo.PosY, posInfo.PosZ);
                go.transform.rotation = new Quaternion(rotInfo.X, rotInfo.Y, rotInfo.Z, rotInfo.W);
            }
        }
    }
    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_Skill sSkillPacket = packet as S_Skill;

        GameObject go = Managers.Object.FindById(sSkillPacket.CasterId);
        if(go != null)
        {
            go.GetComponent<CreatureController>().CurrentSkill = sSkillPacket.SkillId;
        }
    }

    public static void S_ChangeBaseStatHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_ChangeBaseStat changeBaseStatPacket = packet as S_ChangeBaseStat;

        {
            CreatureController cc = Managers.Object.FindById(changeBaseStatPacket.ObjectId).GetComponent<CreatureController>();  
            cc.ObjectInfo.BaseStat = changeBaseStatPacket.Stat;
            DragonController dc = cc as DragonController;
            if(dc)
            {
                dc.RefreshBossUI();
            }
        }

        if (changeBaseStatPacket.ObjectId == Managers.Object.MyPlayer.Id)
        {
            Managers.Data.BaseStat = changeBaseStatPacket.Stat;
        }
        else
        {
            GameObject go = Managers.Object.FindById(changeBaseStatPacket.ObjectId);
            UI_NameHPBar ui_NameHPBar = go.GetComponentInChildren<UI_NameHPBar>();
            if (ui_NameHPBar)
            {
                ui_NameHPBar.SetHp(changeBaseStatPacket.Stat);
            }
        }
        bool isPartyMember = Managers.Party.IsPartyMember(changeBaseStatPacket.ObjectId);
        if(isPartyMember)
        {
            UI_Hud hud = Managers.UI.SceneUI as UI_Hud;
            if (hud)
            {
                hud.RefreshPartyMemberHp(changeBaseStatPacket.ObjectId);
            }
        }
    }

    public static void S_ChangeAdditionalStatHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_ChangeAdditionalStat changeAdditionalStatPacket = packet as S_ChangeAdditionalStat;

        if (changeAdditionalStatPacket.ObjectId == Managers.Object.MyPlayer.Id)
        {
            Managers.Data.AdditionalStat = changeAdditionalStatPacket.Stat;
        }
        else
        {

        }
    }

    public static void S_LevelUpHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_LevelUp levelUpPacket = packet as S_LevelUp;

        GameObject go = Managers.Object.FindById(levelUpPacket.ObjectId);

        if(levelUpPacket.ObjectId == Managers.Object.MyPlayer.Id)
        {
            Managers.Data.Level = levelUpPacket.Level;
        }
        GameObject effect = Managers.Resource.Instantiate("Effect/LevelUp", go.transform);
        effect.transform.position = go.transform.position;
        go.GetComponent<PlayerController>().PlayLevelUpSound();
    }

    public static void S_ChangeStateHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_ChangeState changeStatePacket = packet as S_ChangeState;

        GameObject go = Managers.Object.FindById(changeStatePacket.ObjectId);
        if (go != null)
        {
            CreatureController controller = go.GetComponent<CreatureController>();
            if (controller)
            {
                controller.creatureState = changeStatePacket.CreatureState;
            }
        }
    }

    public static void S_StopHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_Stop stopPacket = packet as S_Stop;

        GameObject go = Managers.Object.FindById(stopPacket.ObjectId);
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if(cc)
            {
                cc.Movement.ServerWorldDirVector = Vector3.zero;
                cc.creatureState = CreatureState.Idle;
            }
        }
    }

    public static void S_PlayerDieHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_PlayerDie playerDiePacket = packet as S_PlayerDie;

        GameObject go = Managers.Object.FindById(playerDiePacket.ObjectId);
        if (go != null)
        {
            PlayerController pc = go.GetComponent<PlayerController>();
            if(pc)
            {
                pc.Die();
            }
        }
    }

    public static void S_RespawnHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_Respawn respawnPacket = packet as S_Respawn;

        GameObject go = Managers.Object.FindById(respawnPacket.ObjectId);
        if (go != null)
        {
            PlayerController pc = go.GetComponent<PlayerController>();
            if (pc)
            {
                pc.Respawn(respawnPacket.BaseState, respawnPacket.PosInfo);
                bool isPartyMember = Managers.Party.IsPartyMember(respawnPacket.ObjectId);
                if (isPartyMember)
                {
                    UI_Hud hud = Managers.UI.SceneUI as UI_Hud;
                    if (hud)
                    {
                        hud.RefreshPartyMemberHp(respawnPacket.ObjectId);
                    }
                }
            }
        }
    }

    public static void S_CheckAttackRangeReqHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_CheckAttackRangeReq checkAttackRangeReqPacket = packet as S_CheckAttackRangeReq;

        if(Managers.Object.MyPlayer.Id == checkAttackRangeReqPacket.ObjectId)
        {
            Managers.Object.MyPlayer.CheckSkillRange(checkAttackRangeReqPacket.SkillId);
        }
    }

    public static void S_ChatHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_Chat sChatPacket = packet as S_Chat;

        GameObject go = Managers.Object.FindById(sChatPacket.ObjectId);
        if (go)
        {
            UI_Hud hud = Managers.UI.SceneUI as UI_Hud;
            if (hud)
            {
                hud.MakeChatElement(sChatPacket.Chat, sChatPacket.ChatType, go.name);
            }
            UI_SpeechBubble speechBubble = go.GetComponentInChildren<UI_SpeechBubble>();
            if (speechBubble)
            {
                speechBubble.ShowText(sChatPacket.Chat, sChatPacket.ChatType);
            }
        }
    }
    public static void S_AddItemHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_AddItem addItemPacket = packet as S_AddItem;

        Managers.Data.AddItem(addItemPacket.ItemId, addItemPacket.Count);
    }
    public static void S_AddMoneyHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_AddMoney addMoneyPacket = packet as S_AddMoney;

        Managers.Data.Money += addMoneyPacket.Money;
    }
    public static void S_DamageHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_Damage damagePacket = packet as S_Damage;

        GameObject go = Managers.Object.FindById(damagePacket.ObjectId);
        GameObject ui = Managers.Resource.Instantiate("UI/UI_Damage");
        GameObjectType objType = ObjectManager.GetObjectTypeById(damagePacket.ObjectId);
        if(objType == GameObjectType.Monster)
        {
            ui.transform.position = go.transform.position + Vector3.up * 1.7f;
        }
        else if(objType == GameObjectType.MonsterBoss)
        {
            ui.transform.position = go.transform.position + Vector3.up * 4f;
            go.GetComponent<DragonController>().PlayHittedEffect();
        }
        UI_Damage damageUI = ui.GetComponent<UI_Damage>();
        if(damageUI)
        {
            damageUI.Init();
            damageUI.SetDamage(damagePacket.Damage, damagePacket.IsCritical);
        }
    }
    public static void S_AddSkillHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_AddSkill addSkillPacket = packet as S_AddSkill;

        Managers.Data.AddSkill(addSkillPacket.SkillId);
    }

    public static void S_EquipHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_Equip sEquipPacket = packet as S_Equip;

        int index = sEquipPacket.Index;
        bool equip = sEquipPacket.Equip;

        Managers.Data.SetEquipInfo(index, equip);
    }
    public static void S_MyPartyInfoHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_MyPartyInfo sMyPartyInfoPacket = packet as S_MyPartyInfo;

        int partyOwnerId = sMyPartyInfoPacket.OwnerId;
        if (partyOwnerId == -1)
        {
            Managers.Party.SetPartyInfo(null, null);
        }
        else
        {
            List<Tuple<int, bool>> partyMemberInfo = new List<Tuple<int, bool>>();
            partyMemberInfo.Add(Tuple.Create(partyOwnerId, true));
            foreach (int memberId in sMyPartyInfoPacket.MemberIds)
            {
                partyMemberInfo.Add(Tuple.Create(memberId, false));
            }
            string partyName = sMyPartyInfoPacket.PartyName;
            Managers.Party.SetPartyInfo(partyMemberInfo, partyName);
        }
    }
    public static void S_PartiesInfoHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_PartiesInfo sPartiesInfoPacket = packet as S_PartiesInfo;

        List<PartyInfo> partyInfos = new List<PartyInfo>();
        foreach(PartyInfo partyInfo in sPartiesInfoPacket.PartyInfos)
        {
            partyInfos.Add(partyInfo);
        }

        UI_Party partyUI = Managers.UI.Root.GetComponentInChildren<UI_Party>();
        if (partyUI)
        {
            partyUI.RefreshPartyElements(partyInfos);
        }
    }
    public static void S_PartyWarningHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_PartyWarning sPartyWarningPacket = packet as S_PartyWarning;

        PartyWarning partyWarning = sPartyWarningPacket.WarningType;
        if (partyWarning != PartyWarning.PartyJoinSuccess)
        {
            UI_MessageWindow messageWindow = Managers.UI.Root.GetComponentInChildren<UI_MessageWindow>();
            if(messageWindow == null)
            {
                messageWindow = Managers.UI.ShowPopupUI<UI_MessageWindow>();
                messageWindow.Init();
            }
            messageWindow.SetMessage(partyWarning);

            UI_Party partyUI = Managers.UI.Root.GetComponentInChildren<UI_Party>();
            if (partyWarning == PartyWarning.PartyFull || partyWarning == PartyWarning.PartyNotExsist)
            {
                if (partyUI)
                {
                    partyUI.PushFindPartyButton(null);
                }
            }
        }
        else if(partyWarning == PartyWarning.PartyJoinSuccess)
        {
            UI_Party partyUI = Managers.UI.Root.GetComponentInChildren<UI_Party>();
            if (partyUI)
            {
                partyUI.PushMyPartyButton(null);
            }
        }
    }
    public static void S_ChangeSkillStateHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_ChangeSkillState sChangeSkillStatePacket = packet as S_ChangeSkillState;

        int objectId = sChangeSkillStatePacket.ObjectId;
        GameObject go = Managers.Object.FindById(objectId);
        DragonController dragonController = go.GetComponent<DragonController>();
        if (dragonController)
        {
            dragonController.SkillState = sChangeSkillStatePacket.SkillType;
        }
    }
    public static void S_ChangeTargetHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_ChangeTarget sChangeTargetPacket = packet as S_ChangeTarget;

        int objectId = sChangeTargetPacket.ObjectId;
        GameObject go = Managers.Object.FindById(objectId);
        DragonController dragonController = go.GetComponent<DragonController>();
        if (dragonController)
        {
            dragonController.TargetId = sChangeTargetPacket.TargetId;
        }
    }
    public static void S_CreateMeteoHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_CreateMeteo meteoPacket = packet as S_CreateMeteo;

        foreach(PositionInfo posInfo in meteoPacket.PosInfos)
        {
            GameObject go = Managers.Resource.Instantiate("Meteo");
            go.GetComponent<Meteo>().SetPos(posInfo);
        }
    }
    public static void S_PlayerAttackedHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = session as ServerSession;
        S_PlayerAttacked sPlayerAttackedPacket = packet as S_PlayerAttacked;

        Managers.Sound.Play("HItted");
    }
}


