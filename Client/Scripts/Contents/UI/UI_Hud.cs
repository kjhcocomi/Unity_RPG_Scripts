using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Hud : UI_Scene
{
    enum Images
    {
        Image_Healthbar,
        Image_Manabar,
        Image_ExpBar,
        ScrollView_ChatBox,

        Image_ConsumeItem1,
        Image_ConsumeItem2,
        Image_Skill1,
        Image_Skill1Cooldown,
        Image_Skill2,
        Image_Skill2Cooldown,

        Image_DashCoolDown,
        Image_BasicAttack
    }
    enum Texts
    {
        Text_Name,
        Text_Level,
        Text_Chat,

        Text_ConsumeItem1,
        Text_ConsumeItem2,
    }
    enum Buttons
    {
        Button_ControllSize,

        Button_RoomTab,
        Button_PartyTab,
    }
    enum InputFields
    {
        InputField_Chat
    }
    enum ScrollBars
    {
        Scrollbar_Chat
    }
    enum GameObjects
    {
        Content,
        PartyRegion
    }
    private bool _isMaximizationChatBox = true;
    private bool _lastTickFocus = false;
    [SerializeField] private Sprite _maximizeIcon;
    [SerializeField] private Sprite _minimizeIcon;
    [SerializeField] private Sprite _emptySprite;

    [SerializeField] private Sprite _attack1Sprite;
    [SerializeField] private Sprite _attack2Sprite;
    [SerializeField] private Sprite _attack3Sprite;
    [SerializeField] private Sprite _attack4Sprite;

    [SerializeField] private Sprite _selectedTabSprite;
    [SerializeField] private Sprite _deSelectedTabSprite;

    private Dictionary<int, UI_PartyMemberHp> _partyMemberHps = new Dictionary<int, UI_PartyMemberHp>();
    private List<UI_ChatElement> _chatElements = new List<UI_ChatElement>();
    private ChatType _currentChatType;
    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputFields));
        Bind<Scrollbar>(typeof(ScrollBars));
        Bind<GameObject>(typeof(GameObjects));

        Get<Button>((int)Buttons.Button_ControllSize).gameObject.BindEvent(PushControllSizeButton);
        Get<Button>((int)Buttons.Button_RoomTab).gameObject.BindEvent(PushRoomChatTab);
        Get<Button>((int)Buttons.Button_PartyTab).gameObject.BindEvent(PushPartyChatTab);
        Get<TMP_InputField>((int)InputFields.InputField_Chat).onSubmit.AddListener(EnterChat);

        RefreshItemSlot();
        RefreshSkillSlot();
        PushRoomChatTab(null);

        transform.SetParent(null);
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (_lastTickFocus == false)
            {
                Get<TMP_InputField>((int)InputFields.InputField_Chat).Select();
                Managers.Object.MyPlayer.BlockInput = true;
            }
            _lastTickFocus = Get<TMP_InputField>((int)InputFields.InputField_Chat).isFocused;
        }
    }
    public void SetInfo(ObjectInfo info)
    {
        StatInfo baseStat = info.BaseStat;
        StatInfo additionalStat = info.AdditionalStat;

        SetLevel(baseStat.Level);
        Get<TextMeshProUGUI>((int)Texts.Text_Name).text = info.Name;

        RefreshHud(baseStat, additionalStat);
    }
    public void RefreshHud(StatInfo baseStat, StatInfo additionalStat)
    {
        int hp = baseStat.Hp;
        int maxHp = baseStat.MaxHp + additionalStat.MaxHp;
        int mana = baseStat.Mana;
        int maxMana = baseStat.MaxMana + additionalStat.MaxMana;
        float exp = baseStat.Exp;
        int maxExp = baseStat.MaxExp;

        SetHpBar((float)hp / maxHp);
        SetManaBar((float)mana / maxMana);
        SetExpBar((float)exp / maxExp);
    }

    public void RefreshPartyInfo()
    {
        _partyMemberHps.Clear();
        GameObject partyRegion = Get<GameObject>((int)GameObjects.PartyRegion);
        foreach (Transform child in partyRegion.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }

        int partyOwnerId = Managers.Party.OwnerId;
        if (partyOwnerId == -1) return;

        {
            UI_PartyMemberHp partyMemberHp = Managers.UI.MakeSubItem<UI_PartyMemberHp>(partyRegion.transform);
            partyMemberHp.Init();
            PlayerController pc = Managers.Object.FindById(partyOwnerId).GetComponent<PlayerController>();
            partyMemberHp.SetInfo(pc.ObjectInfo, true);

            _partyMemberHps.Add(partyOwnerId, partyMemberHp);
        }
        {
            foreach(int memberId in Managers.Party.MemberIds)
            {
                UI_PartyMemberHp partyMemberHp = Managers.UI.MakeSubItem<UI_PartyMemberHp>(partyRegion.transform);
                partyMemberHp.Init();
                PlayerController pc = Managers.Object.FindById(memberId).GetComponent<PlayerController>();
                partyMemberHp.SetInfo(pc.ObjectInfo, false);

                _partyMemberHps.Add(memberId, partyMemberHp);
            }
        }
    }
    public void RefreshPartyMemberHp(int memberId)
    {
        UI_PartyMemberHp partyMemberHp = null;
        if (_partyMemberHps.TryGetValue(memberId, out partyMemberHp))
        {
            PlayerController pc = Managers.Object.FindById(memberId).GetComponent<PlayerController>();
            partyMemberHp.SetHp(pc.ObjectInfo);
        }
    }
    public void SetLevel(int level)
    {
        Get<TextMeshProUGUI>((int)Texts.Text_Level).text = level.ToString();
    }
    public void SetHpBar(float value)
    {
        Get<Image>((int)Images.Image_Healthbar).fillAmount = value;
    }
    public void SetManaBar(float value)
    {
        Get<Image>((int)Images.Image_Manabar).fillAmount = value;
    }
    public void SetExpBar(float value)
    {
        Get<Image>((int)Images.Image_ExpBar).fillAmount = value;
    }

    #region Chat
    public void EnterChat(string chat)
    {
        if (string.IsNullOrEmpty(chat))
        {
            _lastTickFocus = true;
            Managers.Object.MyPlayer.BlockInput = false;
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }
        Get<TMP_InputField>((int)InputFields.InputField_Chat).text = string.Empty;
        _lastTickFocus = true;
        Managers.Object.MyPlayer.BlockInput = false;
        EventSystem.current.SetSelectedGameObject(null);

        if (_currentChatType == ChatType.ChatParty)
        {
            if (Managers.Party.OwnerId == -1)
            {
                MakeChatElement("파티가 존재하지 않습니다.", _currentChatType, null);
                return;
            }
        }

        C_Chat cChatPacket = new C_Chat();
        cChatPacket.ChatType = _currentChatType;
        cChatPacket.Chat = chat;
        Managers.Network.Send(cChatPacket);
    }

    public void MakeChatElement(string chat, ChatType chatType, string name)
    {
        UI_ChatElement chatElement = Managers.UI.MakeSubItem<UI_ChatElement>(Get<GameObject>((int)GameObjects.Content).transform);
        chatElement.SetText(chat, chatType, name);
        Get<Scrollbar>((int)ScrollBars.Scrollbar_Chat).value = 0;

        if (chatType == ChatType.ChatRoom && _currentChatType == ChatType.ChatParty)
        {
            chatElement.gameObject.SetActive(false);
        }

        _chatElements.Add(chatElement);
    }

    private void PushControllSizeButton(PointerEventData data)
    {
        if(_isMaximizationChatBox)
        {
            Get<Image>((int)Images.ScrollView_ChatBox).rectTransform.sizeDelta = new Vector2(500, 60);
            Get<Button>((int)Buttons.Button_ControllSize).image.sprite = _maximizeIcon;
        }
        else
        {
            Get<Image>((int)Images.ScrollView_ChatBox).rectTransform.sizeDelta = new Vector2(500, 250);
            Get<Button>((int)Buttons.Button_ControllSize).image.sprite = _minimizeIcon;
        }
        EventSystem.current.SetSelectedGameObject(null);
        Get<Scrollbar>((int)ScrollBars.Scrollbar_Chat).value = 0;
        _isMaximizationChatBox = !_isMaximizationChatBox;
    }

    private void PushRoomChatTab(PointerEventData data)
    {
        Get<Button>((int)Buttons.Button_RoomTab).image.sprite = _selectedTabSprite;
        Get<Button>((int)Buttons.Button_PartyTab).image.sprite = _deSelectedTabSprite;

        _currentChatType = ChatType.ChatRoom;
        foreach (UI_ChatElement chatElement in _chatElements)
        {
            chatElement.gameObject.SetActive(true);
        }
    }
    private void PushPartyChatTab(PointerEventData data)
    {
        Get<Button>((int)Buttons.Button_RoomTab).image.sprite = _deSelectedTabSprite;
        Get<Button>((int)Buttons.Button_PartyTab).image.sprite = _selectedTabSprite;

        _currentChatType = ChatType.ChatParty;
        foreach (UI_ChatElement chatElement in _chatElements)
        {
            if(chatElement.ChatType == ChatType.ChatRoom)
                chatElement.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Slot
    public void RefreshItemSlot()
    {
        int slot1ItemId = -1;
        int slot2ItemId = -1;
        if (Managers.Data.ConsumeItemQuickSlot.TryGetValue('1', out slot1ItemId))
        {
            Sprite icon = Resources.Load<Sprite>(Managers.Data.ItemDict[slot1ItemId].iconPath);
            Get<Image>((int)Images.Image_ConsumeItem1).sprite = icon;
            Get<TextMeshProUGUI>((int)Texts.Text_ConsumeItem1).text = Managers.Data.ConsumeItemInfo[slot1ItemId].ToString();
        }
        else
        {
            Get<Image>((int)Images.Image_ConsumeItem1).sprite = _emptySprite;
            Get<TextMeshProUGUI>((int)Texts.Text_ConsumeItem1).text = string.Empty;
        }

        if (Managers.Data.ConsumeItemQuickSlot.TryGetValue('2', out slot2ItemId))
        {
            Sprite icon = Resources.Load<Sprite>(Managers.Data.ItemDict[slot2ItemId].iconPath);
            Get<Image>((int)Images.Image_ConsumeItem2).sprite = icon;
            Get<TextMeshProUGUI>((int)Texts.Text_ConsumeItem2).text = Managers.Data.ConsumeItemInfo[slot2ItemId].ToString();
        }
        else
        {
            Get<Image>((int)Images.Image_ConsumeItem2).sprite = _emptySprite;
            Get<TextMeshProUGUI>((int)Texts.Text_ConsumeItem2).text = string.Empty;
        }
    }
    public void RefreshSkillSlot()
    {
        int slot1SkillId = -1;
        int slot2SkillId = -1;
        if (Managers.Data.SkillQuickSlot.TryGetValue('q', out slot1SkillId))
        {
            Sprite icon = Resources.Load<Sprite>(Managers.Data.SkillDict[slot1SkillId].iconPath);
            Get<Image>((int)Images.Image_Skill1).sprite = icon;
        }
        else
        {
            Get<Image>((int)Images.Image_Skill1).sprite = _emptySprite;
        }
        if (Managers.Data.SkillQuickSlot.TryGetValue('e', out slot2SkillId))
        {
            Sprite icon = Resources.Load<Sprite>(Managers.Data.SkillDict[slot2SkillId].iconPath);
            Get<Image>((int)Images.Image_Skill2).sprite = icon;
        }
        else
        {
            Get<Image>((int)Images.Image_Skill2).sprite = _emptySprite;
        }
    }
    public void SetDashValue(float value)
    {
        Get<Image>((int)Images.Image_DashCoolDown).fillAmount = value;
    }
    public void SetSkillCoolDown(float value, int skillSlotNum)
    {
        if(skillSlotNum == 1)
        {
            Get<Image>((int)Images.Image_Skill1Cooldown).fillAmount = value;
        }
        else if(skillSlotNum == 2)
        {
            Get<Image>((int)Images.Image_Skill2Cooldown).fillAmount = value;
        }
    }
    public void SetBasicSkill(int num)
    {
        if(num==0)
        {
            Get<Image>((int)Images.Image_BasicAttack).sprite = _attack1Sprite;
        }
        else if(num==1)
        {
            Get<Image>((int)Images.Image_BasicAttack).sprite = _attack2Sprite;
        }
        else if(num==2)
        {
            Get<Image>((int)Images.Image_BasicAttack).sprite = _attack3Sprite;
        }
        else if(num==3)
        {
            Get<Image>((int)Images.Image_BasicAttack).sprite = _attack4Sprite;
        }
    }
    #endregion
}
