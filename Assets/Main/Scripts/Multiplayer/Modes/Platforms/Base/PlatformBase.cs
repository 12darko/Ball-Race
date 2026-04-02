using UnityEngine;

public abstract class PlatformBase : MonoBehaviour
{
    protected Transform player;

    protected Vector3 startPos;

 
    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    protected virtual void Start()
    {
        startPos = transform.position;
    }
    protected virtual void OnPlayerEnter() { }
    protected virtual void OnPlayerExit() { }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            OnPlayerEnter();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            OnPlayerExit();
    }
}