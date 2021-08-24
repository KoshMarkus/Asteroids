using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrappingObject : MonoBehaviour
{
    protected Camera cam;
    protected Rigidbody rb;
    public AudioClip destroyedSound;

    public Vector3 viewportPosition;

    virtual protected void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    virtual protected void FixedUpdate()
    {
        viewportPosition = cam.WorldToViewportPoint(transform.position);
        WrappingAround();
    }

    virtual protected void WrappingAround()
    {
        var newPosition = transform.position;

        if (viewportPosition.x > 1)
        {
            newPosition.x = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        }

        if (viewportPosition.y > 1)
        {
            newPosition.y = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        }

        if (viewportPosition.x < 0)
        {
            newPosition.x = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        }

        if (viewportPosition.y < 0)
        {
            newPosition.y = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        }

        transform.position = newPosition;
    }
}
