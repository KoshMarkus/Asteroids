using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ScreenWrappingObject, IPooledObject, IDestroyableObject
{
    [SerializeField] Material playerBulletMaterial;
    [SerializeField] Material alienBulletMaterial;

    private bool isFired;

    [SerializeField] float currentDistance;

    private GameObject originPoint;

    public void OnSpawnFromPool()
    {
        CreateOriginPoint();
        isFired = false;
    }

    virtual public void CreateOriginPoint()
    {
        if (!originPoint)
        {
            originPoint = new GameObject();
            originPoint.name = gameObject.name + " origin point";
        }
    }

    protected override void FixedUpdate()
    {
        viewportPosition = cam.WorldToViewportPoint(transform.position);
        WrappingAround();

        currentDistance = Vector3.Distance(originPoint.transform.position, transform.position);

        if (currentDistance > cam.ViewportToWorldPoint(new Vector3(1f, 0, 0)).x * 2)
        {
            Destroyed();
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
            rb.AddForce(transform.up.x*200, transform.up.y*200, 0);

            isFired = true;
        }
    }

    override protected void WrappingAround()
    {
        var newPosition = transform.position;

        if (viewportPosition.x > 1)
        {
            newPosition.x = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            originPoint.transform.position += newPosition * 2;
        }

        if (viewportPosition.y > 1)
        {
            newPosition.y = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
            originPoint.transform.position += newPosition * 2;
        }

        if (viewportPosition.x < 0)
        {
            newPosition.x = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
            originPoint.transform.position += newPosition * 2;
        }

        if (viewportPosition.y < 0)
        {
            newPosition.y = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
            originPoint.transform.position += newPosition * 2;
        }

        transform.position = newPosition;
    }

    public override void Destroyed()
    {
        Destroy(originPoint);
        gameObject.SetActive(false);
    }
}
