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
                message = "������ ��Ƽ�� �����ϴ�.";
                break;
            case PartyWarning.PartyAlreadyJoined:
                message = "�̹� ��Ƽ�� �ֽ��ϴ�.";
                break;
            case PartyWarning.PartyFull:
                message = "��Ƽ�� �ִ� �ο��Դϴ�.";
                break;
            case PartyWarning.PartyNotExsist:
                message = "�������� �ʴ� ��Ƽ�Դϴ�.";
                break;
            case PartyWarning.PartyNotPartyOwner:
                message = "��Ƽ���� �ƴմϴ�.";
                break;
        }
        Get<TextMeshProUGUI>((int)Texts.Text_Message).text = message;
    }
    private void PushConfirmButton(PointerEventData eventData)
    {
        ClosePopupUI();
    }
}
