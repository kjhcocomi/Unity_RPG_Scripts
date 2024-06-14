using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public int Id { get; set; }
    public ObjectInfo ObjectInfo { get; set; }
    
    private void Start()
    {
        Init();
    }
    private void Update()
    {
        UpdateController();
    }
    public void SyncPosAndRot(PositionInfo posInfo, PQuaternion rotInfo)
    {
        transform.position = new Vector3(posInfo.PosX, posInfo.PosY, posInfo.PosZ);
        transform.rotation = new Quaternion(rotInfo.X, rotInfo.Y, rotInfo.Z, rotInfo.W);
    }
    protected virtual void Init() { }
    protected virtual void UpdateController() { } 
}
