using UnityEngine;

public class WarningZone : MonoBehaviour
{
    public GameObject lightningPrefab;
    public float delay = 0.5f;         // ��� �� ���ڱ��� ����

    void Start()
    {
        Invoke(nameof(SpawnLightning), delay);
    }

    void SpawnLightning()
    {
        // ���� ����
        Instantiate(lightningPrefab, transform.position, Quaternion.identity);

        // WarningZone ����
        Destroy(gameObject);
    }
}
