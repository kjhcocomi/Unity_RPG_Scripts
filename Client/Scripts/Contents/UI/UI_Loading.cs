using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : UI_Base
{
    bool _init = false;
    enum Images
    {
        Image_ProgressBar
    }
    public override void Init()
    {
        if (_init) return;
        _init = true;
        Bind<Image>(typeof(Images));
    }
    public void SetProgressBarValue(float value)
    {
        if (_init == false)
        {
            Init();
        }
        Get<Image>((int)Images.Image_ProgressBar).fillAmount = value;
    }

    public float GetProgressBarValue()
    {
        return Get<Image>((int)Images.Image_ProgressBar).fillAmount;
    }
}
