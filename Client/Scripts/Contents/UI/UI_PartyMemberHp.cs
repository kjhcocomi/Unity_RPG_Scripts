using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PartyMemberHp : UI_Base
{
    enum Images
    {
        Image_Hp,
        Image_PartyOwnerIcon,
    }
    enum Texts
    {
        Text_MemberName
    }
    [SerializeField] private Sprite _partyOwnerIcon;
    [SerializeField] private Sprite _emptyIcon;
    bool _init = false;
    public override void Init()
    {
        if(_init) return;
        _init = true;
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        GetComponent<RectTransform>().localScale = Vector3.one;
    }
    public void SetInfo(ObjectInfo info, bool isPartyOwner)
    {
        Get<TextMeshProUGUI>((int)Texts.Text_MemberName).text = info.Name;

        if (isPartyOwner) Get<Image>((int)Images.Image_PartyOwnerIcon).sprite = _partyOwnerIcon;
        else Get<Image>((int)Images.Image_PartyOwnerIcon).sprite = _emptyIcon;

        if (info.ObjectId == Managers.Object.MyPlayer.ObjectInfo.ObjectId) Get<TextMeshProUGUI>((int)Texts.Text_MemberName).color = Color.yellow;

        SetHp(info);
    }
    public void SetHp(ObjectInfo info)
    {
        StatInfo stat = info.BaseStat;

        int hp = stat.Hp;
        int maxHp = stat.MaxHp;

        float value = hp / (float)maxHp;
        Get<Image>((int)Images.Image_Hp).fillAmount = value;
    }
}
