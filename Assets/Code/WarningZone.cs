using UnityEngine;

public class WarningZone : MonoBehaviour
{
    public GameObject lightningPrefab; // Lightning ������
    public float delay = 0.5f;         // ��� �� ���ڱ��� ����
    public int damage = 50;            // ���� ������ (Lightning�� �ѱ�)

    void Start()
    {
        Invoke(nameof(SpawnLightning), delay);
    }

    void SpawnLightning()
    {
        var go = Instantiate(lightningPrefab, transform.position, Quaternion.identity);

        // ���ڿ� ������ �Ķ���� ����
        var l = go.GetComponent<Lightning>();
        if (l != null) l.damage = damage;

        Destroy(gameObject);
    }
}
