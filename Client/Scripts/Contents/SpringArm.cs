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
        // 타겟 따라가기
        transform.position = Vector3.MoveTowards(transform.position, _target.position, _followSpeed * Time.deltaTime);

        // 카메라까지의 방향 * 최대거리
        finalVector = transform.TransformPoint(targetToCamVector.normalized * _distance);

        // 중간에 장애물 있는지 검사, 있으면 장애물 앞으로 카메라 위치
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

        // 부드럽게 움직이도록
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, targetToCamVector.normalized * finalDistance, smoothness * Time.deltaTime);
    }

    private void Init()
    {
        _followSpeed = 10f;
        smoothness = 10f;
    }
    public void SetRotation(float inRotX, float inRotY)
    {
        // 회전값 설정
        Quaternion rot = Quaternion.Euler(inRotX, inRotY, 0);
        transform.rotation = rot;
    }
}
