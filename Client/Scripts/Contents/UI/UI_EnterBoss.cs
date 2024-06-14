using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_EnterBoss : UI_Popup
{
    enum Buttons
    {
        Button_Close,
        Button_Enter
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        Get<Button>((int)Buttons.Button_Close).gameObject.BindEvent(PushCloseButton);
        Get<Button>((int)Buttons.Button_Enter).gameObject.BindEvent(PushEnterButton);

    }

    private void PushCloseButton(PointerEventData eventData)
    {
        ClosePopupUI();
    }
    private void PushEnterButton(PointerEventData eventData) 
    { 
        if (Managers.Party.OwnerId == -1)
        {
            UI_MessageWindow messageWindow = Managers.UI.Root.GetComponent<UI_MessageWindow>();
            if (messageWindow == null)
            {
                messageWindow = Managers.UI.ShowPopupUI<UI_MessageWindow>();
                messageWindow.Init();
            }
            messageWindow.SetMessage(PartyWarning.PartyNotJoined);
        }
        else if (Managers.Party.OwnerId != Managers.Object.MyPlayer.ObjectInfo.ObjectId)
        {
            UI_MessageWindow messageWindow = Managers.UI.Root.GetComponent<UI_MessageWindow>();
            if (messageWindow == null)
            {
                messageWindow = Managers.UI.ShowPopupUI<UI_MessageWindow>();
                messageWindow.Init();
            }
            messageWindow.SetMessage(PartyWarning.PartyNotPartyOwner);
        }
        else
        {
            C_Portal portalPacket = new C_Portal();
            portalPacket.RoomType = RoomType.Boss;
            Managers.Network.Send(portalPacket);
            ClosePopupUI();
        }
    }
}
