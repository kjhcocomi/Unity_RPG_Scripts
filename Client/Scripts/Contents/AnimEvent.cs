using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : MonoBehaviour
{
    public void Skill1()
    {
        GameObject effect = Managers.Resource.Instantiate("Effect/GroundHit");
        effect.transform.position = transform.position + transform.forward * 1.5f;
    }
    public void Skill2()
    {
        GameObject effect = Managers.Resource.Instantiate("Effect/Sword");
        Vector3 angle = effect.transform.rotation.eulerAngles;
        Vector3 offset = transform.rotation.eulerAngles;
        angle += offset;
        effect.transform.rotation = Quaternion.Euler(angle);
        effect.transform.position = transform.position + Vector3.up * 2 + transform.forward * 5;
    }
}
