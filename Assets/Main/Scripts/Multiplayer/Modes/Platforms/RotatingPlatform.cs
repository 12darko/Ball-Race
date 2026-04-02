using UnityEngine;

public class RotatingPlatform : PlatformBase
{
    public Vector3 rotationAxis = Vector3.up;
    public float speed = 50f;

    void Update()
    {
        transform.Rotate(rotationAxis * speed * Time.deltaTime);
    }
}