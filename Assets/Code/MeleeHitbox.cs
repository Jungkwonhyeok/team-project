using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    public int damage = 20;
    public string targetTag = "Player"; // �÷��̾ Ÿ��

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // ���� GameManager.health ���
        GameManager.instance.health -= damage;
        if (GameManager.instance.health <= 0)
        {
            GameManager.instance.health = 0;
            GameManager.instance.PlayerDie();
        }
    }

}
