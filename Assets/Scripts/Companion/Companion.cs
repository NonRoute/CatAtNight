using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour
{
    [SerializeField] bool isGoingToNextPos = false;
    [SerializeField] float timeToTravel = 2f;
    [SerializeField] float gravity = -10f;
    [SerializeField] Vector2 destination;
    private Vector2 startPos;
    private float travelEndTime;
    private float velocityX;
    private float velocityY;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            GoToPlayerPos();
        }
        if (isGoingToNextPos)
        {
            Vector2 displacement = new Vector2(velocityX,velocityY) * Time.deltaTime;
            transform.Translate(displacement);
            if (Time.time > travelEndTime)
            {
                transform.position = destination;
                isGoingToNextPos = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if(isGoingToNextPos)
        {
            velocityY += gravity * Time.fixedDeltaTime;
        }
    }

    [ContextMenu("Simulate Travel")]
    public void GoToNextPos()
    {
        GoToNextPos(destination);
    }

    [ContextMenu("Simulate Travel to Player")]
    public void GoToPlayerPos()
    {
        GoToNextPos(GameplayStateManager.Instance.Player.GetCameraFollow().position - 0.55f * Vector3.up);
    }

    public void GoToNextPos(Vector2 destination)
    {
        Vector2 startPos = transform.position;
        this.destination = destination;
        float deltaX =  destination.x - startPos.x;
        velocityX = deltaX / timeToTravel;
        float deltaY = destination.y - startPos.y;
        velocityY = deltaY / timeToTravel - 0.5f * gravity * timeToTravel;

        travelEndTime = Time.time + timeToTravel;
        isGoingToNextPos = true;
    }
}
