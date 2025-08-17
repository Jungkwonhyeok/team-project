using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public bool isLeft;
    public SpriteRenderer spriter;

    Player playerScript;
    SpriteRenderer playerSprite;

    Vector3 rightPos = new Vector3(-0.01f, -0.14f, 0);
    Vector3 rightPosReverse = new Vector3(-0.01f, -0.1f, 0);

    void Awake()
    {
        
        playerScript = GetComponentInParent<Player>();
        if (playerScript != null)
            playerSprite = playerScript.GetComponent<SpriteRenderer>();

        spriter = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void LateUpdate()
    {
        if (playerScript == null) return;

        
        Vector2 dir = playerScript.inputVec;
        if (dir == Vector2.zero)
            dir = playerScript.lastDir;

        bool isReverse = dir.x < 0; 

        
        if (isLeft)
        {
            spriter.flipY = isReverse;
            spriter.sortingOrder = 11;
        }
        else
        {
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            spriter.flipX = isReverse;
            spriter.sortingOrder = 11;
        }
        if (dir.y > 0) 
            spriter.sortingOrder = playerSprite.sortingOrder - 1; 
        else
            spriter.sortingOrder = playerSprite.sortingOrder + 1; 
    }
}
