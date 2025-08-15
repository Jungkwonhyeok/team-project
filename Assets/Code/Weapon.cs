using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;

    //float timer;
    // Player player;

    void Awake()
    {
        // player = GetComponentInParent<Player>();
    }

    void Start()
    {
        Init();
    }
    void Update()
    {
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                //timer += Time.deltaTime;

                //if (timer > speed)
                //{
                //    timer = 0f;
                //    Fire();
                // }
                break;
        }

        if (Input.GetButtonDown("Jump"))
        {
            Levelup(20, 5);
        }
    }

    public void Levelup(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        //if (id == 0)
        Batch();
    }

    public void Init()
    {
        switch (id)
        {
            case 0:
                speed = 150;
                Batch();
                break;
            default:
                //speed = 0.3f;
                break;
        }
    }

    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            Transform bullet;

            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -1);
        }
    }

    //void Fire()
    //{
    //    if (!player.Scanner.nearestTarget)
    //         return;

    //     Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
    //     bullet.position = transform.position;
    // }
}
