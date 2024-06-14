using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : BaseController
{
    [SerializeField] public CreatureMovement Movement;
    protected Coroutine _skillTimeCoroutine = null;
    protected CreatureState _state;
    public CreatureState creatureState
    {
        get { return _state; }
        set
        {
            if (_state == value && 
                ((_state != CreatureState.Skill) && (_state != CreatureState.Gethit))
                ) return;
            _state = value;
            UpdateAnimation();
        }
    }
    protected int _currentSkillId;
    public int CurrentSkill
    {
        get
        {
            return _currentSkillId;
        }
        set
        {
            CalculateSKillTime(value);
        }
    }
    protected override void UpdateController()
    {
        base.UpdateController();
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateAnimation() { }
    protected virtual void CalculateSKillTime(int skillId) { }
}
