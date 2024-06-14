using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CreatureController
{
    protected bool _reserveNextSkill = false;
    [SerializeField] private ParticleSystem _attackTrail;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _attack12;
    [SerializeField] private AudioClip _attack3;
    [SerializeField] private AudioClip _attack4;
    [SerializeField] private AudioClip _dash;
    [SerializeField] private AudioClip _skill1;
    [SerializeField] private AudioClip _skill2;
    [SerializeField] private AudioClip _levelUp;
    protected override void UpdateController()
    {
        base.UpdateController();
        switch (creatureState)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
        }
    }
    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
        if(_state == CreatureState.Idle || _state == CreatureState.Moving)
        {
            _attackTrail.Stop();
        }
        else if(_state == CreatureState.Skill)
        {
            if(_currentSkillId != 7 && _currentSkillId != 0)
            {
                _attackTrail.Play();
                if(_currentSkillId == 1 || _currentSkillId == 2)
                {
                    StartCoroutine(CoPlaySound(_attack12, 0.2f));
                }
                else if(_currentSkillId == 3)
                {
                    StartCoroutine(CoPlaySound(_attack3, 0.3f));
                }
                else if (_currentSkillId == 4)
                {
                    StartCoroutine(CoPlaySound(_attack4, 0.2f));
                }
                else if (_currentSkillId == 5)
                {
                    StartCoroutine(CoPlaySound(_skill1, 1.4f));
                }
                else if (_currentSkillId == 6)
                {
                    StartCoroutine(CoPlaySound(_skill2, 0.8f));
                }
            }
            else if (_currentSkillId == 7)
            {
                StartCoroutine(CoPlaySound(_dash, 0));
            }
        }
    }
    IEnumerator CoPlaySound(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        _audioSource.PlayOneShot(clip);
    }
    public void PlayLevelUpSound()
    {
        _audioSource.PlayOneShot(_levelUp);
    }
    public virtual void Die()
    {
        _state = CreatureState.Dead;
        Movement.CharacterMesh.GetComponent<Animator>().SetTrigger("Die");
    }
    public virtual void Respawn(StatInfo stat, PositionInfo posInfo)
    {
        ObjectInfo.BaseStat = stat;
        _state = CreatureState.Idle;
        Movement.CharacterMesh.GetComponent<Animator>().SetTrigger("Respawn");
        UI_NameHPBar ui_NameHPBar = gameObject.GetComponentInChildren<UI_NameHPBar>();
        if (ui_NameHPBar)
        {
            ui_NameHPBar.SetHp(stat);
        }
        transform.position = new Vector3(posInfo.PosX, posInfo.PosY, posInfo.PosZ);
    }
    protected override void Init()
    {
        base.Init();
        Movement = GetComponent<PlayerMovement>();
        if (Movement)
        {
            Movement.Controller = this;
        }
        creatureState = CreatureState.Idle;

        UI_Hud hud = Managers.UI.SceneUI as UI_Hud;
        if (hud)
        {
            hud.RefreshPartyInfo();
        }
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
        if (Movement.ServerWorldDirVector.magnitude > 0.01f)
        {
            creatureState = CreatureState.Moving;
        }
        // test
        //Movement.CharacterMesh.transform.localScale = Vector3.one;
    }
    protected override void UpdateMoving()
    {
        if (Movement.ServerWorldDirVector.magnitude < 0.01f)
        {
            creatureState = CreatureState.Idle;
        }
        // test
        //Movement.CharacterMesh.transform.localScale = Vector3.one / 2f;
    }

    protected override void UpdateSkill()
    {
        base.UpdateSkill();
        switch (Managers.Data.SkillDict[_currentSkillId].skillType)
        {
            case SkillType.SkillNone:
                break;
            case SkillType.SkillBasicAttack1:
                break;
            case SkillType.SkillBasicAttack2:
                break;
            case SkillType.SkillBasicAttack3:
                break;
            case SkillType.SkillBasicAttack4:
                break;
            case SkillType.Skill1:
                break;
        }
    }
    protected override void CalculateSKillTime(int skillId)
    {
        base.CalculateSKillTime(skillId);
        
        if (_skillTimeCoroutine == null)
        {
            _skillTimeCoroutine = StartCoroutine(CoSkillTimer(skillId));
            if(Managers.Object.MyPlayer == this)
            {
                if (Managers.Data.SkillDict[_currentSkillId].skillType == SkillType.Dash)
                {
                    Managers.Object.MyPlayer.CanUseDash = false;
                    StartCoroutine(CoSKillCollDown(skillId));
                    (Managers.UI.SceneUI as UI_Hud).SetDashValue(1);
                }
                else if (skillId == 5 || skillId == 6)
                {
                    if (skillId == 5) Managers.Object.MyPlayer.CanUseSkill5 = false;
                    else if (skillId == 6) Managers.Object.MyPlayer.CanUseSkill6 = false;

                    int slot = -1;
                    foreach(var s in Managers.Data.SkillQuickSlot)
                    {
                        if(s.Value == skillId)
                        {
                            if (s.Key == 'q') slot = 1;
                            else if (s.Key == 'e') slot = 2;
                        }
                    }
                    StartCoroutine(CoSKillCollDown(skillId, slot));
                    (Managers.UI.SceneUI as UI_Hud).SetSkillCoolDown(1, slot);
                }
            }
        }
        else
        {
            SkillType currentskillType = Managers.Data.SkillDict[_currentSkillId].skillType;
            SkillType reserveskillType = Managers.Data.SkillDict[skillId].skillType;
            if ((currentskillType == SkillType.SkillBasicAttack1 && reserveskillType == SkillType.SkillBasicAttack2) || 
                (currentskillType == SkillType.SkillBasicAttack2 && reserveskillType == SkillType.SkillBasicAttack3) ||
                 currentskillType == SkillType.SkillBasicAttack3 && reserveskillType == SkillType.SkillBasicAttack4)
            {
                _reserveNextSkill = true;
            }
        }
    }
    IEnumerator CoSkillTimer(int skillId)
    {
        _currentSkillId = skillId;
        if (Managers.Object.MyPlayer == this) (Managers.UI.SceneUI as UI_Hud).SetBasicSkill(_currentSkillId);
        Movement.CharacterMesh.GetComponent<Animator>().SetInteger("skillId", _currentSkillId);
        creatureState = CreatureState.Skill;
        float skillTime = Managers.Data.SkillDict[_currentSkillId].skillTime;
        yield return new WaitForSeconds(skillTime);
        _skillTimeCoroutine = null;
        ChangeState(skillId);
    }
    IEnumerator CoSKillCollDown(int skillId, int slot = 0)
    {
        int skillCollDown = (int)Managers.Data.SkillDict[skillId].cooldown;
        for (int i=0;i< skillCollDown * 20; i++)
        {
            if ((Managers.UI.SceneUI as UI_Hud) == null) yield break;
            float value = 1 - (i / (float)(skillCollDown * 20));
            if (skillId == 7)
            {
                (Managers.UI.SceneUI as UI_Hud).SetDashValue(value);
            }
            else
            {
                (Managers.UI.SceneUI as UI_Hud).SetSkillCoolDown(value, slot);
            }
            yield return new WaitForSeconds(0.05f);
        }
        if (skillId == 5)
        {
            (Managers.UI.SceneUI as UI_Hud).SetSkillCoolDown(0, slot);
            Managers.Object.MyPlayer.CanUseSkill5 = true;
        }
        else if (skillId == 6)
        {
            (Managers.UI.SceneUI as UI_Hud).SetSkillCoolDown(0, slot);
            Managers.Object.MyPlayer.CanUseSkill6 = true;
        }
        else if (skillId == 7)
        {
            (Managers.UI.SceneUI as UI_Hud).SetDashValue(0);
            Managers.Object.MyPlayer.CanUseDash = true;
        }
    }
    protected void ChangeState(int skillId)
    {
        if (_state == CreatureState.Dead) return;
        if (_reserveNextSkill)
        {
            SkillType skillType = Managers.Data.SkillDict[_currentSkillId].skillType;
            if (skillType == SkillType.SkillBasicAttack1 || skillType == SkillType.SkillBasicAttack2 || skillType == SkillType.SkillBasicAttack3)
            {
                CurrentSkill++;
            }
            _reserveNextSkill = false;
        }
        else
        {
            creatureState = CreatureState.Idle;
            _currentSkillId = 0;
            if (Managers.Object.MyPlayer == this) (Managers.UI.SceneUI as UI_Hud).SetBasicSkill(_currentSkillId);
            Movement.CharacterMesh.GetComponent<Animator>().SetInteger("skillId", _currentSkillId);
        }
    }
}
