using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip _hitSound;
    protected override void Init()
    {
        base.Init();
    }
    protected override void UpdateController()
    {
        base.UpdateController();
        switch (creatureState)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Search:
                UpdateSearch();
                break;
            case CreatureState.Trace:
                UpdateTrace();
                break;
        }
    }
    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
        switch (creatureState)
        {
            case CreatureState.Idle:
                //Movement.CharacterMesh.GetComponent<Animator>().CrossFade("Idle", 0.001f);
                Movement.CharacterMesh.GetComponent<Animator>().SetTrigger("Idle");
                break;
            case CreatureState.Skill:
                //Movement.CharacterMesh.GetComponent<Animator>().CrossFade("Attack", 0.001f, -1, 0f);
                Movement.CharacterMesh.GetComponent<Animator>().SetTrigger("Attack");
                Movement.ServerWorldDirVector = Vector3.zero;
                break;
            case CreatureState.Search:
                Movement.CharacterMesh.GetComponent<Animator>().SetTrigger("Walk");
                break;
            case CreatureState.Trace:
                Movement.CharacterMesh.GetComponent<Animator>().SetTrigger("Walk");
                break;
            case CreatureState.Gethit:
                Movement.CharacterMesh.GetComponent<Animator>().SetTrigger("GetHit");
                Movement.ServerWorldDirVector = Vector3.zero;
                GameObject effect = Managers.Resource.Instantiate("Effect/Hit2");
                effect.transform.position = transform.position + Vector3.up;
                _audio.PlayOneShot(_hitSound);
                break;
            case CreatureState.Dead:
                Movement.CharacterMesh.GetComponent<Animator>().SetTrigger("Die");
                Movement.ServerWorldDirVector = Vector3.zero;
                GetComponentInChildren<UI_NameHPBar>().gameObject.SetActive(false);
                break;
        }
    }
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }
    protected override void UpdateSkill()
    {
        base.UpdateSkill();
    }
    protected void UpdateSearch()
    {
    }
    protected void UpdateTrace()
    {
    }
}
