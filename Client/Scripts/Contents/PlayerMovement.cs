using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CreatureMovement
{
    public Quaternion _serverLookRotation = Quaternion.identity;
    public Vector3 CWorldDirVector { get; set; }
    public Quaternion CLookRotation { get; set; }

    private Vector3 _localVector = Vector3.zero;
    protected override void Init()
    {
        base.Init();
        if (Controller as HeroPlayerController)
        {
            StartCoroutine(CoSendMovePacket());
        }
    }
    protected override void UpdateMovement()
    {
        base.UpdateMovement();
        CharacterMesh.transform.rotation = Quaternion.Lerp(CharacterMesh.transform.rotation, _serverLookRotation, Time.deltaTime * 10).normalized;

        // tmp

        if(ServerWorldDirVector.magnitude<0.01f)
        {
            _localVector = Vector3.Lerp(_localVector, Vector3.zero, Time.deltaTime * 10);
        }
        else
        {
            Quaternion inverseQuaternion = Quaternion.Inverse(_serverLookRotation);
            _localVector = Vector3.Lerp(_localVector, (inverseQuaternion * ServerWorldDirVector).normalized, Time.deltaTime * 10);
        }

        CharacterMesh.GetComponent<Animator>().SetFloat("dirX", _localVector.x);
        CharacterMesh.GetComponent<Animator>().SetFloat("dirY", _localVector.z);
    }
    public override void SetDirectionAndLookRotation(PositionInfo targetPos, PQuaternion lookRotation)
    {
        base.SetDirectionAndLookRotation(targetPos, lookRotation);

        if (lookRotation != null)
        {
            _serverLookRotation.x = lookRotation.X;
            _serverLookRotation.y = lookRotation.Y;
            _serverLookRotation.z = lookRotation.Z;
            _serverLookRotation.w = lookRotation.W;
        }
    }
    IEnumerator CoSendMovePacket()
    {
        while(true)
        {
            C_Move movePacket = new C_Move();

            PositionInfo dir = new PositionInfo();
            PQuaternion lookRotation = new PQuaternion();
            PositionInfo pos = new PositionInfo();

            dir.PosX = CWorldDirVector.x;
            dir.PosY = CWorldDirVector.y;
            dir.PosZ = CWorldDirVector.z;

            lookRotation.X = CLookRotation.x;
            lookRotation.Y = CLookRotation.y;
            lookRotation.Z = CLookRotation.z;
            lookRotation.W = CLookRotation.w;

            pos.PosX = transform.position.x;
            pos.PosY = transform.position.y;
            pos.PosZ = transform.position.z;

            movePacket.Dir = dir;
            movePacket.LookRotation = lookRotation;
            movePacket.Position = pos;

            Managers.Network.Send(movePacket);

            yield return new WaitForSeconds(0.05f);
        }
    }
}
