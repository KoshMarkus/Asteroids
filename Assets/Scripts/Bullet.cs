using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ScreenWrappingObject
{
    [SerializeField] Material playerBulletMaterial;
    [SerializeField] Material alienBulletMaterial;

    [SerializeField] float speed;

    private bool isFired;

    [SerializeField] float currentDistance;

    private Vector3 originPosition;


    public void OnEnable()
    {
        isFired = false;
        originPosition = transform.position;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        currentDistance = Vector3.Distance(originPosition, transform.position);

        if (currentDistance > cam.ViewportToWorldPoint(new Vector3(1f, 0, 0)).x * 2)
        {
            gameObject.SetActive(false);
        }

        if (!isFired)
        {
            if (gameObject.CompareTag("PlayerBullet"))
            {
                foreach(Transform child in gameObject.transform.Find("Body").transform)
                {
                    child.GetComponent<Renderer>().material = playerBulletMaterial;
                }
            }
            else if (gameObject.CompareTag("AlienBullet"))
            {
                foreach (Transform child in gameObject.transform.Find("Body").transform)
                {
                    child.GetComponent<Renderer>().material = alienBulletMaterial;
                }
            }

            rb.velocity = new Vector3(0, 0, 0);
            rb.AddForce(transform.up*speed);

            isFired = true;
        }
    }

    override protected void WrappingAround()
    {
        var newPosition = transform.position;

        if (viewportPosition.x > 1)
        {
            newPosition.x = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            originPosition += newPosition * 2;
        }

        if (viewportPosition.y > 1)
        {
            newPosition.y = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
            originPosition += newPosition * 2;
        }

        if (viewportPosition.x < 0)
        {
            newPosition.x = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
            originPosition += newPosition * 2;
        }

        if (viewportPosition.y < 0)
        {
            newPosition.y = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
            originPosition += newPosition * 2;
        }

        transform.position = newPosition;
    }
}
