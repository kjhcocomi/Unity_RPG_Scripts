using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
	#region Skill
	[Serializable]
	public class Skill
	{
        public int id;
        public string name;
        public float cooldown;
        public float damage;
        public float rangeX;
        public float rangeZ;
        public float rangeOffset;
        public string description;
        public string iconPath;
        public SkillType skillType;
        public float skillTime;
        public float delayTime;
        public int mana;
        public int price;
    }
    [Serializable]
    public class SkillData : ILoader<int, Skill>
	{
		public List<Skill> skills = new List<Skill>();

		public Dictionary<int, Skill> MakeDict()
		{
			Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
			foreach (Skill skill in skills)
				dict.Add(skill.id, skill);

            return dict;
		}
	}
    #endregion

    #region EquipItemStat
    [Serializable]
    public class EquipItemStat
    {
        public int maxHp;
        public int maxMana;
        public int attack;
        public int speed;
        public int criticalProbability;
        public int criticalDamage;
        public int id;
    }
    [Serializable]
    public class EquipItemStatData : ILoader<int, EquipItemStat>
    {
        public List<EquipItemStat> equipItemStats = new List<EquipItemStat>();

        public Dictionary<int, EquipItemStat> MakeDict()
        {
            Dictionary<int, EquipItemStat> dict = new Dictionary<int, EquipItemStat>();
            foreach (EquipItemStat stat in equipItemStats)
            {
                dict.Add(stat.id, stat);
            }
            return dict;
        }
    }
    #endregion

    #region Item
    [Serializable]
    public class Item
    {
        public int id;
        public string name;
        public string description;
        public string iconPath;
        public bool canStack;
        public EquipItemStat statEffect;
        public ItemType itemType;
        public EquipArea equipArea;
        public int price;
    }
    [Serializable]
    public class ItemData : ILoader<int, Item>
    {
        public List<Item> items = new List<Item>();

        public Dictionary<int, Item> MakeDict()
        {
            Dictionary<int, Item> dict = new Dictionary<int, Item>();
            foreach (Item item in items)
            {
                if (item.itemType == ItemType.ItemEquip)
                {
                    item.statEffect = Managers.Data.EquipItemStatDict[item.id];
                }
                dict.Add(item.id, item);
            }
            return dict;
        }
    }
    #endregion
}