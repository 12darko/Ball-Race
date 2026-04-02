using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : PlatformBase
{
    public float waitTime = 0.5f;
    public float destroyTime = 2f;
    public Color warningColor = Color.red;

    private bool isFalling;
    private Rigidbody rb;
    private Renderer rend;
    private Color originalColor;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();

        rb.isKinematic = true;
        rb.useGravity = false;

        if (rend != null)
            originalColor = rend.material.color;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isFalling && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FallRoutine());
        }
    }

    IEnumerator FallRoutine()
    {
        isFalling = true;

        // UYARI
        if (rend != null)
            rend.material.color = warningColor;

        yield return new WaitForSeconds(waitTime);

        // DÜŞ
        rb.isKinematic = false;
        rb.useGravity = true;

        Destroy(gameObject, destroyTime);
    }
}