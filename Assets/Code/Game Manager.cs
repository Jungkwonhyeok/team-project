using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("#Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    [Header("# Player Info")]
    public float health;
    public float maxhealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 600 };
    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    [Header("# Boss")]
    public GameObject bossPrefab;   // Boss 프리팹
    public Transform bossSpawnPos;  // 보스 등장 위치
    private bool bossSpawned = false;
    public float bossAppearTime = 60f; // 게임 시작 후 60초에 보스 등장


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        health = maxhealth;
    }
    void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
        }

            // 보스 등장 체크
        if (!bossSpawned && gameTime >= bossAppearTime)
        {
             SpawnBoss();
             bossSpawned = true;
        }
    }

    

    public void GetExp()
    {
        exp++;

        if (exp == nextExp[level])
        {
            level++;
            exp = 0;
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }

    public void SpawnBoss()
    {
        if (bossPrefab != null && bossSpawnPos != null)
        {
            GameObject boss = Instantiate(bossPrefab, bossSpawnPos.position, Quaternion.identity);
            // 필요하면 BossController의 player 필드도 자동 연결
            BossController bc = boss.GetComponent<BossController>();
            if (bc != null)
                bc.player = player.transform;
        }
    }
}
