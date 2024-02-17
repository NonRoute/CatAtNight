using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float jumpForce = 30;
    [SerializeField] private float chargePercent = 0;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalVelocity = Input.GetAxis("Horizontal") * moveSpeed;
        Vector2 newVelocity = rb.velocity;
        newVelocity.x = horizontalVelocity;
        rb.velocity = newVelocity;

        if(horizontalVelocity != 0)
        {
            if(horizontalVelocity > 0)
            {
                sprite.flipX = true;
            }
            else
            {
                sprite.flipX = false;
            }
        }

        chargePercent += 50 * Time.deltaTime;
        if(chargePercent > 100) 
        {
            chargePercent = 100;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            chargePercent = 0;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rb.AddForce(jumpForce * (1+chargePercent/100) * Vector2.up);
        }
    }
}
