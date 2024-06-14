using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSkillCheck : MonoBehaviour
{
    [SerializeField] private ParticleSystem _flameParticle;
    [SerializeField] private GameObject _dragon;
    [SerializeField] private GameObject _dragonHead;
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioClip _dragonBreath;
    [SerializeField] private AudioClip _dragonScream;

    Vector3 boxSize;
    Vector3 center;
    RaycastHit[] hits;
    int _layerMask;
    private void Start()
    {
        _layerMask = LayerMask.GetMask("Player");
    }
    public void Attack1()
    {
        float rangeX = 1.5f;
        float rangeZ = 8;
        float offset = 4;

        CheckSkillRange(rangeX, rangeZ, offset, SkillType.SkillBasicAttack1);
    }
    public void Attack2()
    {
        float rangeX = 3f;
        float rangeZ = 13;
        float offset = 6.5f;

        CheckSkillRange(rangeX, rangeZ, offset, SkillType.SkillBasicAttack2);
    }
    public void Skill1()
    {
        float rangeX = 3f;
        float rangeZ = 13;
        float offset = 6.5f;

        CheckSkillRange(rangeX, rangeZ, offset, SkillType.Skill1);
    }
    public void Skill1_Flag(int flag)
    {
        if (flag == 1)
        {
            _flameParticle.Play();
            _audioSource.PlayOneShot(_dragonBreath);
        }
        else if (flag == -1)
        {
            _flameParticle.Stop();
        }
    }
    public void Skill2()
    {
        _audioSource.PlayOneShot(_dragonScream);
    }
    private void CheckSkillRange(float rangeX, float rangeZ, float offset, SkillType skillType)
    {
        if(skillType == SkillType.Skill1)
        {
            boxSize = new Vector3(rangeX, 10, rangeZ);
            center = _dragonHead.transform.position + _dragonHead.transform.forward * offset + Vector3.up * 10;
            hits = Physics.BoxCastAll(center, boxSize / 2, Vector3.down, _dragonHead.transform.rotation, 10, _layerMask);

        }
        else
        {
            boxSize = new Vector3(rangeX, 2, rangeZ);
            center = _dragon.transform.position + (-_dragon.transform.forward) * offset + Vector3.up * 3;
            hits = Physics.BoxCastAll(center, boxSize / 2, Vector3.down, _dragon.transform.rotation, 3, _layerMask);
        }

        if(hits != null)
        {
            SendDamagedPacket(skillType);
        }
    }
    private void SendDamagedPacket(SkillType skillType)
    {
        HeroPlayerController player = null;
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.GetComponent<HeroPlayerController>())
            {
                player = hit.transform.gameObject.GetComponent<HeroPlayerController>();
                break;
            }
        }
        if (player)
        {
            C_DamagedDragonAttack damagedDragonAttackPacket = new C_DamagedDragonAttack();
            damagedDragonAttackPacket.SkillType = skillType;
            Managers.Network.Send(damagedDragonAttackPacket);
        }
    }
    private void OnDrawGizmos()
    {
        if (hits != null)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(center, Vector3.down * hits[i].distance);

                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(center + Vector3.down * hits[i].distance, boxSize);
            }
        }

    }
}
