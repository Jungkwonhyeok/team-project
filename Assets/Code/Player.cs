using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;          // �̵� �ӵ�
    public Vector2 inputVec;          // �Է� ����

    private Vector2 lastDir = Vector2.down; // ������ �� ���� (�⺻: �Ʒ�)

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
        // �Է� �ޱ�
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        inputVec = new Vector2(h, v).normalized;

        // �Է��� ������ ������ ���� ������Ʈ (8���� ����)
        if (inputVec != Vector2.zero)
            lastDir = GetEightDirection(inputVec);

        // Animator �Ķ���� ����
        anim.SetFloat("MoveX", lastDir.x);
        anim.SetFloat("MoveY", lastDir.y);
        anim.SetFloat("Speed", inputVec.sqrMagnitude); // 0�̸� Idle, >0�̸� Move
    }

    void FixedUpdate()
    {
        // �̵� ó��
        Vector2 nextPos = rigid.position + inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextPos);
    }

    // �Է� ������ 8���� �� �ϳ��� ����
    Vector2 GetEightDirection(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (angle >= 67.5f && angle < 112.5f) return new Vector2(0, 1);       // ��
        if (angle <= -67.5f && angle > -112.5f) return new Vector2(0, -1);   // ��
        if (angle >= 22.5f && angle < 67.5f) return new Vector2(1, 1);       // ���
        if (angle >= -67.5f && angle < -22.5f) return new Vector2(1, -1);    // ����
        if (angle >= 112.5f && angle < 157.5f) return new Vector2(-1, 1);    // �»�
        if (angle >= -157.5f && angle < -112.5f) return new Vector2(-1, -1); // ����
        if (angle >= -22.5f && angle < 22.5f) return new Vector2(1, 0);      // ��
        if (angle >= -180f && angle < -157.5f || angle >= 157.5f && angle <= 180f) return new Vector2(-1, 0); // ��

        return dir.normalized;
    }
}
