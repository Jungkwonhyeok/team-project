using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossController : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;                 // �÷��̾� Ʈ������ �Ҵ�
    public Animator anim;                    // ���� �ִϸ�����
    [Tooltip("���� ���� ������Ʈ(�ڽ�)")]
    public GameObject meleeHitboxGO;         // MeleeHitbox �ڽ� ������Ʈ

    [Header("Stats")]
    public int maxHP = 100;
    public int currentHP = 100;
    public float attackCooldown = 1.2f;      // 1Ÿ�� 2��Ÿ ���� ��
    public int meleeDamage = 15;

    [Header("Phase2 (<= 50%)")]
    public bool isPhase2 = false;
    public float lightningInterval = 3f;     // ���� �ֱ�
    public GameObject warningPrefab;         // WarningZone ������
    public float warningOffsetY = 0f;        // �ʿ�� ��¦ ������

    bool aiRunning = false;
    bool dead = false;

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        meleeHitboxGO.SetActive(false); // �����ϰ� ���α�
        currentHP = Mathf.Clamp(currentHP, 1, maxHP);
    }

    void OnEnable()
    {
        if (!aiRunning) StartCoroutine(AI());
    }

    // === ���� AI ���� ===
    IEnumerator AI()
    {
        aiRunning = true;

        while (!dead)
        {
            // ������ üũ
            if (!isPhase2 && currentHP <= maxHP / 2)
            {
                EnterPhase2();
            }

            // ���� ����: Phase1 = 1Ÿ / Phase2 = 2��Ÿ
            if (isPhase2)
                yield return StartCoroutine(MeleeCombo2());
            else
                yield return StartCoroutine(MeleeOnce());

            yield return new WaitForSeconds(attackCooldown);
        }
    }

    void EnterPhase2()
    {
        isPhase2 = true;
        anim.SetBool("Phase2", true);
        StartCoroutine(LightningRoutine());
    }

    // === ����: ���� 1Ÿ ===
    IEnumerator MeleeOnce()
    {
        anim.SetTrigger("Attack");
        // �ִϸ��̼� �̺�Ʈ(AE_MeleeOn/Off)���� ���� ���� on/off ó��
        // �뷫 Ŭ�� ���̿� ���� ��� (�ʿ�� ��Ȯ�� ���̷� ����)
        yield return new WaitForSeconds(1.0f);
    }

    // === ����: ���� 2��Ÿ ===
    IEnumerator MeleeCombo2()
    {
        // 1Ÿ
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.9f);

        // 2Ÿ
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.9f);
    }

    // === ������2: ���� �ֱ� ��ƾ ===
    IEnumerator LightningRoutine()
    {
        while (isPhase2 && !dead)
        {
            yield return new WaitForSeconds(lightningInterval);

            // �÷��̾� �� �� ��� ����
            if (player)
            {
                Vector3 pos = player.position;
                pos.y += warningOffsetY;
                Instantiate(warningPrefab, pos, Quaternion.identity);
            }
        }
    }

    // === �ִϸ��̼� �̺�Ʈ: ���� ���� on/off ===
    // Attack Ŭ���� �̺�Ʈ�� �־��ּ���.
    public void AE_MeleeOn() { meleeHitboxGO.SetActive(true); }
    public void AE_MeleeOff() { meleeHitboxGO.SetActive(false); }

    // (����) ĳ���� ��� ���� ��� ���� �̺�Ʈ
    public void AE_FireLightning()
    {
        if (player)
        {
            Vector3 pos = player.position;
            pos.y += warningOffsetY;
            Instantiate(warningPrefab, pos, Quaternion.identity);
        }
    }

    // ==== �ǰ�/��� ó�� (����) ====
    public void TakeDamage(int amount)
    {
        if (dead) return;
        currentHP -= amount;
        if (currentHP <= 0)
        {
            dead = true;
            StopAllCoroutines();
            meleeHitboxGO.SetActive(false);
            // ��� �ִϸ��̼�/��� �� ó��
            Destroy(gameObject, 1.5f);
        }
    }
}
