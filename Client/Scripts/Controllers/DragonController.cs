using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : CreatureController
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private Animator _anim;
    [SerializeField] private ParticleSystem _flameParticle;
    private SkillType _skillState;
    public SkillType SkillState
    {
        get { return _skillState; }
        set
        {
            _skillState = value;
            UpdateSkillState();
        }
    }
    public int TargetId { get; set; } = -1;
    UI_BossHp _bossHp = null;
    protected override void Init()
    {
        base.Init();
        _bossHp = Managers.Resource.Instantiate("UI/UI_BossHp").GetComponent<UI_BossHp>();
        _bossHp.Init();
        _bossHp.RefreshInfo(ObjectInfo.BaseStat);
    }
    protected override void UpdateController()
    {
        base.UpdateController();
        if (_state == CreatureState.Idle)
        {
            if (TargetId != -1)
            {
                GameObject target = Managers.Object.FindById(TargetId);
                if (target)
                {
                    Vector3 dir = target.transform.position - transform.position;
                    Quaternion lookRotation = Quaternion.LookRotation(-dir);
                    transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime);
                }
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime);
            }
        }
    }
    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
        switch (creatureState)
        {
            case CreatureState.Idle:
                _anim.SetTrigger("Idle");
                break;
            case CreatureState.Skill:
                break;
            case CreatureState.Stun:
                _anim.SetTrigger("Stun");
                if (_flameParticle.isPlaying) _flameParticle.Stop();
                break;
            case CreatureState.Dead:
                _anim.SetTrigger("Die");
                break;
        }
    }
    private void UpdateSkillState()
    {
        switch (_skillState)
        {
            case SkillType.SkillBasicAttack1:
                _anim.SetTrigger("Attack1");
                break;
            case SkillType.SkillBasicAttack2:
                _anim.SetTrigger("Attack2");
                break;
            case SkillType.Skill1:
                _anim.SetTrigger("Skill1");
                break;
            case SkillType.Skill2:
                _anim.SetTrigger("Skill2");
                break;
        }
    }
    public void PlayHittedEffect()
    {
        GameObject effect = Managers.Resource.Instantiate("Effect/Hit2");
        effect.transform.position = transform.position + Vector3.up * 3;
        _audio.PlayOneShot(_hitSound);
    }
    public void RefreshBossUI()
    {
        _bossHp.RefreshInfo(ObjectInfo.BaseStat);
    }
    private void OnDestroy()
    {
        if(_bossHp)
        {
            Destroy(_bossHp.gameObject);
        }
    }
}
