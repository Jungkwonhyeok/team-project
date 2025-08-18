using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossController : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;                 // 플레이어 트랜스폼 할당
    public Animator anim;                    // 보스 애니메이터
    [Tooltip("근접 판정 오브젝트(자식)")]
    public GameObject meleeHitboxGO;         // MeleeHitbox 자식 오브젝트

    [Header("Stats")]
    public int maxHP = 100;
    public int currentHP = 100;
    public float attackCooldown = 1.2f;      // 1타와 2연타 사이 텀
    public int meleeDamage = 15;

    [Header("Phase2 (<= 50%)")]
    public bool isPhase2 = false;
    public float lightningInterval = 3f;     // 낙뢰 주기
    public GameObject warningPrefab;         // WarningZone 프리팹
    public float warningOffsetY = 0f;        // 필요시 살짝 오프셋

    bool aiRunning = false;
    bool dead = false;

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        meleeHitboxGO.SetActive(false); // 안전하게 꺼두기
        currentHP = Mathf.Clamp(currentHP, 1, maxHP);
    }

    void OnEnable()
    {
        if (!aiRunning) StartCoroutine(AI());
    }

    // === 메인 AI 루프 ===
    IEnumerator AI()
    {
        aiRunning = true;

        while (!dead)
        {
            // 페이즈 체크
            if (!isPhase2 && currentHP <= maxHP / 2)
            {
                EnterPhase2();
            }

            // 근접 패턴: Phase1 = 1타 / Phase2 = 2연타
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

    // === 패턴: 근접 1타 ===
    IEnumerator MeleeOnce()
    {
        anim.SetTrigger("Attack");
        // 애니메이션 이벤트(AE_MeleeOn/Off)에서 실제 판정 on/off 처리
        // 대략 클립 길이에 맞춰 대기 (필요시 정확한 길이로 수정)
        yield return new WaitForSeconds(1.0f);
    }

    // === 패턴: 근접 2연타 ===
    IEnumerator MeleeCombo2()
    {
        // 1타
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.9f);

        // 2타
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.9f);
    }

    // === 페이즈2: 낙뢰 주기 루틴 ===
    IEnumerator LightningRoutine()
    {
        while (isPhase2 && !dead)
        {
            yield return new WaitForSeconds(lightningInterval);

            // 플레이어 발 밑 경고 생성
            if (player)
            {
                Vector3 pos = player.position;
                pos.y += warningOffsetY;
                Instantiate(warningPrefab, pos, Quaternion.identity);
            }
        }
    }

    // === 애니메이션 이벤트: 근접 판정 on/off ===
    // Attack 클립에 이벤트로 넣어주세요.
    public void AE_MeleeOn() { meleeHitboxGO.SetActive(true); }
    public void AE_MeleeOff() { meleeHitboxGO.SetActive(false); }

    // (선택) 캐스팅 모션 끝에 경고 생성 이벤트
    public void AE_FireLightning()
    {
        if (player)
        {
            Vector3 pos = player.position;
            pos.y += warningOffsetY;
            Instantiate(warningPrefab, pos, Quaternion.identity);
        }
    }

    // ==== 피격/사망 처리 (예시) ====
    public void TakeDamage(int amount)
    {
        if (dead) return;
        currentHP -= amount;
        if (currentHP <= 0)
        {
            dead = true;
            StopAllCoroutines();
            meleeHitboxGO.SetActive(false);
            // 사망 애니메이션/드롭 등 처리
            Destroy(gameObject, 1.5f);
        }
    }
}
