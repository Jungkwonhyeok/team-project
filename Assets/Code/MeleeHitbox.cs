using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    public int damage = 20;
    public string targetTag = "Player"; // 플레이어만 타격

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 기존 GameManager.health 사용
        GameManager.instance.health -= damage;
        if (GameManager.instance.health <= 0)
        {
            GameManager.instance.health = 0;
            GameManager.instance.PlayerDie();
        }
    }

}
