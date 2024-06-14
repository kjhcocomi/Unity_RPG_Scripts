using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_EquipItem : UI_Base
{
    enum Images
    {
        Image_ItemIcon
    }
    enum Texts
    {
        Text_ItemEquip
    }
    public UI_Inventory Inventory;
    public int ItemId;
    public bool IsEquip;
    public int Index;

    bool _init = false;
    GameObject _descUI = null;
    public override void Init()
    {
        if (_init) return;
        _init = true;
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        Get<Image>((int)Images.Image_ItemIcon).gameObject.BindEvent(EnterCursor, Define.UIEvent.PointerEnter);
        Get<Image>((int)Images.Image_ItemIcon).gameObject.BindEvent(ExitCursor, Define.UIEvent.PointerExit);
        Get<Image>((int)Images.Image_ItemIcon).gameObject.BindEvent(BeginDrag, Define.UIEvent.BeginDrag);
        Get<Image>((int)Images.Image_ItemIcon).gameObject.BindEvent(Drag, Define.UIEvent.Drag);
        Get<Image>((int)Images.Image_ItemIcon).gameObject.BindEvent(EndDrag, Define.UIEvent.EndDrag);

        GetComponent<RectTransform>().localScale = Vector3.one;
    }
    public void SetInfo(int itemId, bool isEquip, int index)
    {
        ItemId = itemId;
        Index = index;
        Sprite icon = Resources.Load<Sprite>(Managers.Data.ItemDict[itemId].iconPath);
        Debug.Log(Get<Image>((int)Images.Image_ItemIcon));
        Get<Image>((int)Images.Image_ItemIcon).sprite = icon;
        SetEquip(isEquip);
    }
    public void SetEquip(bool isEquip)
    {
        IsEquip = isEquip;
        if(isEquip)
        {
            Get<TextMeshProUGUI>((int)Texts.Text_ItemEquip).gameObject.SetActive(true);
        }
        else
        {
            Get<TextMeshProUGUI>((int)Texts.Text_ItemEquip).gameObject.SetActive(false);
        }
    }
    private void EnterCursor(PointerEventData eventData)
    {
        if (_descUI != null) return;
        _descUI = Managers.Resource.Instantiate("UI/UI_Desc");
        UI_Desc ui = _descUI.GetComponent<UI_Desc>();
        ui.transform.GetChild(0).position = eventData.position + Vector2.right * 50;
        ui.Init();
        ui.SetText(Managers.Data.ItemDict[ItemId].description);
    }
    private void ExitCursor(PointerEventData eventData)
    {
        if (_descUI == null) return;
        Destroy(_descUI);
    }
    private void BeginDrag(PointerEventData eventData)
    {
        Inventory.BeginDrag(eventData, ItemId, true, Index);
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
        if(_descUI) Destroy(_descUI);
    }
}
