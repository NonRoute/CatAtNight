using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PipeVertex
{
    public Transform vertex;
    public float timeToTravel;
}

public class Pipe : MonoBehaviour
{
    [SerializeField] private List<PipeVertex> vertices;
    private List<PipeVertex> backwardVertices;

    [SerializeField] private Vector2 forwardDirection;
    [SerializeField] private float forwardSpeed;
    private Vector2 forwardVelocity;

    [SerializeField] private Vector2 backwardDirection;
    [SerializeField] private float backwardSpeed;
    private Vector2 backwardVelocity;

    private void Start()
    {
        backwardVertices = new(vertices);
        backwardVertices.Reverse();
        forwardVelocity = forwardDirection.normalized * forwardSpeed;
        backwardVelocity = backwardDirection.normalized * backwardSpeed;
    }

    public List<PipeVertex> GetVertices(bool isForward)
    {
        return isForward ? vertices : backwardVertices;
    }

    public Vector2 GetEndVelocity(bool isForward)
    {
        return isForward ? forwardVelocity : backwardVelocity;
    }
}
