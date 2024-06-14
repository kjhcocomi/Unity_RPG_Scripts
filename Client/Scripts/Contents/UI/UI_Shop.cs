using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Shop : UI_Popup
{
    enum Images
    {
        Image_Hp,
        Image_Mp,
        Image_Skill1,
        Image_Skill2,
        Image_Helmet,
        Image_Armor,
        Image_Weapon,
        Image_Ring,
    }
    enum Texts
    {
        Text_Money
    }
    enum Buttons
    {
        Button_Close,

        Button_Purchase_HpPotion,
        Button_Purchase_MpPotion,
        Button_Purchase_Skill1,
        Button_Purchase_Skill2,
        Button_Purchase_Helmet,
        Button_Purchase_Armor,
        Button_Purchase_Weapon,
        Button_Purchase_Ring
    }
    GameObject _descUI = null;
    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        Get<Button>((int)Buttons.Button_Close).gameObject.BindEvent(PushCloseButton);

        // Purchase Button
        Get<Button>((int)Buttons.Button_Purchase_HpPotion).gameObject.BindEvent((p) => PushPurchaseItemButton(p, 1));
        Get<Button>((int)Buttons.Button_Purchase_MpPotion).gameObject.BindEvent((p) => PushPurchaseItemButton(p, 2));
        Get<Button>((int)Buttons.Button_Purchase_Helmet).gameObject.BindEvent((p) => PushPurchaseItemButton(p, 3));
        Get<Button>((int)Buttons.Button_Purchase_Armor).gameObject.BindEvent((p) => PushPurchaseItemButton(p, 4));
        Get<Button>((int)Buttons.Button_Purchase_Weapon).gameObject.BindEvent((p) => PushPurchaseItemButton(p, 5));
        Get<Button>((int)Buttons.Button_Purchase_Ring).gameObject.BindEvent((p) => PushPurchaseItemButton(p, 6));

        Get<Button>((int)Buttons.Button_Purchase_Skill1).gameObject.BindEvent((p) => PushPurchaseSkillButton(p, 5));
        Get<Button>((int)Buttons.Button_Purchase_Skill2).gameObject.BindEvent((p) => PushPurchaseSkillButton(p, 6));

        // Enter Cursor
        Get<Image>((int)Images.Image_Hp).gameObject.BindEvent((p) => EnterCursor_Item(p, 1), Define.UIEvent.PointerEnter);
        Get<Image>((int)Images.Image_Mp).gameObject.BindEvent((p) => EnterCursor_Item(p, 2), Define.UIEvent.PointerEnter);
        Get<Image>((int)Images.Image_Helmet).gameObject.BindEvent((p) => EnterCursor_Item(p, 3), Define.UIEvent.PointerEnter);
        Get<Image>((int)Images.Image_Armor).gameObject.BindEvent((p) => EnterCursor_Item(p, 4), Define.UIEvent.PointerEnter);
        Get<Image>((int)Images.Image_Weapon).gameObject.BindEvent((p) => EnterCursor_Item(p, 5), Define.UIEvent.PointerEnter);
        Get<Image>((int)Images.Image_Ring).gameObject.BindEvent((p) => EnterCursor_Item(p, 6), Define.UIEvent.PointerEnter);

        Get<Image>((int)Images.Image_Skill1).gameObject.BindEvent((p) => EnterCursor_Skill(p, 5), Define.UIEvent.PointerEnter);
        Get<Image>((int)Images.Image_Skill2).gameObject.BindEvent((p) => EnterCursor_Skill(p, 6), Define.UIEvent.PointerEnter);

        // Exit Cursor
        Get<Image>((int)Images.Image_Hp).gameObject.BindEvent(ExitCursor, Define.UIEvent.PointerExit);
        Get<Image>((int)Images.Image_Mp).gameObject.BindEvent(ExitCursor, Define.UIEvent.PointerExit);
        Get<Image>((int)Images.Image_Helmet).gameObject.BindEvent(ExitCursor, Define.UIEvent.PointerExit);
        Get<Image>((int)Images.Image_Armor).gameObject.BindEvent(ExitCursor, Define.UIEvent.PointerExit);
        Get<Image>((int)Images.Image_Weapon).gameObject.BindEvent(ExitCursor, Define.UIEvent.PointerExit);
        Get<Image>((int)Images.Image_Ring).gameObject.BindEvent(ExitCursor, Define.UIEvent.PointerExit);
        Get<Image>((int)Images.Image_Skill1).gameObject.BindEvent(ExitCursor, Define.UIEvent.PointerExit);
        Get<Image>((int)Images.Image_Skill2).gameObject.BindEvent(ExitCursor, Define.UIEvent.PointerExit);

        RefreshMoney();
    }
    public void RefreshMoney()
    {
        Get<TextMeshProUGUI>((int)Texts.Text_Money).text = Managers.Data.Money.ToString();
    }
    private void PushCloseButton(PointerEventData data) 
    {
        ClosePopupUI();
    }
    private void PushPurchaseItemButton(PointerEventData data, int itemId)
    {
        int price = 0;
        if (itemId <= 2) price = 3;
        else price = 15;

        if (Managers.Data.Money >= price)
        {
            C_PurchaseItem purchaseItemPacket = new C_PurchaseItem();
            purchaseItemPacket.ItemId = itemId;
            Managers.Network.Send(purchaseItemPacket);
            Managers.Sound.Play("Buy");
        }
    }
    private void PushPurchaseSkillButton(PointerEventData data, int skillId)
    {
        int price = 10;

        if (Managers.Data.Money >= price)
        {
            C_PurchaseSkill purchaseSkillPacket = new C_PurchaseSkill();
            purchaseSkillPacket.SkillId = skillId;
            Managers.Network.Send(purchaseSkillPacket);
            Managers.Sound.Play("Buy");
        }
    }
    private void EnterCursor_Item(PointerEventData data, int itemId)
    {
        if (_descUI != null) return;
        _descUI = Managers.Resource.Instantiate("UI/UI_Desc");
        UI_Desc ui = _descUI.GetComponent<UI_Desc>();
        ui.transform.GetChild(0).position = data.position + Vector2.right * 150;
        ui.Init();
        ui.SetText(Managers.Data.ItemDict[itemId].description);
    }

    private void EnterCursor_Skill(PointerEventData data, int skillId)
    {
        if (_descUI != null) return;
        _descUI = Managers.Resource.Instantiate("UI/UI_Desc");
        UI_Desc ui = _descUI.GetComponent<UI_Desc>();
        ui.transform.GetChild(0).position = data.position + Vector2.right * 150;
        ui.Init();
        ui.SetText(Managers.Data.SkillDict[skillId].description);
    }
    private void ExitCursor(PointerEventData data)
    {
        if (_descUI == null) return;
        Destroy(_descUI);
    }
}
