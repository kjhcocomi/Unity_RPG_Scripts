using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWarning : MonoBehaviour
{
    [SerializeField] private GameObject _rangeWarning;
    public void SetInfo(float time)
    {
        _rangeWarning.transform.DOScale(Vector3.one, time);
        Destroy(gameObject, time);
    }
}
