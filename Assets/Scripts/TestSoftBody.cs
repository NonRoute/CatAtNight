using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSoftBody : MonoBehaviour
{

    Rigidbody2D rb;
    private float horizontal;
    private float vertical;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 50f;
    [SerializeField] LayerMask platformLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    [ContextMenu("Wake UP")]
    void ResetRB()
    {
        rb.WakeUp();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if(Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0f, jumpForce),ForceMode2D.Impulse);
        }

        RaycastHit2D hit = Physics2D.Raycast(origin: rb.transform.position, direction: Vector2.down
            , distance: 1f, layerMask: platformLayer);

        if (hit.collider != null)
        {
            Vector2 perpendicular = Vector2.Perpendicular(hit.normal);
            //print(perpendicular);
            Debug.DrawLine((Vector2)rb.transform.position, (Vector2)rb.transform.position + perpendicular, Color.yellow, 10.0f);
            rb.AddForce(moveSpeed * horizontal * -perpendicular);
        }
        else
        {
            rb.AddForce(new Vector2(moveSpeed * horizontal, 0f));
        }

    }
}
