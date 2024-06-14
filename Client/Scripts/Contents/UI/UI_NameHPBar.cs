using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_NameHPBar : UI_Base
{
    Camera _camera = null;
    bool _init = false;
    enum Images
    {
        Image_HPBar
    }
    enum Texts
    {
        Text_Name
    }
    public override void Init()
    {
        if (_init) return;
        _init = true;
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        _camera = Camera.main;
    }
    private void FixedUpdate()
    {
        if (_camera != null)
        {
            transform.rotation = _camera.transform.rotation;
        }
    }
    public void SetInfo(ObjectInfo info)
    {
        SetName(info.Name);
        SetHp(info.BaseStat);
    }
    public void SetName(string name)
    {
        Get<TextMeshProUGUI>((int)Texts.Text_Name).text = name;
    }
    public void SetHp(StatInfo baseStatInfo)
    {
        int hp = baseStatInfo.Hp;
        int maxHp = baseStatInfo.MaxHp;
        float ratio = hp / (float)maxHp;
        Get<Image>((int)Images.Image_HPBar).fillAmount = ratio;
    }
}
