using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Party : UI_Popup
{
    enum Images
    {
    }
    enum Texts
    {
        // MyParty
        Text_PartyName,

    }
    enum Buttons
    {
        Button_Close,

        Button_MyParty,
        Button_FindParty,

        // MyParty
        Button_MakeParty,
        Button_ExitParty,
    }
    enum GameObjects 
    {
        MyParty,
        FindParty,

        // MyParty
        PartyMemberRegion,

        // FindParty
        Parent_PartyElement
    }
    [SerializeField] private Sprite _selectedTabSprite;
    [SerializeField] private Sprite _deSelectedTabSprite;
    public override void Init()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        Get<Button>((int)Buttons.Button_Close).gameObject.BindEvent(PushCloseButton);
        Get<Button>((int)Buttons.Button_MyParty).gameObject.BindEvent(PushMyPartyButton);
        Get<Button>((int)Buttons.Button_FindParty).gameObject.BindEvent(PushFindPartyButton);
        Get<Button>((int)Buttons.Button_MakeParty).gameObject.BindEvent(PushMakePartyButton);
        Get<Button>((int)Buttons.Button_ExitParty).gameObject.BindEvent(ExitPartyButton);

        PushMyPartyButton(null);
    }
    #region Buttons Events
    private void PushCloseButton(PointerEventData eventData)
    {
        ClosePopupUI();
    }
    public void PushMyPartyButton(PointerEventData eventData)
    {
        // Change Tab Sprite
        Get<Button>((int)Buttons.Button_MyParty).image.sprite = _selectedTabSprite;
        Get<Button>((int)Buttons.Button_FindParty).image.sprite = _deSelectedTabSprite;

        // Refresh Contents
        Get<GameObject>((int)GameObjects.MyParty).SetActive(true);
        Get<GameObject>((int)GameObjects.FindParty).SetActive(false);

        C_MyPartyInfoReq myPartyInfoReqPacket = new C_MyPartyInfoReq();
        Managers.Network.Send(myPartyInfoReqPacket);
    }
    public void PushFindPartyButton(PointerEventData eventData)
    {
        // Change Tab Sprite
        Get<Button>((int)Buttons.Button_MyParty).image.sprite = _deSelectedTabSprite;
        Get<Button>((int)Buttons.Button_FindParty).image.sprite = _selectedTabSprite;

        // Refresh Contents
        Get<GameObject>((int)GameObjects.MyParty).SetActive(false);
        Get<GameObject>((int)GameObjects.FindParty).SetActive(true);

        C_PartiesInfoReq partiesInfoReqPacket = new C_PartiesInfoReq();
        Managers.Network.Send(partiesInfoReqPacket);
    }
    private void PushMakePartyButton(PointerEventData eventData)
    {
        if (Managers.Party.OwnerId == -1)
        {
            if (Managers.UI.Root.GetComponentInChildren<UI_MakeParty>() == null)
            {
                Managers.UI.ShowPopupUI<UI_MakeParty>();
            }
        }
        else
        {
            UI_MessageWindow messageWindow = Managers.UI.Root.GetComponentInChildren<UI_MessageWindow>();
            if (messageWindow == null)
            {
                messageWindow = Managers.UI.ShowPopupUI<UI_MessageWindow>();
                messageWindow.Init();
            }
            messageWindow.SetMessage(PartyWarning.PartyAlreadyJoined);
        }
    }
    private void ExitPartyButton(PointerEventData eventData)
    {
        Debug.Log("Exit Party");

        C_ExitParty exitPartyPacket = new C_ExitParty();
        Managers.Network.Send(exitPartyPacket);
    }
    public void RefreshPartyMembers()
    {
        GameObject partyMembers = Get<GameObject>((int)GameObjects.PartyMemberRegion);
        foreach (Transform child in partyMembers.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
        // 파티 있으면 파티 멤버들 보여주는 UI 생성
        if(Managers.Party.OwnerId == -1)
        {
            Get<TextMeshProUGUI>((int)Texts.Text_PartyName).text = "없음";
        }
        else
        {
            Get<TextMeshProUGUI>((int)Texts.Text_PartyName).text = Managers.Party.PartyName;
            {
                // Owner
                UI_PartyMember partyMemberSubItem = Managers.UI.MakeSubItem<UI_PartyMember>(partyMembers.transform);
                partyMemberSubItem.Init();
                partyMemberSubItem.SetInfo(Managers.Party.OwnerId, true);
            }
            foreach(int memberId in Managers.Party.MemberIds)
            {
                // Members
                UI_PartyMember partyMemberSubItem = Managers.UI.MakeSubItem<UI_PartyMember>(partyMembers.transform);
                partyMemberSubItem.Init();
                partyMemberSubItem.SetInfo(memberId, false);
            }
        }
    }
    public void RefreshPartyElements(List<PartyInfo> partyInfos)
    {
        GameObject partyElements = Get<GameObject>((int)GameObjects.Parent_PartyElement);
        foreach (Transform child in partyElements.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
        // TODO : UI_PartyElement
        if (partyInfos != null)
        {
            foreach(PartyInfo partyInfo in partyInfos)
            {
                UI_PartyElement partyElement = Managers.UI.MakeSubItem<UI_PartyElement>(partyElements.transform);
                partyElement.Init();
                partyElement.SetInfo(this, partyInfo.PartyId, partyInfo.MemberCount, partyInfo.PartyName);
            }
        }
    }
    #endregion
}
