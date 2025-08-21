using UnityEngine;

public class WarningZone : MonoBehaviour
{
    public GameObject lightningPrefab;
    public float delay = 0.5f;         // °æ°í ÈÄ ³«·Ú±îÁö Áö¿¬

    void Start()
    {
        Invoke(nameof(SpawnLightning), delay);
    }

    void SpawnLightning()
    {
        // ³«·Ú »ý¼º
        Instantiate(lightningPrefab, transform.position, Quaternion.identity);

        // WarningZone Á¦°Å
        Destroy(gameObject);
    }
}
