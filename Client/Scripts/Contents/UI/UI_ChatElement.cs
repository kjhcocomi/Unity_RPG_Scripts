using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ChatElement : UI_Base
{
    [SerializeField] private TextMeshProUGUI _text;
    public override void Init()
    {
    }
    public ChatType ChatType { get; private set; }
    public void SetText(string text, ChatType chatType, string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            _text.text = text;
        }
        else
        {
            _text.text = $"{name} : {text}";
        }

        if(chatType == ChatType.ChatParty)
        {
            _text.color = Color.magenta;
        }
        
        GetComponent<RectTransform>().localScale = Vector3.one;

        ChatType = chatType;
    }
}
