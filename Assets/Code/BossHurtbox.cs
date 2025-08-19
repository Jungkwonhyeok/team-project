using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossHurtbox : MonoBehaviour
{
    public BossController boss;
    public Rigidbody2D rb;              // 있으면 약간 넉백
    public float knockback = 2f;

    void Reset()
    {
        // 에디터에서 자동 참조 시도
        boss = GetComponentInParent<BossController>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    void Awake()
    {
        if (!boss) boss = GetComponentInParent<BossController>();
        if (!rb) rb = GetComponentInParent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Bullet") || boss == null) return;

        var b = other.GetComponent<Bullet>();
        if (b == null) return;

        boss.TakeDamage(Mathf.RoundToInt(b.damage));

        // 선택: 플레이어 방향 기준 살짝 넉백
        if (rb && GameManager.instance && GameManager.instance.player)
        {
            Vector3 p = GameManager.instance.player.transform.position;
            Vector2 dir = (rb.position - (Vector2)p).normalized;
            rb.AddForce(dir * knockback, ForceMode2D.Impulse);
        }
    }
}