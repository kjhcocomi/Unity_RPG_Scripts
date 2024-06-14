using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringArm : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _camera;

    [SerializeField] private float _followSpeed;
    [SerializeField] private float smoothness;
   
    [SerializeField] private Vector3 targetToCamVector;
    [SerializeField] private Vector3 finalVector;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float finalDistance;

    private float _distance;
   
    void Start()
    {
        Init();
        _distance = maxDistance;
    }

    private void Update()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        _distance = Mathf.Clamp(_distance - wheelInput * Time.deltaTime * 300, 3f, maxDistance);
    }

    private void LateUpdate()
    {
        // Ÿ�� ���󰡱�
        transform.position = Vector3.MoveTowards(transform.position, _target.position, _followSpeed * Time.deltaTime);

        // ī�޶������ ���� * �ִ�Ÿ�
        finalVector = transform.TransformPoint(targetToCamVector.normalized * _distance);

        // �߰��� ��ֹ� �ִ��� �˻�, ������ ��ֹ� ������ ī�޶� ��ġ
        RaycastHit hit;
        int ingnoreMask = LayerMask.GetMask("Range") | LayerMask.GetMask("Player") | LayerMask.GetMask("Monster");
        if(Physics.Linecast(transform.position, finalVector, out hit, ~ingnoreMask))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, _distance);
        }
        else
        {
            finalDistance = _distance;
        }

        // �ε巴�� �����̵���
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, targetToCamVector.normalized * finalDistance, smoothness * Time.deltaTime);
    }

    private void Init()
    {
        _followSpeed = 10f;
        smoothness = 10f;
    }
    public void SetRotation(float inRotX, float inRotY)
    {
        // ȸ���� ����
        Quaternion rot = Quaternion.Euler(inRotX, inRotY, 0);
        transform.rotation = rot;
    }
}
