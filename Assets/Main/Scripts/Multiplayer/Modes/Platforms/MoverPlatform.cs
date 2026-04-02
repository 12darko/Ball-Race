using UnityEngine;

public class MoverPlatform : PlatformBase
{
    public Vector3 movementDirection = Vector3.right;
    public float distance = 3f;
    public float speed = 2f;

    void Update()
    {
        float move = Mathf.PingPong(Time.time * speed, distance);
        transform.position = startPos + movementDirection * (move - distance / 2f);
    }
}