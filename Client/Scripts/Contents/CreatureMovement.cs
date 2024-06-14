using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMovement : MonoBehaviour
{
    public CreatureController Controller;
    [SerializeField] public GameObject CharacterMesh;
    public Vector3 ServerWorldDirVector { get; set; } = Vector3.zero;
    private void Start()
    {
        Init();
    }
    private void Update()
    {
        UpdateMovement();
    }
    protected virtual void Init() { }
    protected virtual void UpdateMovement()
    {
        transform.Translate(transform.InverseTransformDirection(ServerWorldDirVector) * Time.deltaTime);
    }
    public virtual void SetDirectionAndLookRotation(PositionInfo targetPos, PQuaternion lookRotation)
    {
        if (Managers.Data.SkillDict[Controller.CurrentSkill].skillType == SkillType.Dash) return;
        ServerWorldDirVector = new Vector3(
            targetPos.PosX - transform.position.x,
            targetPos.PosY - transform.position.y,
            targetPos.PosZ - transform.position.z
            );
    }
}
