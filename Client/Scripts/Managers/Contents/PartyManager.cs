using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager
{
    public int OwnerId { get; private set; } = -1;
    public List<int> MemberIds { get; private set; } = new List<int>();
    public string PartyName { get; private set; }
    public void SetPartyInfo(List<Tuple<int, bool>> partyMemberInfo, string partyName)
    {
        OwnerId = -1;
        MemberIds.Clear();
        PartyName = string.Empty;
        if (partyMemberInfo != null)
        { 
            foreach(var partyMember in partyMemberInfo)
            {
                if(partyMember.Item2)
                {
                    OwnerId = partyMember.Item1;
                }
                else
                {
                    MemberIds.Add(partyMember.Item1);
                }
            }
            PartyName = partyName;
        }
        UI_Party partyUI = Managers.UI.Root.GetComponentInChildren<UI_Party>();
        if (partyUI)
        {
            partyUI.RefreshPartyMembers();
        }
        UI_Hud hud = Managers.UI.SceneUI as UI_Hud;
        if (hud)
        {
            hud.RefreshPartyInfo();
        }
    }
    public bool IsPartyMember(int playerId)
    {
        if (OwnerId == -1) return false;
        if (playerId == OwnerId) return true;
        foreach (int memberId in  MemberIds)
        {
            if (playerId == memberId) 
                return true;
        }
        return false;
    }
}
