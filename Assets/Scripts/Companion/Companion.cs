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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            GoToPlayerPos();
        }
    }

    private void FixedUpdate()
    {
        if(isGoingToNextPos)
        {
            Vector2 displacement = new Vector2(velocityX,velocityY) * Time.fixedDeltaTime;
            transform.Translate(displacement);
            velocityY += gravity * Time.fixedDeltaTime;
            if(Time.time > travelEndTime)
            {
                transform.position = destination;
                isGoingToNextPos = false;
            }
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
        GoToNextPos(GameplayStateManager.Instance.Player.transform.position - 0.55f * Vector3.up);
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
