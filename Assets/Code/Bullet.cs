using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per; // °üÅë È½¼ö

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if (per > -1)
        {
            rigid.velocity = dir * 15f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (per == -1) return;

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null) enemy.health -= damage;
        }
        else if (collision.CompareTag("Boss"))
        {
            BossController boss = collision.GetComponent<BossController>();
            if (boss != null) boss.TakeDamage((int)damage);
        }

        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            per--;
            if (per == -1)
            {
                rigid.velocity = Vector2.zero;
                gameObject.SetActive(false);
            }
        }
    }
}
