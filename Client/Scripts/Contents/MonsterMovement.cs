using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : CreatureMovement
{
    private Quaternion _lookRotation;
    protected override void UpdateMovement()
    {
        base.UpdateMovement();
        CharacterMesh.transform.rotation = Quaternion.Lerp(CharacterMesh.transform.rotation, _lookRotation, Time.deltaTime * 10).normalized;
    }
    public override void SetDirectionAndLookRotation(PositionInfo targetPos, PQuaternion lookRotation)
    {
        base.SetDirectionAndLookRotation(targetPos, lookRotation);
        _lookRotation = Quaternion.LookRotation(ServerWorldDirVector);
    }
}
