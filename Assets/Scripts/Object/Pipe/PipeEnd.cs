using UnityEngine;

public class PipeEnd : MonoBehaviour
{
    [SerializeField] private bool isForward;
    public bool IsForward => isForward;

    private Pipe pipe;
    public Pipe Pipe => pipe;

    private void Start()
    {
        pipe = transform.parent.GetComponent<Pipe>();
    }
}
