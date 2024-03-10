using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class YarnBall : MonoBehaviour
{
    [SerializeField] private DamageInfo damageInfo = new();
    [SerializeField] private float throwForceMultiplayer;

    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    public void Throw(bool isFacingRight)
    {
        rb.gravityScale = 1;
        rb.freezeRotation = false;
        rb.AddForce(new Vector2(isFacingRight ? 1 : -1, 1) * throwForceMultiplayer, ForceMode2D.Impulse);
        StartCoroutine(SelfDestruct());
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            if (!damageInfo.targetEntityType.HasFlag(damagable.GetEntityType())) return;
            ContactPoint2D contact = collision.GetContact(0);
            damagable.RecieveDamage(damageInfo, contact.point);
            Destroy(gameObject);
        }
    }
}
