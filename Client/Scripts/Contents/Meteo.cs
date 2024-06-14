using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteo : MonoBehaviour
{
    Vector3 _targetPos = Vector3.zero;
    Vector3 _startPos = Vector3.zero;
    Vector3 _direction = Vector3.zero;
    float _distance;
    float _time = 3f;
    private void FixedUpdate()
    {
        transform.LookAt(_targetPos);
        transform.position = transform.position + _direction * Time.deltaTime * 8;
    }
    public void SetPos(PositionInfo targetPos)
    {
        _targetPos.x = targetPos.PosX;
        _targetPos.y = 0.01f;
        _targetPos.z = targetPos.PosZ;

        _startPos = _targetPos + 10 * (Vector3.left + Vector3.forward + Vector3.up);

        transform.position = _startPos;

        _direction = (_targetPos - _startPos).normalized;
        _distance = (_targetPos - _startPos).magnitude;

        GameObject go = Managers.Resource.Instantiate("RangeWarning");
        go.transform.position = _targetPos;
        go.GetComponent<RangeWarning>().SetInfo(2f);

        Destroy(gameObject, _time);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HeroPlayerController>() == null) return;
        C_DamagedDragonAttack damagedPacket = new C_DamagedDragonAttack();
        damagedPacket.SkillType = SkillType.Skill2;
        Managers.Network.Send(damagedPacket);
    }
}
