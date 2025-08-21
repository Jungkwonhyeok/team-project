using UnityEngine;

public class WarningZone : MonoBehaviour
{
    public GameObject lightningPrefab; // Lightning 프리팹
    public float delay = 0.5f;         // 경고 후 낙뢰까지 지연
    public int damage = 50;            // 낙뢰 데미지 (Lightning에 넘김)

    void Start()
    {
        Invoke(nameof(SpawnLightning), delay);
    }

    void SpawnLightning()
    {
        var go = Instantiate(lightningPrefab, transform.position, Quaternion.identity);

        // 낙뢰에 데미지 파라미터 전달
        var l = go.GetComponent<Lightning>();
        if (l != null) l.damage = damage;

        Destroy(gameObject);
    }
}
