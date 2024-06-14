using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public HeroPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    public void Add(ObjectInfo info, bool myPlayer = false)
    {
        GameObjectType objectType = GetObjectTypeById(info.ObjectId);
        if (objectType == GameObjectType.Player)
        {
            if (myPlayer)
            {
                GameObject go = Managers.Resource.Instantiate("Creature/HeroPlayer");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                MyPlayer = go.GetComponent<HeroPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer.ObjectInfo = info;
                MyPlayer.SyncPosAndRot(info.PosInfo, info.RotInfo);
            }
            else
            {
                GameObject go = Managers.Resource.Instantiate("Creature/Player");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                PlayerController pc = go.GetComponent<PlayerController>();
                pc.Id = info.ObjectId;
                pc.ObjectInfo = info;
                pc.SyncPosAndRot(info.PosInfo, info.RotInfo);

                UI_NameHPBar ui_NameHPBar = go.GetComponentInChildren<UI_NameHPBar>();
                if (ui_NameHPBar)
                {
                    ui_NameHPBar.Init();
                    ui_NameHPBar.SetInfo(info);
                }
            }
        }
        else if (objectType == GameObjectType.Monster)
        {
            GameObject go = null;
            if (info.BaseStat.Level == 1)
            {
                go = Managers.Resource.Instantiate("Creature/TestMonster");
            }
            else if (info.BaseStat.Level == 2)
            {
                go = Managers.Resource.Instantiate("Creature/TestMonster2");
            }
            
            if (go == null)
            {
                Debug.Log("Monster is Null");
                return;
            }

            _objects.Add(info.ObjectId, go);

            CreatureController cc = go.GetComponent<CreatureController>();
            cc.Id = info.ObjectId;
            cc.ObjectInfo = info;
            go.transform.position = new Vector3(info.PosInfo.PosX, 0, info.PosInfo.PosZ);

            UI_NameHPBar ui_NameHPBar = go.GetComponentInChildren<UI_NameHPBar>();
            if (ui_NameHPBar)
            {
                ui_NameHPBar.Init();
                ui_NameHPBar.SetInfo(info);
            }
        }
        else if(objectType == GameObjectType.Item) 
        {
            GameObject go = null;
            if (info.ItemId == 0)
            {
                go = Managers.Resource.Instantiate("Item/GoldCoin");
                go.GetComponent<ItemObject>().GetComponent<ItemObject>().Money = info.Money;
            }
            if (info.ItemId == 1)
            {
                go = Managers.Resource.Instantiate("Item/Item_HpPotion");
            }
            else if (info.ItemId == 2)
            {
                go = Managers.Resource.Instantiate("Item/Item_ManaPotion");
            }
            _objects.Add(info.ObjectId, go);
            go.GetComponent<ItemObject>().GetComponent<ItemObject>().ObjectId = info.ObjectId;
            go.transform.position = new Vector3(info.PosInfo.PosX, 0, info.PosInfo.PosZ);
        }
        else if(objectType == GameObjectType.MonsterBoss)
        {
            GameObject go = Managers.Resource.Instantiate("Creature/Boss_Dragon");
            _objects.Add(info.ObjectId, go);

            CreatureController cc = go.GetComponent<CreatureController>();
            cc.Id = info.ObjectId;
            cc.ObjectInfo = info;

            go.transform.position = new Vector3(info.PosInfo.PosX, 0, info.PosInfo.PosZ);
            go.transform.rotation = Quaternion.identity;
        }
    }

    public void Remove(int id)
    {
        GameObject go = FindById(id);
        if (go == null)
            return;

        _objects.Remove(id);
        Managers.Resource.Destroy(go);
    }

    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public void Clear()
    {
        foreach (GameObject obj in _objects.Values)
            Managers.Resource.Destroy(obj);
        _objects.Clear();
        MyPlayer = null;
    }
}
