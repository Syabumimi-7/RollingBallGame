using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 camOffset = new Vector3(0f, 5f, -10f);  // 後ろ＋上
    private Transform target;
    private Rigidbody playerRB;

    public float positionSmoothTime = 0.2f;
    public float rotationSmoothTime = 0.15f;
    public float directionSmoothTime = 0.2f;

    private Vector3 velocitySmooth = Vector3.zero;
    private Vector3 currentLookDirection = Vector3.forward;

    void Start()
    {
        target = GameObject.Find("Player").transform;
        playerRB = target.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        if (target == null || playerRB == null) return;

        Vector3 velocity = playerRB.velocity;
        Vector3 targetDirection;

        // プレイヤーが動いているとき → 速度方向
        if (velocity.sqrMagnitude > 0.01f)
        {
            targetDirection = velocity.normalized;
        }
        // 止まっているとき → プレイヤーの forward
        else
        {
            targetDirection = target.forward;
        }

        // スムーズに方向を補間
        currentLookDirection = Vector3.Slerp(currentLookDirection, targetDirection, Time.deltaTime / directionSmoothTime);

        // カメラの理想位置を計算
        Vector3 desiredPosition = target.position - currentLookDirection * 10f + Vector3.up * camOffset.y;

        // カメラの位置をスムーズに移動
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocitySmooth, positionSmoothTime);

        // カメラがプレイヤーを向くように回転を補間
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime / rotationSmoothTime);
    }
}
