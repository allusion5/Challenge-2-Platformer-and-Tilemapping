using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlockHitDetectionBehavior : MonoBehaviour
{
    public GameObject item;
    private Vector2 originalPosition;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private bool hasItem = true;

    Animator anim;
    Rigidbody2D itemBlock;

    void Start()
    {
        anim = GetComponent<Animator>();
        itemBlock = GetComponent<Rigidbody2D>();
        originalPosition = itemBlock.position;
        spawnPosition = new Vector3 (originalPosition.x, originalPosition.y);
        spawnRotation = Quaternion.identity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

            if (collision.gameObject.CompareTag("Player"))
            {
                if (hasItem == true)
                {
                itemBlock.MovePosition(originalPosition + new Vector2(0, 0.2f));
                Instantiate(item, spawnPosition, spawnRotation);
                FindObjectOfType<AudioManager>().Play("powerupSpawn");
                hasItem = false;
                }
            }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            itemBlock.MovePosition(originalPosition);
            anim.SetInteger("State", 1);
        }
    }


}
