using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MessageWindow : UI_Popup
{
    enum Texts
    {
        Text_Message
    }
    enum Buttons
    {
        Button_Confirm
    }
    private bool _init = false;
    public override void Init()
    {
        if (_init) return;
        _init = true;
        base.Init();
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        Get<Button>((int)Buttons.Button_Confirm).gameObject.BindEvent(PushConfirmButton);
    }
    public void SetMessage(PartyWarning partyWarning)
    {
        string message = "";
        switch(partyWarning)
        {
            case PartyWarning.PartyNotJoined:
                message = "가입한 파티가 없습니다.";
                break;
            case PartyWarning.PartyAlreadyJoined:
                message = "이미 파티가 있습니다.";
                break;
            case PartyWarning.PartyFull:
                message = "파티가 최대 인원입니다.";
                break;
            case PartyWarning.PartyNotExsist:
                message = "존재하지 않는 파티입니다.";
                break;
            case PartyWarning.PartyNotPartyOwner:
                message = "파티장이 아닙니다.";
                break;
        }
        Get<TextMeshProUGUI>((int)Texts.Text_Message).text = message;
    }
    private void PushConfirmButton(PointerEventData eventData)
    {
        ClosePopupUI();
    }
}
