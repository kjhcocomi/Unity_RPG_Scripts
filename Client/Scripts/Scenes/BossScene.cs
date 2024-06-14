using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Boss;

        //Managers.UI.ShowSceneUI<UI_Hud>();

        C_ChangeSceneComplete changeSceneCompletePacket = new C_ChangeSceneComplete();
        changeSceneCompletePacket.RoomType = RoomType.Boss;
        Managers.Network.Send(changeSceneCompletePacket);
    }
    public override void Clear()
    {
    }
}
