using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageScene : BaseScene
{
    // Start is called before the first frame update
    static bool _firstEnter = true;
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Village;

        if(_firstEnter)
        {
            _firstEnter = false;
            Managers.UI.ShowSceneUI<UI_Hud>();
        }
        

        C_ChangeSceneComplete changeSceneCompletePacket = new C_ChangeSceneComplete();
        changeSceneCompletePacket.RoomType = RoomType.Village;
        Managers.Network.Send(changeSceneCompletePacket);
    }

    public override void Clear()
    {

    }
}
