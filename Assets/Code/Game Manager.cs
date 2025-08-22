using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("#Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    public bool isGaming = false;
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
    public Result uiResult;
    public GameObject enemyCLeaner;
    [Header("# Boss")]
    public GameObject bossPrefab;   // Boss ������
    public Transform bossSpawnPos;  // ���� ���� ��ġ
    private bool bossSpawned = false;
    public float bossAppearTime = 60f; // ���� ���� �� ?�ʿ� ���� ����


    void Awake()
    {
        instance = this;
    }

    public void GameStart()
    {
        health = maxhealth;
        isGaming = true;
        player.transform.GetChild(2).gameObject.SetActive(true);
        Resume();
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;
        enemyCLeaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCLeaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        if (!isLive)
            return;

        if(isGaming)
        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory();
        }

            // ���� ���� üũ
        if (!bossSpawned && gameTime >= bossAppearTime)
        {
             SpawnBoss();
             bossSpawned = true;
        }
    }

    

    public void GetExp()
    {
        if (!isLive)
            return;

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

    public void PlayerDie()
    {
        if (!isLive) return;   // �̹� �׾����� ���� �� ��
        isLive = false;

        

        // �÷��̾� �ִϸ��̼� ����
        player.GetComponent<Animator>().SetTrigger("die");

        // �ڽ� ������Ʈ�� ���� (����, HUD ��)
        
        for(int i = 0; i < player.transform.childCount; i++)
        {
            if(i >= 2)
            {
                player.transform.GetChild(i).gameObject.SetActive(false);
            }

            GameManager.instance.GameOver();
        }

   
    }


    public void SpawnBoss()
    {
        if (bossPrefab != null && bossSpawnPos != null)
        {
            GameObject boss = Instantiate(bossPrefab, bossSpawnPos.position, Quaternion.identity);
            // �ʿ��ϸ� BossController�� player �ʵ嵵 �ڵ� ����
            BossController bc = boss.GetComponent<BossController>();
            if (bc != null)
                bc.player = player.transform;
        }
    }
}
