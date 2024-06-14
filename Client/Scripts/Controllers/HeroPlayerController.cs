using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroPlayerController : PlayerController
{
    [SerializeField] private SpringArm _springArm;

    private Vector3 _lookDir = Vector3.forward;
    private Vector2 _dirSum;
    private float rotX;
    private float rotY;

    [SerializeField] private float _sensitivity;
    [SerializeField] private float _clampAngle;
    public bool BlockInput { get; set; } = false;
    public bool CanUseDash = true;
    public bool CanUseSkill5 = true;
    public bool CanUseSkill6 = true;
    protected override void Init()
    {
        base.Init();
        _springArm = GetComponentInChildren<SpringArm>();
        _layerMask = ~LayerMask.GetMask("Player");
    }
    protected override void UpdateController()
    {
        base.UpdateController();
        StoreMouseInputInfo();
        HandleInput();
    }
    private void StoreMouseInputInfo()
    {
        //if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetKey(KeyCode.Mouse0) == false) return;
        rotX -= Input.GetAxis("Mouse Y") * _sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * _sensitivity * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, -_clampAngle, _clampAngle);
        if (_springArm)
            _springArm.SetRotation(rotX, rotY);
    }

    private void HandleInput()
    {
        // 기준 방향을 기준으로 하는 방향
        Vector3 localDirection = new Vector3(_dirSum.x, 0, _dirSum.y);

        // 카메라가 쳐다보는 방향에서 좌우 벡터만 추출, 기준 방향
        Vector3 lookDir = (Quaternion.Euler(0, rotY, 0) * Vector3.forward).normalized;

        Quaternion referenceRotation = Quaternion.LookRotation(lookDir);
        Vector3 localVector = referenceRotation * localDirection;

        Vector3 worldVector = transform.TransformDirection(localVector).normalized;

        _lookDir = Movement.CharacterMesh.transform.forward;

        if (_dirSum == Vector2.zero)
        {
            (Movement as PlayerMovement).CWorldDirVector = Vector3.zero;
        }
        else
        {
            (Movement as PlayerMovement).CLookRotation = referenceRotation;
            (Movement as PlayerMovement).CWorldDirVector = worldVector;
        }

        _dirSum = Vector2.zero;
    }
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
        if (BlockInput) return;
        StoreMoveInput();
        HandleSkillInput();
    }
    protected override void UpdateMoving()
    {
        base.UpdateMoving();
        if (BlockInput) return;
        StoreMoveInput();
        HandleSkillInput();
    }
    protected override void UpdateSkill()
    {
        base.UpdateSkill();
        if (BlockInput) return;
        HandleSkillInput();
    }
    private void StoreMoveInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _dirSum += Vector2.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _dirSum += Vector2.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _dirSum += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _dirSum += Vector2.right;
        }
        _dirSum = _dirSum.normalized;
    }
    int _layerMask;
    RaycastHit[] hits;
    Vector3 boxSize;
    Vector3 center;
    private void HandleSkillInput()
    {
        // 일반 콤보공격
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            C_Skill cSkillPacket = new C_Skill();
            if (_currentSkillId == 0)
            {
                cSkillPacket.SkillId = 1;
            }
            else if (_currentSkillId == 1)
            {
                cSkillPacket.SkillId = 2;
            }
            else if (_currentSkillId == 2)
            {
                cSkillPacket.SkillId = 3;
            }
            else if (_currentSkillId == 3)
            {
                cSkillPacket.SkillId = 4;
            }
            Managers.Network.Send(cSkillPacket);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))  // 1번 아이템
        {
            int consumeItemId = -1;
            if(Managers.Data.ConsumeItemQuickSlot.TryGetValue('1', out consumeItemId))
            {
                if (Managers.Data.ConsumeItemInfo[consumeItemId] <= 0) return;
                C_UseItem useItemPacket = new C_UseItem();
                useItemPacket.ItemId = consumeItemId;
                Managers.Network.Send(useItemPacket);
                Managers.Sound.Play("Posion");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))  // 2번 아이템
        {
            int consumeItemId = -1;
            if (Managers.Data.ConsumeItemQuickSlot.TryGetValue('2', out consumeItemId))
            {
                if (Managers.Data.ConsumeItemInfo[consumeItemId] <= 0) return;
                C_UseItem useItemPacket = new C_UseItem();
                useItemPacket.ItemId = consumeItemId;
                Managers.Network.Send(useItemPacket);
                Managers.Sound.Play("Posion");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))  // 스킬공격 (콤보 X)
        {
            if (_currentSkillId == 0)
            {
                int skillId;
                if (Managers.Data.SkillQuickSlot.TryGetValue('q', out skillId))
                {
                    if ((skillId == 5 && CanUseSkill5 == false) || (skillId == 6 && CanUseSkill6 == false)) return;
                    C_Skill cSkillPacket = new C_Skill();
                    cSkillPacket.SkillId = skillId;
                    Managers.Network.Send(cSkillPacket);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))  // 스킬공격 (콤보 X)
        {
            if (_currentSkillId == 0)
            {
                int skillId;
                if (Managers.Data.SkillQuickSlot.TryGetValue('e', out skillId))
                {
                    if ((skillId == 5 && CanUseSkill5 == false) || (skillId == 6 && CanUseSkill6 == false)) return;
                    C_Skill cSkillPacket = new C_Skill();
                    cSkillPacket.SkillId = skillId;
                    Managers.Network.Send(cSkillPacket);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))  // 대쉬
        {
            if (_currentSkillId == 0 && CanUseDash)
            {
                C_Skill cSkillPacket = new C_Skill();
                cSkillPacket.SkillId = 7;
                PositionInfo dirInfo = new PositionInfo();
                PositionInfo posInfo = new PositionInfo();
                dirInfo.PosX = _lookDir.x;
                dirInfo.PosY = _lookDir.y;
                dirInfo.PosZ = _lookDir.z;
                posInfo.PosX = transform.position.x;
                posInfo.PosY = transform.position.y;
                posInfo.PosZ = transform.position.z;
                cSkillPacket.Direction = dirInfo;
                cSkillPacket.Position = posInfo;
                Managers.Network.Send(cSkillPacket);
            }
        }
        else if(Input.GetKeyDown(KeyCode.I))
        {
            UI_Inventory inventory = Managers.UI.Root.GetComponentInChildren<UI_Inventory>();
            if (inventory)
            {
                inventory.ClosePopupUI();
            }
            else
            {
                Managers.UI.ShowPopupUI<UI_Inventory>();
            }
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            UI_Party inventory = Managers.UI.Root.GetComponentInChildren<UI_Party>();
            if (inventory)
            {
                inventory.ClosePopupUI();
            }
            else
            {
                Managers.UI.ShowPopupUI<UI_Party>();
            }
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(Managers.UI.GetPopupUICount() > 0)
            {
                Managers.UI.ClosePopupUI();
            }
            else
            {

            }
        }
    }

    public override void Die()
    {
        base.Die();

        // TODO : Respawn UI
        StartCoroutine(CoTestRespawn());
    }

    public override void Respawn(StatInfo stat, PositionInfo posInfo)
    {
        base.Respawn(stat, posInfo);
        Managers.Data.BaseStat = stat;
    }

    IEnumerator CoTestRespawn()
    {
        yield return new WaitForSeconds(5f);
        C_Respawn respawnPacket = new C_Respawn();
        respawnPacket.ObjectId = Id;
        Managers.Network.Send(respawnPacket);
    }

    public void CheckSkillRange(int skillId)
    {
        Dictionary<int, Data.Skill> dict = Managers.Data.SkillDict;
        float rangeX = dict[skillId].rangeX;
        float rangeZ = dict[skillId].rangeZ;
        float offset = dict[skillId].rangeOffset;

        boxSize = new Vector3(rangeX, 2.0f, rangeZ);
        center = transform.position + Movement.CharacterMesh.transform.forward * offset + Vector3.up * 3;
        hits = Physics.BoxCastAll(center, boxSize / 2, Vector3.down, Movement.CharacterMesh.transform.rotation, 3, _layerMask);

        C_CheckAttackRange checkAttackRangePacket = new C_CheckAttackRange();
        checkAttackRangePacket.SkillId = skillId;

        foreach (RaycastHit hit in hits)
        {
            CreatureController target = hit.transform.gameObject.GetComponent<CreatureController>();
            if(target)
            {
                if(ObjectManager.GetObjectTypeById(target.Id) == GameObjectType.Monster ||
                    ObjectManager.GetObjectTypeById(target.Id) == GameObjectType.MonsterBoss)
                {
                    checkAttackRangePacket.TargetIds.Add(target.Id);
                }
            }
        }

        Managers.Network.Send(checkAttackRangePacket);
    }
   
    private void OnDrawGizmos()
    {
        if(hits != null)
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
