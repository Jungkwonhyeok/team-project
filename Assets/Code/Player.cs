using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;          // 이동 속도
    public Vector2 inputVec;          // 입력 벡터

    private Vector2 lastDir = Vector2.down; // 마지막 본 방향 (기본: 아래)

    private Rigidbody2D rigid;
    private SpriteRenderer spriter;
    private Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 입력 받기
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        inputVec = new Vector2(h, v).normalized;

        // 입력이 있으면 마지막 방향 업데이트 (8방향 스냅)
        if (inputVec != Vector2.zero)
            lastDir = GetEightDirection(inputVec);

        // Animator 파라미터 전달
        anim.SetFloat("MoveX", lastDir.x);
        anim.SetFloat("MoveY", lastDir.y);
        anim.SetFloat("Speed", inputVec.sqrMagnitude); // 0이면 Idle, >0이면 Move
    }

    void FixedUpdate()
    {
        // 이동 처리
        Vector2 nextPos = rigid.position + inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextPos);
    }

    // 입력 방향을 8방향 중 하나로 스냅
    Vector2 GetEightDirection(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (angle >= 67.5f && angle < 112.5f) return new Vector2(0, 1);       // 상
        if (angle <= -67.5f && angle > -112.5f) return new Vector2(0, -1);   // 하
        if (angle >= 22.5f && angle < 67.5f) return new Vector2(1, 1);       // 우상
        if (angle >= -67.5f && angle < -22.5f) return new Vector2(1, -1);    // 우하
        if (angle >= 112.5f && angle < 157.5f) return new Vector2(-1, 1);    // 좌상
        if (angle >= -157.5f && angle < -112.5f) return new Vector2(-1, -1); // 좌하
        if (angle >= -22.5f && angle < 22.5f) return new Vector2(1, 0);      // 우
        if (angle >= -180f && angle < -157.5f || angle >= 157.5f && angle <= 180f) return new Vector2(-1, 0); // 좌

        return dir.normalized;
    }
}
