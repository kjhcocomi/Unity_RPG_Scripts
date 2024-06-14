using Data;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.Skill> SkillDict { get; private set; } = new Dictionary<int, Data.Skill>();
    public Dictionary<int, Data.EquipItemStat> EquipItemStatDict { get; private set; } = new Dictionary<int, Data.EquipItemStat>();
    public Dictionary<int, Data.Item> ItemDict { get; private set; } = new Dictionary<int, Data.Item>();

    public void Init()
    {
        SkillDict = LoadJson<Data.SkillData, int, Data.Skill>("SkillData").MakeDict();
        EquipItemStatDict = LoadJson<Data.EquipItemStatData, int, Data.EquipItemStat>("EquipItemStatData").MakeDict();
        ItemDict = LoadJson<Data.ItemData, int, Data.Item>("ItemData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
    private void RefreshHud()
    {
        UI_Hud hud = Managers.UI.SceneUI as UI_Hud;
        if(hud)
        {
            hud.RefreshHud(_baseStat, _additionalStat);
        }
    }
    #region Stat
    private int _level = 1;
    private StatInfo _baseStat = new StatInfo();
    private StatInfo _additionalStat = new StatInfo();
    public StatInfo BaseStat 
    {
        get { return _baseStat; }
        set
        {
            _baseStat = value;
            RefreshHud();
            UI_Inventory inventory = Managers.UI.Root.GetComponentInChildren<UI_Inventory>();
            if (inventory)
            {
                inventory.RefreshStatInfo(_baseStat, _additionalStat);
            }
        }
    }
    public StatInfo AdditionalStat
    {
        get { return _additionalStat; }
        set
        {
            _additionalStat = value;
            RefreshHud();
            UI_Inventory inventory = Managers.UI.Root.GetComponentInChildren<UI_Inventory>();
            if (inventory)
            {
                inventory.RefreshStatInfo(_baseStat, _additionalStat);
            }
        }
    }
    public int Level
    {
        get { return _level; }
        set
        {
            _level = value;
            UI_Hud hud = Managers.UI.SceneUI as UI_Hud;
            if (hud)
            {
                hud.SetLevel(_level);
            }
        }
    }
    #endregion
    #region Item
    private int _money = 0;
    public int Money
    {
        get { return _money; }
        set
        {
            _money = value;
            UI_Inventory inventory = Managers.UI.Root.GetComponentInChildren<UI_Inventory>();
            if (inventory)
            {
                inventory.RefreshMoneyInfo(_money);
            }
            UI_Shop shop = Managers.UI.Root.GetComponentInChildren<UI_Shop>();
            if(shop)
            {
                shop.RefreshMoney();
            }
        }
    }
    public Dictionary<int, int> ConsumeItemInfo { get; set; } = new Dictionary<int, int>();
    public Dictionary<char, int> ConsumeItemQuickSlot { get; set; } = new Dictionary<char, int>();
    public List<int> EquipItemInfo { get; set; } = new List<int>();
    public List<bool> EquipInfo { get; set; } = new List<bool>();
    public List<int> OwnSkillInfo { get; set; } = new List<int>();
    public Dictionary<char, int> SkillQuickSlot { get; set; } = new Dictionary<char, int>();
    public void AddItem(int itemId, int addCount)
    {
        if (ItemDict[itemId].itemType == ItemType.ItemComsume)
        {
            int currentCount;
            if (ConsumeItemInfo.TryGetValue(itemId, out currentCount))
            {
                ConsumeItemInfo[itemId] += addCount;
            }
            else
            {
                ConsumeItemInfo.Add(itemId, addCount);
            }
            UI_Hud hud = Managers.UI.SceneUI as UI_Hud;
            if (hud)
            {
                hud.RefreshItemSlot();
            }
        }
        else if(ItemDict[itemId].itemType == ItemType.ItemEquip)
        {
            if (addCount == 1)
            {
                EquipItemInfo.Add(itemId);
                EquipInfo.Add(false);
            }
            //else if(addCount == -1)
            //{
            //    EquipItemInfo.Remove(itemId);
            //}
        }
    }
    public void SetEquipInfo(int index, bool equip)
    {
        EquipInfo[index] = equip;
        UI_Inventory inventory = Managers.UI.Root.GetComponentInChildren<UI_Inventory>();
        if (inventory)
        {
            inventory.RefreshEquipInfo();
        }
    }
    public void AddSkill(int skillId)
    {
        OwnSkillInfo.Add(skillId);
    }
    public void SetConsumeItemSlot(char key, int itemId)
    {
        if (key != '1' && key != '2') return;
        int slot1ItemId = -1;
        int slot2ItemId = -1;
        ConsumeItemQuickSlot.TryGetValue('1', out slot1ItemId);
        ConsumeItemQuickSlot.TryGetValue('2', out slot2ItemId);

        if(key=='1')
        {
            if(slot2ItemId == itemId) // 이미 다른 슬롯에 드래그한 아이템이 있을 경우
            {
                ConsumeItemQuickSlot.Remove('2');
            }
        }
        else if(key=='2')
        {
            if (slot1ItemId == itemId) // 이미 다른 슬롯에 드래그한 아이템이 있을 경우
            {
                ConsumeItemQuickSlot.Remove('1');
            }
        }

        ConsumeItemQuickSlot[key] = itemId;

        UI_Inventory inventory = Managers.UI.Root.GetComponentInChildren<UI_Inventory>();
        if (inventory)
        {
            inventory.RefreshConsumeSlot();
        }
        UI_Hud hud = Managers.UI.SceneUI as UI_Hud;
        if (hud)
        {
            hud.RefreshItemSlot();
        }
    }
    public void SetSkillSlot(char key, int skillId)
    {
        if (key != 'q' && key != 'e') return;
        int slot1SkillId = -1;
        int slot2SkillId = -1;
        SkillQuickSlot.TryGetValue('q', out slot1SkillId);
        SkillQuickSlot.TryGetValue('e', out slot2SkillId);

        if (key == 'q')
        {
            if (slot2SkillId == skillId) // 이미 다른 슬롯에 드래그한 아이템이 있을 경우
            {
                SkillQuickSlot.Remove('e');
            }
        }
        else if (key == 'e')
        {
            if (slot1SkillId == skillId) // 이미 다른 슬롯에 드래그한 아이템이 있을 경우
            {
                SkillQuickSlot.Remove('q');
            }
        }

        SkillQuickSlot[key] = skillId;

        UI_Inventory inventory = Managers.UI.Root.GetComponentInChildren<UI_Inventory>();
        if (inventory)
        {
            inventory.RefreshSkillSlot();
        }
        UI_Hud hud = Managers.UI.SceneUI as UI_Hud;
        if (hud)
        {
            hud.RefreshSkillSlot();
        }
    }
    #endregion

    #region Chat
    public List<UI_ChatElement> ChatElements { get; set; } = new List<UI_ChatElement>();
    #endregion
}
