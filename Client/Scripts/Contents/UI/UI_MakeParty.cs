using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MakeParty : UI_Popup
{
    enum InputFields
    {
        InputField_PartyName
    }
    enum Buttons
    {
        Button_Confirm,
        Button_Cancel
    }
    public override void Init()
    {
        base.Init();
        Bind<TMP_InputField>(typeof(InputFields));
        Bind<Button>(typeof(Buttons));

        Get<Button>((int)Buttons.Button_Confirm).gameObject.BindEvent(PushConfirmButton);
        Get<Button>((int)Buttons.Button_Cancel).gameObject.BindEvent(PushCancelButton);
    }
    private void PushConfirmButton(PointerEventData eventData)
    {
        C_MakeParty makePartyPacket = new C_MakeParty();
        makePartyPacket.PartyName = Get<TMP_InputField>((int)InputFields.InputField_PartyName).text;
        Managers.Network.Send(makePartyPacket);
        ClosePopupUI();
    }
    private void PushCancelButton(PointerEventData eventData)
    {
        ClosePopupUI();
    }
}
