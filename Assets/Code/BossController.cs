using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossController : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public Animator anim;
    public GameObject meleeHitboxGO;

    [Header("Stats")]
    public int maxHP = 500;
    public int currentHP = 500;
    public float attackCooldown = 1.2f;
    public int meleeDamage = 20;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float attackRange = 1.8f;

    [Header("Phase2 (<=50%)")]
    public bool isPhase2 = false;
    public float lightningInterval = 1f;
    public GameObject warningPrefab;      // °æ°í ÀÌÆåÆ®
    public GameObject lightningPrefab;    // ½ÇÁ¦ ³«·Ú ÀÌÆåÆ®
    public float warningOffsetY = 0f;

    bool aiRunning = false;
    bool dead = false;
    Rigidbody2D rb;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (meleeHitboxGO) meleeHitboxGO.SetActive(false);
        currentHP = maxHP;
    }

    void OnEnable()
    {
        if (!aiRunning) StartCoroutine(AI());
    }

    void FixedUpdate()
    {
        if (dead || player == null || !GameManager.instance.isLive) return;

        float dist = Vector2.Distance(rb.position, (Vector2)player.position);
        if (dist > attackRange)
        {
            Vector2 dir = ((Vector2)player.position - rb.position).normalized;
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
            
        }

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
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator MeleeCombo2()
    {
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.9f);
        anim.SetTrigger("Attack");
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

    // --- Animation Events ---
    public void AE_MeleeOn() { if (meleeHitboxGO) meleeHitboxGO.SetActive(true); }
    public void AE_MeleeOff() { if (meleeHitboxGO) meleeHitboxGO.SetActive(false); }

   
    // ½ÇÁ¦ ³«·Ú ¼ÒÈ¯
    public void CastLightning()
    {
        if (lightningPrefab == null || player == null) return;

        Vector3 spawnPos = player.position;
        spawnPos.y += warningOffsetY;

        Instantiate(lightningPrefab, spawnPos, Quaternion.identity);
    }

    // --- Damage & Death ---
    public void TakeDamage(int amount)
    {
        if (dead) return;

        currentHP -= amount;
        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("Hit");
        }
    }

    void Die()
    {
        dead = true;
        StopAllCoroutines();
        if (meleeHitboxGO) meleeHitboxGO.SetActive(false);

        anim.SetBool("die", true);
        GetComponent<Collider2D>().enabled = false;

        Invoke(nameof(DisableBoss), 1.5f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || dead) return;

        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null)
        {
            TakeDamage((int)bullet.damage);
            
        }
    }

    void DisableBoss()
    {
        gameObject.SetActive(false); // Ç®¿¡ µ¹·Áº¸³¿
    }
}
