using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public LayerMask spinyMask;
    public float speed = 1;
    Transform spinyTransform;
    SpriteRenderer spinyRenderer;
    private Rigidbody2D spinyBody;
    float spinyWidth, spinyHeight;

    void Start()
    {
        spinyTransform = GetComponent<Transform>();
        spinyBody = GetComponent<Rigidbody2D>();
        spinyWidth = GetComponent<SpriteRenderer>() .bounds.extents.x;
        spinyHeight = GetComponent<SpriteRenderer>().bounds.extents.y;
    }

    private void FixedUpdate()
    {
        Vector2 lineCastPos = spinyTransform.position.ToVector2() - spinyTransform.right.ToVector2() * spinyWidth;

        bool isGrounded = Physics2D.Linecast(lineCastPos, lineCastPos + Vector2.down, spinyMask);
        bool isBlocked = Physics2D.Linecast(lineCastPos, lineCastPos - spinyTransform.right.ToVector2() * 0.05f, spinyMask);
        if(!isGrounded || isBlocked)
        {
            Vector3 currentRotation = spinyTransform.eulerAngles;
            currentRotation.y += 180;
            spinyTransform.eulerAngles = currentRotation;
        }
        Vector2 movement = spinyBody.velocity;
        movement.x = -spinyTransform.right.x * speed;
        spinyBody.velocity = movement;

    }
}