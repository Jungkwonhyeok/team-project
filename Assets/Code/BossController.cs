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

        // ���� ����: �����Ÿ� ���̸� ����
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

        // �¿� ���� (��������Ʈ���)
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
            if (anim) anim.SetBool("die", true); // ������
            Destroy(gameObject, 1.5f);
        }
        else
        {
            if (anim) anim.SetTrigger("Hit"); // ��Ʈ Ŭ�� ������
        }
    }

    public void OnHit(int damage)
    {
        if (curHealth <= 0) return;

        curHealth -= damage;
        anim.SetTrigger("Hit"); // �´� ��� ����

        if (curHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        anim.SetTrigger("Die"); // �״� ��� ����
        GetComponent<Collider2D>().enabled = false; // ���̻� �� �°�
        // �ʿ��ϸ� ���� ���߰ų� ���� �¸� ó��
    }

    void Update()
    {
        if (dead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            // �̵� ���߰� ����
            anim.SetBool("isWalking", false);
            Attack();
        }
        else
        {
            // �÷��̾� ����
            anim.SetBool("isWalking", true);
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    void Attack()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) // �ߺ� ����
        {
            anim.SetTrigger("Attack");
        }
    }

}