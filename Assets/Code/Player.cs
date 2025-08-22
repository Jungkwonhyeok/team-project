using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed ;          
    public Vector2 inputVec;
    public Scanner Scanner; 

    public Vector2 lastDir = Vector2.down; // 마지막 방향

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
        if (!GameManager.instance.isLive) //플레이어가 죽었으면 입력 받지 마라
            return;

        float h = Input.GetAxisRaw("Horizontal"); // A,D or ←, →
        float v = Input.GetAxisRaw("Vertical"); // W,S or ↑, ↓

        inputVec = new Vector2(h, v).normalized;

        
        if (inputVec != Vector2.zero)
            lastDir = GetEightDirection(inputVec);

        //Blend Tree 애니메이션 제어
        anim.SetFloat("MoveX", lastDir.x);
        anim.SetFloat("MoveY", lastDir.y);
        anim.SetFloat("Speed", inputVec.sqrMagnitude); //sqrMagnitude=벡터 크기의 제곱값
    }

    void FixedUpdate() //물리 이동 처리
    {
        if (!GameManager.instance.isLive)
            return;

        Vector2 nextPos = rigid.position + inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextPos);
    }

    
    Vector2 GetEightDirection(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // 360/8 범위 정함
        if (angle >= 67.5f && angle < 112.5f) return new Vector2(0, 1);     //위      
        if (angle <= -67.5f && angle > -112.5f) return new Vector2(0, -1);  //오른쪽 위 
        if (angle >= 22.5f && angle < 67.5f) return new Vector2(1, 1);       //오른쪽
        if (angle >= -67.5f && angle < -22.5f) return new Vector2(1, -1);    //아래
        if (angle >= 112.5f && angle < 157.5f) return new Vector2(-1, 1);    //왼쪽아래
        if (angle >= -157.5f && angle < -112.5f) return new Vector2(-1, -1); //왼쪽
        if (angle >= -22.5f && angle < 22.5f) return new Vector2(1, 0);      //왼쪽 위
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
