using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomController : MonoBehaviour
{
    
    public LayerMask mushroomMask;
    public float speed = 4;
    Transform mushroomTransform;
    SpriteRenderer mushroomRenderer;
    private Rigidbody2D mushroomBody;
    private CircleCollider2D mushroomCollider;
    float mushroomWidth, mushroomHeight;

    void Start()
    {
        mushroomTransform = GetComponent<Transform>();
        mushroomBody = GetComponent<Rigidbody2D>();
        mushroomRenderer = GetComponent<SpriteRenderer>();
        mushroomWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        mushroomHeight = GetComponent<SpriteRenderer>().bounds.extents.y;
        mushroomCollider = GetComponent<CircleCollider2D>();

        StartCoroutine(itemSpawnFromBlock());

        IEnumerator itemSpawnFromBlock()
        {
            mushroomBody.AddForce(new Vector2(0, 60));
            yield return new WaitForSecondsRealtime(0.85f);
            mushroomBody.gravityScale = 1;
            mushroomRenderer.flipX = true;
            mushroomCollider.enabled = true;
            mushroomBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }    

    void FixedUpdate()
    {
        Vector2 lineCastPos = mushroomTransform.position.ToVector2() + mushroomTransform.right.ToVector2() * 0.5f*  mushroomWidth + Vector2.up * mushroomHeight;
        
        bool isBlocked = Physics2D.Linecast(lineCastPos, lineCastPos - mushroomTransform.right.ToVector2() * 1f, mushroomMask);
        Debug.DrawLine(lineCastPos, lineCastPos - mushroomTransform.right.ToVector2() * 0.5f, Color.yellow, 0f);
        if (isBlocked)
        {
            Vector3 currentRotation = mushroomTransform.eulerAngles;
            currentRotation.y += 180;
            mushroomTransform.eulerAngles = currentRotation;
        }
        Vector2 movement = mushroomBody.velocity;
        movement.x = mushroomTransform.right.x * speed;
        mushroomBody.velocity = movement;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Physics2D.IgnoreLayerCollision(1, 8);
        }
    }
}
