using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed ;          
    public Vector2 inputVec;
    public Scanner Scanner; 

    public Vector2 lastDir = Vector2.down; // ������ ����

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();   
        anim = GetComponent<Animator>();
        Scanner = GetComponent<Scanner>();
    }

    void Update()
    {
        if (!GameManager.instance.isLive) //�÷��̾ �׾����� �Է� ���� ����
            return;

        float h = Input.GetAxisRaw("Horizontal"); // A,D or ��, ��
        float v = Input.GetAxisRaw("Vertical"); // W,S or ��, ��

        inputVec = new Vector2(h, v).normalized;

        
        if (inputVec != Vector2.zero)
            lastDir = GetEightDirection(inputVec);

        //Blend Tree �ִϸ��̼� ����
        anim.SetFloat("MoveX", lastDir.x);
        anim.SetFloat("MoveY", lastDir.y);
        anim.SetFloat("Speed", inputVec.sqrMagnitude); //sqrMagnitude=���� ũ���� ������
    }

    void FixedUpdate() //���� �̵� ó��
    {
        if (!GameManager.instance.isLive)
            return;

        Vector2 nextPos = rigid.position + inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextPos);
    }

    
    Vector2 GetEightDirection(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // 360/8 ���� ����
        if (angle >= 67.5f && angle < 112.5f) return new Vector2(0, 1);     //��      
        if (angle <= -67.5f && angle > -112.5f) return new Vector2(0, -1);  //������ �� 
        if (angle >= 22.5f && angle < 67.5f) return new Vector2(1, 1);       //������
        if (angle >= -67.5f && angle < -22.5f) return new Vector2(1, -1);    //�Ʒ�
        if (angle >= 112.5f && angle < 157.5f) return new Vector2(-1, 1);    //���ʾƷ�
        if (angle >= -157.5f && angle < -112.5f) return new Vector2(-1, -1); //����
        if (angle >= -22.5f && angle < 22.5f) return new Vector2(1, 0);      //���� ��
        if (angle >= -180f && angle < -157.5f || angle >= 157.5f && angle <= 180f) return new Vector2(-1, 0);

        return dir.normalized;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
            return;

        GameManager.instance.health -= Time.deltaTime * 10;

        if(GameManager.instance.health < 0)
        {
            for (int index=2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anim.SetTrigger("die");
            GameManager.instance.GameOver();

        }
    }
}
