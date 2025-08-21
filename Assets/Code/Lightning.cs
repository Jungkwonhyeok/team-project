using UnityEngine;

public class Lightning : MonoBehaviour
{
    public int damage = 50;
    public float lifeTime = 0.8f;
    public string targetTag = "Player"; // �÷��̾ Ÿ��

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.instance.health -= (float)damage;
        if (GameManager.instance.health <= 0)
        {
            GameManager.instance.health = 0;
            GameManager.instance.isLive = false;
            other.GetComponent<Animator>().SetTrigger("die");
        }
    }

}