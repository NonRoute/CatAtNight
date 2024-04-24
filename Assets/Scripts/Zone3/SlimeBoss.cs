using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBoss : MonoBehaviour
{
    [SerializeField] private bool isEnabled = false;
    [SerializeField] private bool isInterrupted = false;
    [SerializeField] private bool isGrounded = true;

    [SerializeField] private Transform bottom;
    [SerializeField] private Transform sprite;

    [SerializeField] private int health = 5;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 1.5f;
    [SerializeField] private float jumpCooldown = 1f;
    [SerializeField] private float shakeSpeed = 1f;
    [SerializeField] private float shakePower = 1f;
    [SerializeField] private float interruptDuration = 5f;

    [SerializeField] private bool isPhase2 = false;
    [SerializeField] private float moveSpeed_p2 = 10f;
    [SerializeField] private float jumpForce_p2 = 2f;
    [SerializeField] private float jumpCooldown_p2 = 2f;
    [SerializeField] private float shakeSpeed_p2 = 100f;
    [SerializeField] private float shakePower_p2 = 0.75f;
    [SerializeField] private float interruptDuration_p2 = 3f;
    [SerializeField] private float gravityScale_p2 = 1.5f;
    [SerializeField] private Color phase2Color = Color.white;


    [SerializeField] private float lastJumpTime;
    [SerializeField] private float interruptEndTime;

    [SerializeField] private DamageInfo damageInfo = new();
    [SerializeField] private float moveDirection = 1f;
    [SerializeField] private LayerMask floorLayer;

    [SerializeField] string actionText = "Attack the Slime";
    [SerializeField] string talkText = "Attack!";
    [SerializeField] string responseText = "OK!";


    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isEnabled) return;

        UpdateCondition();
        if (isInterrupted)
        {
            UpdateInterrupted();
            return;
        }

        float currentTime = Time.time;
        Move();
        if(currentTime - lastJumpTime > jumpCooldown)
        {
            Jump();
        }
    }

    private void UpdateCondition()
    {
        if (Time.time > interruptEndTime)
        {
            isInterrupted = false;
            sprite.transform.localPosition = Vector2.zero;
        }

        isGrounded = Physics2D.OverlapCircle(bottom.position, 2f, floorLayer);

        if (isGrounded)
        {
            Vector2 playerPos = GameplayStateManager.Instance.Player.GetCameraFollow().position;
            moveDirection = Mathf.Sign(playerPos.x - transform.position.x);
        }
    }

    private void Move()
    {
        float speed = moveSpeed;
        if(!isGrounded)
        {
            speed *= 1.5f;
        }
        rb.velocity = new Vector2(speed * moveDirection, rb.velocity.y);
    }

    private void Jump()
    {
        SoundManager.TryPlayNew("SlimeJump");
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        lastJumpTime = Time.time;
    }

    private void UpdateInterrupted()
    {
        sprite.transform.localPosition = shakePower * Mathf.Sin(Time.time * shakeSpeed) * Vector2.right;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print(collision.gameObject.name);
        if (collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            if (!damageInfo.targetEntityType.HasFlag(damagable.GetEntityType())) return;
            damagable.RecieveDamage(damageInfo, bottom.position + 2 * Vector3.down);
        }
        if (collision.gameObject.CompareTag("Companion"))
        {
            CompanionUIManager.Instance.OpenChoice3(actionText);
            GameplayStateManager.Instance.Player.SetUpChoice3(talkText, responseText);
            GameplayStateManager.Instance.Player.SetCompanionAction(CompanionAction.AttackBossZone3);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Companion"))
        {
            CompanionUIManager.Instance.HideChoice3();
            GameplayStateManager.Instance.Player.ClearChoice3();
        }
    }

    [ContextMenu("Damage")]
    public void ReceiveDamage()
    {
        if (isInterrupted) return;
        health -= 1;
        if(health <= 0)
        {
            OnDead();
        }
        else
        {
            SoundManager.TryPlayNew("SlimeHurt");
            isInterrupted = true;
            interruptEndTime = Time.time + interruptDuration;
            if(health <= 2 && !isPhase2)
            {
                StartPhase2();
            }
        }
    }

    public void StartPhase2()
    {
        isPhase2 = true;
        moveSpeed = moveSpeed_p2;
        jumpCooldown = jumpCooldown_p2;
        jumpForce = jumpForce_p2;
        rb.gravityScale = gravityScale_p2;
        shakeSpeed = shakeSpeed_p2;
        shakePower = shakePower_p2;
        interruptDuration = interruptDuration_p2;
        spriteRenderer.color = phase2Color;
    }

    public void OnDead()
    {
        DataManager.Instance.DestroyObject(gameObject);
        gameObject.SetActive(false);
        SoundManager.TryPlayNew("Boss3Dead");
        Zone3Manager.Instance.AfterBossDead();
    }

    public void ActivateBoss()
    {
        isEnabled = true;
        lastJumpTime = Time.time + 1.5f;
    }
}
