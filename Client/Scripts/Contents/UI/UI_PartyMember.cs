using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PartyMember : UI_Base
{
    enum Images
    {
        Image_PositionIcon
    }
    enum Texts
    {
        Text_Name,
        Text_Level,
    }
    [SerializeField] private Sprite _partyOwnerIcon;
    [SerializeField] private Sprite _emptyIcon;

    private bool _init = false;
    public override void Init()
    {
        if (_init) return;
        _init = true;
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        GetComponent<RectTransform>().localScale = Vector3.one;
    }
    public void SetInfo(int objId, bool partyOwner)
    {
        GameObject go = Managers.Object.FindById(objId);
        BaseController member = go.GetComponent<BaseController>();
        if (partyOwner)
        {
            Get<Image>((int)Images.Image_PositionIcon).sprite = _partyOwnerIcon;
        }
        else
        {
            Get<Image>((int)Images.Image_PositionIcon).sprite = _emptyIcon;
        }

        Get<TextMeshProUGUI>((int)Texts.Text_Name).text = member.ObjectInfo.Name;
        Get<TextMeshProUGUI>((int)Texts.Text_Level).text = member.ObjectInfo.BaseStat.Level.ToString();
        if (objId == Managers.Object.MyPlayer.Id)
        {
            Get<TextMeshProUGUI>((int)Texts.Text_Name).color = Color.yellow;
            Get<TextMeshProUGUI>((int)Texts.Text_Level).color = Color.yellow;
        }
    }
}
