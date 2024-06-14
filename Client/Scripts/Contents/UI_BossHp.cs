using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossHp : UI_Base
{
    enum Images
    {
        Image_Hp,
        Image_Stun
    }
    enum Texts
    {
        Text_Hp
    }
    bool _init = false;
    public override void Init()
    {
        if (_init) return;
        _init = true;
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
    }
    public void RefreshInfo(StatInfo stat)
    {
        int hp = stat.Hp;
        int maxHp = stat.MaxHp;

        int stunCount = stat.StunCount;
        int maxStunCount = stat.MaxStunCount;

        Get<Image>((int)Images.Image_Hp).fillAmount = (float)hp / maxHp;
        Get<Image>((int)Images.Image_Stun).fillAmount = (float)stunCount / maxStunCount;
        Get<TextMeshProUGUI>((int)Texts.Text_Hp).text = $"{hp} / {maxHp}";
    }
}
