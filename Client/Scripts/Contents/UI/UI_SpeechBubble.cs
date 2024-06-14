using DG.Tweening;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SpeechBubble : UI_Base
{
    enum Texts
    {
        Text_Chat
    }
    private Tween _process = null;
    private Coroutine _coroutine = null;
    private Camera _camera = null;
    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        transform.localScale = Vector3.zero;
        _camera = Camera.main;
    }
    private void FixedUpdate()
    {
        if (_camera)
        {
            transform.rotation = _camera.transform.rotation;
        }
    }
    public void ShowText(string chat, ChatType chatType)
    {
        if (_process != null) _process.Kill();
        if (_coroutine != null) StopCoroutine(_coroutine);
        Get<TextMeshProUGUI>((int)Texts.Text_Chat).text = chat;
        if (chatType==ChatType.ChatParty)
        {
            Get<TextMeshProUGUI>((int)Texts.Text_Chat).color = Color.magenta;
        }
        else
        {
            Get<TextMeshProUGUI>((int)Texts.Text_Chat).color = Color.black;
        }
        _process = transform.DOScale(Vector3.one, 0.5f);
        _coroutine = StartCoroutine(CoHideText());
    }

    private void HideText()
    {
        if (_process != null) _process.Kill();
        Get<TextMeshProUGUI>((int)Texts.Text_Chat).text = string.Empty;
        _process = transform.DOScale(Vector3.zero, 0.5f);
        _coroutine = null;
    }

    IEnumerator CoHideText()
    {
        yield return new WaitForSeconds(3f);
        HideText();
    }
}
