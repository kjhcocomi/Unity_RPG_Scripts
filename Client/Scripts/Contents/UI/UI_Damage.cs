using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Damage : UI_Base
{
    enum Texts
    {
        Text_Damage
    }
    Color _nonCriticalColor = new Color(250 / 255f, 225 / 255f, 120 / 255f, 1);
    Color _criticalColor = new Color(230 / 255f, 50 / 255f, 80 / 255f, 1);
    float _destroyTime = 3f;
    Camera _camera;
    bool _init = false;
    public override void Init()
    {
        if (_init) return;
        _init = true;
        Bind<TextMeshProUGUI>(typeof(Texts));
        _camera = Camera.main;
    }
    private void FixedUpdate()
    {
        if(_camera)
        {
            transform.rotation = _camera.transform.rotation;
        }
    }
    public void SetDamage(int damage, bool isCritical)
    {
        if (!isCritical)
        {
            Get<TextMeshProUGUI>((int)Texts.Text_Damage).color = _nonCriticalColor;
        }
        else
        {
            Get<TextMeshProUGUI>((int)Texts.Text_Damage).color = _criticalColor;
            GetComponent<RectTransform>().localScale = Vector3.one * 1.5f;
        }

        Get<TextMeshProUGUI>((int)Texts.Text_Damage).text = damage.ToString();

        transform.DOMove(transform.position + Vector3.up, _destroyTime);
        transform.DOScale(Vector3.zero, _destroyTime);
        Get<TextMeshProUGUI>((int)Texts.Text_Damage).DOFade(0, _destroyTime);
        StartCoroutine(CoDestroy());
    }
    IEnumerator CoDestroy()
    {
        yield return new WaitForSeconds(_destroyTime);
        Destroy(this.gameObject);
    }
}
