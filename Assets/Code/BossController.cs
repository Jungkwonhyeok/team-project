using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class BossController : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public Animator anim;
    public GameObject meleeHitboxGO;

    [Header("Stats")]
    public int maxHP = 100;
    public int currentHP = 100;
    public float attackCooldown = 1.2f;
    public int meleeDamage = 15;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float attackRange = 1.8f;

    [Header("Phase2 (<=50%)")]
    public bool isPhase2 = false;
    public float lightningInterval = 3f;
    public GameObject warningPrefab;
    public float warningOffsetY = 0f;

    bool aiRunning = false;
    bool dead = false;
    Rigidbody2D rb;

    public int maxHealth = 200;
    int curHealth;
    

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (meleeHitboxGO) meleeHitboxGO.SetActive(false);
        currentHP = Mathf.Clamp(currentHP, 1, maxHP);

        curHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (!aiRunning) StartCoroutine(AI());
    }

    void FixedUpdate()
    {
        if (dead || player == null || !GameManager.instance.isLive) return;

        // 간단 추적: 사정거리 밖이면 접근
        float dist = Vector2.Distance(rb.position, (Vector2)player.position);
        if (dist > attackRange)
        {
            Vector2 dir = ((Vector2)player.position - rb.position).normalized;
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
            if (anim) anim.SetBool("Walk", true);
        }
        else
        {
            if (anim) anim.SetBool("Walk", false);
        }

        // 좌우 반전 (스프라이트라면)
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.flipX = player.position.x < transform.position.x;
    }

    IEnumerator AI()
    {
        aiRunning = true;
        while (!dead && GameManager.instance.isLive)
        {
            if (!isPhase2 && currentHP <= maxHP / 2)
                EnterPhase2();

            if (isPhase2) yield return StartCoroutine(MeleeCombo2());
            else yield return StartCoroutine(MeleeOnce());

            yield return new WaitForSeconds(attackCooldown);
        }
    }

    void EnterPhase2()
    {
        isPhase2 = true;
        
        StartCoroutine(LightningRoutine());
    }

    IEnumerator MeleeOnce()
    {
        if (anim) anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator MeleeCombo2()
    {
        if (anim) anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.9f);
        if (anim) anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.9f);
    }

    IEnumerator LightningRoutine()
    {
        while (isPhase2 && !dead && GameManager.instance.isLive)
        {
            yield return new WaitForSeconds(lightningInterval);
            if (player && warningPrefab)
            {
                Vector3 pos = player.position; pos.y += warningOffsetY;
                Instantiate(warningPrefab, pos, Quaternion.identity);
            }
        }
    }

    public void AE_MeleeOn() { if (meleeHitboxGO) meleeHitboxGO.SetActive(true); }
    public void AE_MeleeOff() { if (meleeHitboxGO) meleeHitboxGO.SetActive(false); }
    public void AE_FireLightning()
    {
        if (player && warningPrefab)
        {
            Vector3 pos = player.position; pos.y += warningOffsetY;
            Instantiate(warningPrefab, pos, Quaternion.identity);
        }
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;
        currentHP -= amount;
        if (currentHP <= 0)
        {
            dead = true;
            StopAllCoroutines();
            if (meleeHitboxGO) meleeHitboxGO.SetActive(false);
            if (anim) anim.SetBool("die", true); // 있으면
            Destroy(gameObject, 1.5f);
        }
        else
        {
            if (anim) anim.SetTrigger("Hit"); // 히트 클립 있으면
        }
    }

    public void OnHit(int damage)
    {
        if (curHealth <= 0) return;

        curHealth -= damage;
        anim.SetTrigger("Hit"); // 맞는 모션 실행

        if (curHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        anim.SetTrigger("Die"); // 죽는 모션 실행
        GetComponent<Collider2D>().enabled = false; // 더이상 안 맞게
        // 필요하면 스폰 멈추거나 게임 승리 처리
    }

    void Update()
    {
        if (dead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            // 이동 멈추고 공격
            anim.SetBool("isWalking", false);
            Attack();
        }
        else
        {
            // 플레이어 추적
            anim.SetBool("isWalking", true);
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    void Attack()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) // 중복 방지
        {
            anim.SetTrigger("Attack");
        }
    }

}