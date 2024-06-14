using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Lobby : UI_Scene
{
    enum Texts
    {
        Text_Name,
        Text_Connect
    }
    enum Buttons
    {
        Button_EnterGame
    }
    public override void Init()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        Get<Button>((int)Buttons.Button_EnterGame).gameObject.BindEvent(PushEnterGameButton);
    }

    private void PushEnterGameButton(PointerEventData data)
    {
        C_ConnectReq connectReqPacket = new C_ConnectReq();
        connectReqPacket.Name = Get<TextMeshProUGUI>((int)Texts.Text_Name).text;
        Managers.Network.Send(connectReqPacket);
    }

    public void SetConnectText(bool connect)
    {
        if (connect)
        {
            Get<TextMeshProUGUI>((int)Texts.Text_Connect).text = "Connected";
            Get<TextMeshProUGUI>((int)Texts.Text_Connect).color = Color.green;
        }
        else
        {
            Get<TextMeshProUGUI>((int)Texts.Text_Connect).text = "Disconnected";
            Get<TextMeshProUGUI>((int)Texts.Text_Connect).color = Color.red;
        }
    }
}
