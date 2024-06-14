using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillSlot : UI_Base
{
    enum Images
    {
        Image_SkillIcon
    }
    public UI_Inventory Inventory;
    public int SkillId;

    bool _init = false;
    GameObject _descUI = null;
    public override void Init()
    {
        if (_init) return;
        _init = true;
        Bind<Image>(typeof(Images));

        Get<Image>((int)Images.Image_SkillIcon).gameObject.BindEvent(EnterCursor, Define.UIEvent.PointerEnter);
        Get<Image>((int)Images.Image_SkillIcon).gameObject.BindEvent(ExitCursor, Define.UIEvent.PointerExit);
        Get<Image>((int)Images.Image_SkillIcon).gameObject.BindEvent(BeginDrag, Define.UIEvent.BeginDrag);
        Get<Image>((int)Images.Image_SkillIcon).gameObject.BindEvent(Drag, Define.UIEvent.Drag);
        Get<Image>((int)Images.Image_SkillIcon).gameObject.BindEvent(EndDrag, Define.UIEvent.EndDrag);

        GetComponent<RectTransform>().localScale = Vector3.one;
    }
    public void SetInfo(int skillId)
    {
        SkillId = skillId;
        Sprite icon = Resources.Load<Sprite>(Managers.Data.SkillDict[skillId].iconPath);
        Get<Image>((int)Images.Image_SkillIcon).sprite = icon;
    }
    private void EnterCursor(PointerEventData eventData)
    {
        if (_descUI != null) return;
        _descUI = Managers.Resource.Instantiate("UI/UI_Desc");
        UI_Desc ui = _descUI.GetComponent<UI_Desc>();
        ui.transform.GetChild(0).position = eventData.position + Vector2.right * 50;
        ui.Init();
        ui.SetText(Managers.Data.SkillDict[SkillId].description);
    }
    private void ExitCursor(PointerEventData eventData)
    {
        if (_descUI == null) return;
        Destroy(_descUI);
    }
    private void BeginDrag(PointerEventData eventData)
    {
        Inventory.BeginDrag(eventData, SkillId, false);
        if (_descUI == null) return;
        Destroy(_descUI);
    }
    private void Drag(PointerEventData eventData)
    {
        Inventory.Drag(eventData);
    }
    private void EndDrag(PointerEventData eventData)
    {
        Inventory.EndDrag(eventData);
    }
    private void OnDestroy()
    {
        if (_descUI) Destroy(_descUI);
    }
}
