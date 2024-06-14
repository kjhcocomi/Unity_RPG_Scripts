using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_PartyElement : UI_Base
{
    enum Texts
    {
        Text_PartyName,
        Text_MemberCount
    }
    enum Buttons
    {
        Button_JoinParty
    }
    private UI_Party _partyUI;
    private int _partyId;

    private bool _init = false;
    public override void Init()
    {
        if (_init) return;
        _init = true;
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        Get<Button>((int)Buttons.Button_JoinParty).gameObject.BindEvent(PushJoinButton);

        GetComponent<RectTransform>().localScale = Vector3.one;
    }
    public void SetInfo(UI_Party partyUI, int partyId, int memberCount, string partyName)
    {
        _partyUI = partyUI;
        _partyId = partyId;
        Get<TextMeshProUGUI>((int)Texts.Text_PartyName).text = partyName;
        Get<TextMeshProUGUI>((int)Texts.Text_MemberCount).text = $"{memberCount}/2";
    }
    private void PushJoinButton(PointerEventData eventData)
    {
        Debug.Log("Join Party");
        C_JoinParty joinPartyPacket = new C_JoinParty();
        joinPartyPacket.PartyId = _partyId;
        Managers.Network.Send(joinPartyPacket);
    }
}
