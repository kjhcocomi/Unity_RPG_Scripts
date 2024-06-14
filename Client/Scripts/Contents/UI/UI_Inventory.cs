using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory : UI_Popup
{
    enum Texts
    {
        // Money
        Text_Money,
        // Stat
        Text_Stat_Level,
        Text_Stat_Hp,
        Text_Stat_Mana,
        Text_Stat_Attack,
        Text_Stat_Speed,
        Text_Stat_CriticalProbability,
        Text_Stat_CriticalDamage,
        Text_Stat_Exp
    }
    enum Images
    {
        // 장착한 아이템
        Image_WeaponIcon,
        Image_RingIcon,
        Image_HeadIcon,
        Image_ArmorIcon,

        // lock
        Image_EquipSlotLockBg,
        Image_SkillSlotLockBg,
        Image_ConsumeSlotLockBg,

        Image_DragItem,

        // Consume Item
        Image_ConsumeItem1,
        Image_ConsumeItem2,

        // Skill
        Image_SkillIQ,
        Image_SkillIE,
    }
    enum Buttons
    {
        // Select Button
        Button_SelectEquip,
        Button_SelectComsume,
        Button_SelectSkill,
        // Close Button
        Button_Close,
        
    }
    enum GameObjects
    {
        Content
    }
    [SerializeField] private Sprite _selectTabSprite;
    [SerializeField] private Sprite _deSelectTabSprite;
    [SerializeField] private Sprite _emptySprite;
    private int _currentDragItemId = -1;
    private int _currentDragSkillId = -1;
    private int _currentIndex = -1;

    private int _headItemIdx = -1;
    private int _armorItemIdx = -1;
    private int _weaponItemIdx = -1;
    private int _ringItemIdx = -1;

    private List<UI_EquipItem> _equipItems = new List<UI_EquipItem>();
    public override void Init()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        Get<Button>((int)Buttons.Button_Close).gameObject.BindEvent(PushCloseButton);

        Get<Button>((int)Buttons.Button_SelectEquip).gameObject.BindEvent(PushEquipTab);
        Get<Button>((int)Buttons.Button_SelectComsume).gameObject.BindEvent(PushComsumeTab);
        Get<Button>((int)Buttons.Button_SelectSkill).gameObject.BindEvent(PushSKillTab);

        Get<Image>((int)Images.Image_ConsumeItem1).gameObject.BindEvent((p) => Drop(p, Images.Image_ConsumeItem1), Define.UIEvent.Drop);
        Get<Image>((int)Images.Image_ConsumeItem2).gameObject.BindEvent((p) => Drop(p, Images.Image_ConsumeItem2), Define.UIEvent.Drop);

        Get<Image>((int)Images.Image_WeaponIcon).gameObject.BindEvent((p) => Drop(p, Images.Image_WeaponIcon), Define.UIEvent.Drop);
        Get<Image>((int)Images.Image_RingIcon).gameObject.BindEvent((p) => Drop(p, Images.Image_RingIcon), Define.UIEvent.Drop);
        Get<Image>((int)Images.Image_HeadIcon).gameObject.BindEvent((p) => Drop(p, Images.Image_HeadIcon), Define.UIEvent.Drop);
        Get<Image>((int)Images.Image_ArmorIcon).gameObject.BindEvent((p) => Drop(p, Images.Image_ArmorIcon), Define.UIEvent.Drop);

        Get<Image>((int)Images.Image_SkillIQ).gameObject.BindEvent((p) => Drop(p, Images.Image_SkillIQ), Define.UIEvent.Drop);
        Get<Image>((int)Images.Image_SkillIE).gameObject.BindEvent((p) => Drop(p, Images.Image_SkillIE), Define.UIEvent.Drop);

        Get<Image>((int)Images.Image_WeaponIcon).gameObject.BindEvent((p) => PushEquipSlot(p, _weaponItemIdx));
        Get<Image>((int)Images.Image_RingIcon).gameObject.BindEvent((p) => PushEquipSlot(p, _ringItemIdx));
        Get<Image>((int)Images.Image_HeadIcon).gameObject.BindEvent((p) => PushEquipSlot(p, _headItemIdx));
        Get<Image>((int)Images.Image_ArmorIcon).gameObject.BindEvent((p) => PushEquipSlot(p, _armorItemIdx));

        Get<Image>((int)Images.Image_DragItem).gameObject.SetActive(false);

        RefreshInfo();
        PushEquipTab(null);
        RefreshEquipInfo();
        RefreshConsumeSlot();
        RefreshSkillSlot();
    }
    public void RefreshInfo()
    {
        RefreshMoneyInfo(Managers.Data.Money);
        RefreshStatInfo(Managers.Data.BaseStat, Managers.Data.AdditionalStat);
    }
    public void RefreshMoneyInfo(int money)
    {
        Get<TextMeshProUGUI>((int)Texts.Text_Money).text = $"{money}";
    }
    public void RefreshStatInfo(StatInfo baseStat, StatInfo addStat)
    {
        int level = baseStat.Level;

        int hp = baseStat.Hp;
        int maxHp = baseStat.MaxHp + addStat.MaxHp;

        int mp = baseStat.Mana;
        int maxMp = baseStat.MaxMana + addStat.MaxMana;

        int baseAttack = baseStat.Attack;
        int addAttack = addStat.Attack;

        int baseSpeed = baseStat.Speed;
        int addSpeed = addStat.Speed;

        int cp = baseStat.CriticalProbability;
        int addCp = addStat.CriticalProbability;

        int cd = baseStat.CriticalDamage;
        int addCd = addStat.CriticalDamage;

        float exp = baseStat.Exp;
        int maxExp = baseStat.MaxExp;

        Get<TextMeshProUGUI>((int)Texts.Text_Stat_Level).text = $"{level}";
        Get<TextMeshProUGUI>((int)Texts.Text_Stat_Hp).text = $"{hp} / {maxHp}";
        Get<TextMeshProUGUI>((int)Texts.Text_Stat_Mana).text = $"{mp} / {maxMp}";
        Get<TextMeshProUGUI>((int)Texts.Text_Stat_Attack).text = $"{baseAttack+addAttack}({baseAttack}+{addAttack})";
        Get<TextMeshProUGUI>((int)Texts.Text_Stat_Speed).text = $"{baseSpeed+addSpeed}({baseSpeed}+{addSpeed})";
        Get<TextMeshProUGUI>((int)Texts.Text_Stat_CriticalProbability).text = $"{cp+addCp}({cp}+{addCp})";
        Get<TextMeshProUGUI>((int)Texts.Text_Stat_CriticalDamage).text = $"{cd+addCd}({cd}+{addCd})";
        Get<TextMeshProUGUI>((int)Texts.Text_Stat_Exp).text = $"{exp} / {maxExp}";
    }
    public void RefreshEquipInfo()
    {
        _headItemIdx = -1;
        _armorItemIdx = -1;
        _weaponItemIdx = -1;
        _ringItemIdx = -1;
        for (int i = 0; i < _equipItems.Count; i++)
        {
            _equipItems[i].SetEquip(Managers.Data.EquipInfo[i]);
            if(_equipItems[i].IsEquip)
            {
                int itemId = _equipItems[i].ItemId;
                Sprite icon = Managers.Resource.Load<Sprite>(Managers.Data.ItemDict[itemId].iconPath);
                if (Managers.Data.ItemDict[itemId].equipArea == EquipArea.EquipHead)
                {
                    Get<Image>((int)Images.Image_HeadIcon).sprite = icon;
                    _headItemIdx = i;
                }
                else if (Managers.Data.ItemDict[itemId].equipArea == EquipArea.EquipArmor)
                {
                    Get<Image>((int)Images.Image_ArmorIcon).sprite = icon;
                    _armorItemIdx = i;
                }
                else if (Managers.Data.ItemDict[itemId].equipArea == EquipArea.EquipWeapon)
                {
                    Get<Image>((int)Images.Image_WeaponIcon).sprite = icon;
                    _weaponItemIdx = i;
                }
                else if (Managers.Data.ItemDict[itemId].equipArea == EquipArea.EquipRing)
                {
                    Get<Image>((int)Images.Image_RingIcon).sprite = icon;
                    _ringItemIdx = i;
                }
            }
        }
        if (_headItemIdx == -1) Get<Image>((int)Images.Image_HeadIcon).sprite = _emptySprite;
        if (_armorItemIdx == -1) Get<Image>((int)Images.Image_ArmorIcon).sprite = _emptySprite;
        if (_weaponItemIdx == -1) Get<Image>((int)Images.Image_WeaponIcon).sprite = _emptySprite;
        if (_ringItemIdx == -1) Get<Image>((int)Images.Image_RingIcon).sprite = _emptySprite;
    }
    public void RefreshConsumeSlot()
    {
        int slot1ItemId = -1;
        int slot2ItemId = -1;
        if(Managers.Data.ConsumeItemQuickSlot.TryGetValue('1', out slot1ItemId))
        {
            Sprite icon = Resources.Load<Sprite>(Managers.Data.ItemDict[slot1ItemId].iconPath);
            Get<Image>((int)Images.Image_ConsumeItem1).sprite = icon;
        }
        else
        {
            Get<Image>((int)Images.Image_ConsumeItem1).sprite = _emptySprite;
        }
        if(Managers.Data.ConsumeItemQuickSlot.TryGetValue('2', out slot2ItemId))
        {
            Sprite icon = Resources.Load<Sprite>(Managers.Data.ItemDict[slot2ItemId].iconPath);
            Get<Image>((int)Images.Image_ConsumeItem2).sprite = icon;
        }
        else
        {
            Get<Image>((int)Images.Image_ConsumeItem2).sprite = _emptySprite;
        }
    }
    public void RefreshSkillSlot()
    {
        int slot1SkillId = -1;
        int slot2SkillId = -1;
        if (Managers.Data.SkillQuickSlot.TryGetValue('q', out slot1SkillId))
        {
            Sprite icon = Resources.Load<Sprite>(Managers.Data.SkillDict[slot1SkillId].iconPath);
            Get<Image>((int)Images.Image_SkillIQ).sprite = icon;
        }
        else
        {
            Get<Image>((int)Images.Image_SkillIQ).sprite = _emptySprite;
        }
        if (Managers.Data.SkillQuickSlot.TryGetValue('e', out slot2SkillId))
        {
            Sprite icon = Resources.Load<Sprite>(Managers.Data.SkillDict[slot2SkillId].iconPath);
            Get<Image>((int)Images.Image_SkillIE).sprite = icon;
        }
        else
        {
            Get<Image>((int)Images.Image_SkillIE).sprite = _emptySprite;
        }
    }
    private void PushCloseButton(PointerEventData data)
    {
        ClosePopupUI();
    }
    private void PushEquipTab(PointerEventData data)
    {
        Get<Image>((int)Images.Image_EquipSlotLockBg).gameObject.SetActive(false);
        Get<Image>((int)Images.Image_ConsumeSlotLockBg).gameObject.SetActive(true);
        Get<Image>((int)Images.Image_SkillSlotLockBg).gameObject.SetActive(true);

        Get<Button>((int)Buttons.Button_SelectEquip).image.sprite = _selectTabSprite;
        Get<Button>((int)Buttons.Button_SelectComsume).image.sprite = _deSelectTabSprite;
        Get<Button>((int)Buttons.Button_SelectSkill).image.sprite = _deSelectTabSprite;

        GameObject content = Get<GameObject>((int)GameObjects.Content);
        foreach (Transform child in content.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
        _equipItems.Clear();
        for (int i= 0;i<Managers.Data.EquipItemInfo.Count;i++)
        {
            UI_EquipItem equipItem = Managers.UI.MakeSubItem<UI_EquipItem>(content.transform);
            equipItem.Init();
            equipItem.SetInfo(Managers.Data.EquipItemInfo[i], false, i);
            equipItem.Inventory = this;

            _equipItems.Add(equipItem);
        }
    }
    private void PushComsumeTab(PointerEventData data)
    {
        Get<Image>((int)Images.Image_EquipSlotLockBg).gameObject.SetActive(true);
        Get<Image>((int)Images.Image_ConsumeSlotLockBg).gameObject.SetActive(false);
        Get<Image>((int)Images.Image_SkillSlotLockBg).gameObject.SetActive(true);

        Get<Button>((int)Buttons.Button_SelectEquip).image.sprite = _deSelectTabSprite;
        Get<Button>((int)Buttons.Button_SelectComsume).image.sprite = _selectTabSprite;
        Get<Button>((int)Buttons.Button_SelectSkill).image.sprite = _deSelectTabSprite;

        GameObject content = Get<GameObject>((int)GameObjects.Content);
        foreach (Transform child in content.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
        foreach(var item in Managers.Data.ConsumeItemInfo)
        {
            int itemId = item.Key;
            int count = item.Value;
            UI_ConsumeItem consumeItem = Managers.UI.MakeSubItem<UI_ConsumeItem>(content.transform);
            consumeItem.Init();
            consumeItem.SetInfo(itemId, count);
            consumeItem.Inventory = this;
        }
    }
    private void PushSKillTab(PointerEventData data)
    {
        Get<Image>((int)Images.Image_EquipSlotLockBg).gameObject.SetActive(true);
        Get<Image>((int)Images.Image_ConsumeSlotLockBg).gameObject.SetActive(true);
        Get<Image>((int)Images.Image_SkillSlotLockBg).gameObject.SetActive(false);

        Get<Button>((int)Buttons.Button_SelectEquip).image.sprite = _deSelectTabSprite;
        Get<Button>((int)Buttons.Button_SelectComsume).image.sprite = _deSelectTabSprite;
        Get<Button>((int)Buttons.Button_SelectSkill).image.sprite = _selectTabSprite;

        GameObject content = Get<GameObject>((int)GameObjects.Content);
        foreach (Transform child in content.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
        foreach (var id in Managers.Data.OwnSkillInfo)
        {
            int skillId = id;
            UI_SkillSlot consumeItem = Managers.UI.MakeSubItem<UI_SkillSlot>(content.transform);
            consumeItem.Init();
            consumeItem.SetInfo(skillId);
            consumeItem.Inventory = this;
        }
    }
    private void Drop(PointerEventData data, Images slot)
    {
        if (slot == Images.Image_ConsumeItem1 || slot == Images.Image_ConsumeItem2)
        {
            if (_currentDragItemId == -1) return;
            if (Managers.Data.ItemDict[_currentDragItemId].itemType != ItemType.ItemComsume) return;
            if (slot == Images.Image_ConsumeItem1)
            {
                Managers.Data.SetConsumeItemSlot('1', _currentDragItemId);
            }
            else if (slot == Images.Image_ConsumeItem2)
            {
                Managers.Data.SetConsumeItemSlot('2', _currentDragItemId);
            }
            Managers.Sound.Play("Drop");
        }
        else if (slot == Images.Image_SkillIQ || slot == Images.Image_SkillIE)
        {
            if (_currentDragSkillId == -1) return;
            if (slot == Images.Image_SkillIQ)
            {
                Managers.Data.SetSkillSlot('q', _currentDragSkillId);
            }
            else if (slot == Images.Image_SkillIE)
            {
                Managers.Data.SetSkillSlot('e', _currentDragSkillId);
            }
            Managers.Sound.Play("Drop");
        }
        else
        {
            if (_currentDragItemId == -1) return;
            if (Managers.Data.ItemDict[_currentDragItemId].itemType != ItemType.ItemEquip) return;
            if (slot == Images.Image_HeadIcon && Managers.Data.ItemDict[_currentDragItemId].equipArea == EquipArea.EquipHead)
            {
                C_Equip equipPacket = new C_Equip();
                equipPacket.Index = _currentIndex;
                equipPacket.Equip = true;
                Managers.Network.Send(equipPacket);
            }
            else if (slot == Images.Image_ArmorIcon && Managers.Data.ItemDict[_currentDragItemId].equipArea == EquipArea.EquipArmor)
            {
                C_Equip equipPacket = new C_Equip();
                equipPacket.Index = _currentIndex;
                equipPacket.Equip = true;
                Managers.Network.Send(equipPacket);
            }
            else if (slot == Images.Image_WeaponIcon && Managers.Data.ItemDict[_currentDragItemId].equipArea == EquipArea.EquipWeapon)
            {
                C_Equip equipPacket = new C_Equip();
                equipPacket.Index = _currentIndex;
                equipPacket.Equip = true;
                Managers.Network.Send(equipPacket);
            }
            else if (slot == Images.Image_RingIcon && Managers.Data.ItemDict[_currentDragItemId].equipArea == EquipArea.EquipRing)
            {
                C_Equip equipPacket = new C_Equip();
                equipPacket.Index = _currentIndex;
                equipPacket.Equip = true;
                Managers.Network.Send(equipPacket);
            }
            Managers.Sound.Play("Drop");
        }
    }
    public void BeginDrag(PointerEventData data, int id, bool isItem = true, int index = -1)
    {
        Get<Image>((int)Images.Image_DragItem).gameObject.SetActive(true);
        Sprite icon = null;
        if (isItem)
        {
            _currentDragItemId = id;
            icon = Resources.Load<Sprite>(Managers.Data.ItemDict[_currentDragItemId].iconPath);
        }
        else
        {
            _currentDragSkillId = id;
            icon = Resources.Load<Sprite>(Managers.Data.SkillDict[_currentDragSkillId].iconPath);
        }
        _currentIndex = index;
        Get<Image>((int)Images.Image_DragItem).sprite = icon;
    }
    public void Drag(PointerEventData data)
    {
        Get<Image>((int)Images.Image_DragItem).rectTransform.position = data.position;
    }
    public void EndDrag(PointerEventData data, bool isItem = true)
    {
        if (isItem)
        {
            _currentDragItemId = -1;
        }
        else
        {
            _currentDragSkillId = -1;
        }
        _currentIndex = -1;
        Get<Image>((int)Images.Image_DragItem).gameObject.SetActive(false);
    }
    private void PushEquipSlot(PointerEventData data, int equipIdx)
    {
        if (equipIdx == -1) return;
        if (data.button != PointerEventData.InputButton.Right) return;

        C_Equip unEquipPacket = new C_Equip();
        unEquipPacket.Index = equipIdx;
        unEquipPacket.Equip = false;
        Managers.Network.Send(unEquipPacket);
    }
}
