using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Desc : UI_Base
{
    enum Texts
    {
        Text_Desc
    }
    bool _init = false;
    public override void Init()
    {
        if (_init) return;
        _init = true;
        Bind<TextMeshProUGUI>(typeof(Texts));
    }
    public void SetText(string text)
    {
        Get<TextMeshProUGUI>((int)Texts.Text_Desc).text = text;
    }
}
